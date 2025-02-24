using MyConcierge.Application.Services;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Infrastructure;
using MyConcierge.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MyConcierge.Domain.Models;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services pour l'API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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


var app = builder.Build();

// Activer Swagger en mode développement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Active les routes pour Swagger
app.MapControllers(); 

// Lancer l'application
app.Run();
