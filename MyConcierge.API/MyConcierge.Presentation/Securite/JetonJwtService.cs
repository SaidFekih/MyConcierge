using Microsoft.IdentityModel.Tokens;
using MyConcierge.Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyConcierge.Presentation.Securite
{
    // Fabrique le jeton (JWT) que le frontend envoie ensuite à chaque appel.
    // Le réglage (clé secrète, durée) vient de la configuration : voir Jwt dans appsettings.json et les secrets.
    public class JetonJwtService
    {
        private readonly IConfiguration _configuration;

        public JetonJwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreerJeton(Compte compte)
        {
            var cle = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var heuresDeValidite = int.Parse(_configuration["Jwt:DureeEnHeures"] ?? "8");

            var informations = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, compte.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, compte.Courriel),
                new Claim("nom", compte.Nom)
            };

            var jeton = new JwtSecurityToken(
                issuer: _configuration["Jwt:Emetteur"],
                audience: _configuration["Jwt:Audience"],
                claims: informations,
                expires: DateTime.UtcNow.AddHours(heuresDeValidite),
                signingCredentials: new SigningCredentials(cle, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jeton);
        }
    }
}
