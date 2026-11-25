using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyConcierge.Application.Services;
using MyConcierge.Presentation.Securite;

namespace MyConcierge.Presentation.Controllers
{
    public record DemandeConnexion(string Courriel, string MotDePasse);

    // La seule route accessible sans être connecté
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly CompteService _compteService;
        private readonly JetonJwtService _jetonService;

        public AuthController(CompteService compteService, JetonJwtService jetonService)
        {
            _compteService = compteService;
            _jetonService = jetonService;
        }

        // Limitée à quelques essais par minute (contre les devinettes de mot de passe)
        [AllowAnonymous]
        [EnableRateLimiting("connexion")]
        [HttpPost("connexion")]
        public async Task<IActionResult> Connexion([FromBody] DemandeConnexion demande)
        {
            var compte = await _compteService.VerifierConnexionAsync(demande.Courriel, demande.MotDePasse);

            // Même message si le courriel n'existe pas ou si le mot de passe est faux
            if (compte == null)
                return Unauthorized("Courriel ou mot de passe incorrect.");

            return Ok(new
            {
                jeton = _jetonService.CreerJeton(compte),
                nom = compte.Nom,
                courriel = compte.Courriel
            });
        }
    }
}
