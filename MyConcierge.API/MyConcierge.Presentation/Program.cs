using MyConcierge.Application.Services;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Infrastructure;
using MyConcierge.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MyConcierge.Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyConcierge.Presentation.Securite;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services pour l'API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Bouton Authorize dans Swagger : on colle le jeton reçu de POST /api/auth/connexion
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Colle ici le jeton reçu de POST /api/auth/connexion"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Active les Controllers
builder.Services.AddControllers(); 

// Ajouter la connexion à la base de données
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ajouter les services et repositories
builder.Services.AddScoped<IReferenceTypeRepository, ReferenceTypeRepository>();
builder.Services.AddScoped<ReferenceTypeRepository>();
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<UtilisateurRepository>();
builder.Services.AddScoped<IUniteRepository, UniteRepository>();
builder.Services.AddScoped<UniteRepository>();
builder.Services.AddScoped<IContratsLocationRepository, ContratsLocationRepository>();
builder.Services.AddScoped<ContratsLocationRepository>();
builder.Services.AddScoped<IPaiementRepository, PaiementRepository>();
builder.Services.AddScoped<PaiementRepository>();
builder.Services.AddScoped<ReferenceTypeService>();
builder.Services.AddScoped<ContratsLocationService>();
builder.Services.AddScoped<UniteService>();
builder.Services.AddScoped<UtilisateurService>();
builder.Services.AddScoped<PaiementService>();

// Comptes et connexion
builder.Services.AddScoped<ICompteRepository, CompteRepository>();
builder.Services.AddScoped<CompteService>();
builder.Services.AddScoped<JetonJwtService>();

// La clé secrète des jetons n'est jamais dans le code : elle vient des secrets (dotnet user-secrets) ou du serveur
var cleJwt = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(cleJwt) || cleJwt.Length < 32)
    throw new InvalidOperationException("La clé Jwt:Key est manquante ou trop courte (32 caractères minimum). Ajoute-la dans les secrets utilisateur du projet.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false; // on garde les noms du jeton (sub, email...)
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Emetteur"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cleJwt)),
            ValidateLifetime = true
        };
    });

// Toutes les routes exigent une connexion, sauf celles marquées [AllowAnonymous]
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});

// 5 essais de connexion par minute et par adresse IP
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("connexion", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        httpContext.Connection.RemoteIpAddress?.ToString() ?? "inconnue",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(1) }));
});


var app = builder.Build();

// Activer Swagger en mode développement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger/index.html")).AllowAnonymous();
}

app.UseHttpsRedirection();

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// Active les routes pour Swagger
app.MapControllers();

// Premier compte : créé au démarrage s'il n'existe encore aucun compte (réglage CompteInitial dans les secrets)
using (var portee = app.Services.CreateScope())
{
    try
    {
        var compteService = portee.ServiceProvider.GetRequiredService<CompteService>();
        var erreur = await compteService.CreerCompteInitialSiNecessaireAsync(
            app.Configuration["CompteInitial:Nom"],
            app.Configuration["CompteInitial:Courriel"],
            app.Configuration["CompteInitial:MotDePasse"]);

        if (erreur != null)
            app.Logger.LogWarning("Aucun compte n'existe et le compte initial n'a pas pu être créé : {Erreur}", erreur);
    }
    catch (Exception exception)
    {
        app.Logger.LogError(exception, "Impossible de vérifier ou créer le compte initial. La migration des comptes est-elle appliquée ?");
    }
}

// Lancer l'application
app.Run();
