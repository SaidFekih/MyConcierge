using Microsoft.AspNetCore.Mvc;
using MyConcierge.Application.Services;

namespace MyConcierge.Presentation.Controllers
{
    public record DemandeCreationCompte(string Nom, string Courriel, string MotDePasse);

    // Gestion des comptes : toute personne connectée peut créer ou désactiver un compte
    [ApiController]
    [Route("api/comptes")]
    public class ComptesController : ControllerBase
    {
        private readonly CompteService _service;

        public ComptesController(CompteService service)
        {
            _service = service;
        }

        // On ne renvoie jamais le mot de passe haché
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comptes = await _service.ListerAsync();
            return Ok(comptes.Select(compte => new
            {
                compte.Id,
                compte.Nom,
                compte.Courriel,
                compte.EstActif,
                compte.DateCreation
            }));
        }

        [HttpPost]
        public async Task<IActionResult> Creer([FromBody] DemandeCreationCompte demande)
        {
            var erreur = await _service.CreerCompteAsync(demande.Nom, demande.Courriel, demande.MotDePasse);
            if (erreur != null) return BadRequest(erreur);

            return Ok();
        }

        [HttpPost("{id:int}/desactiver")]
        public async Task<IActionResult> Desactiver(int id)
        {
            var erreur = await _service.ChangerEstActifAsync(id, false, ObtenirIdDuCompteConnecte());
            if (erreur != null) return BadRequest(erreur);

            return NoContent();
        }

        [HttpPost("{id:int}/reactiver")]
        public async Task<IActionResult> Reactiver(int id)
        {
            var erreur = await _service.ChangerEstActifAsync(id, true, ObtenirIdDuCompteConnecte());
            if (erreur != null) return BadRequest(erreur);

            return NoContent();
        }

        // L'identifiant du compte est dans le jeton (champ « sub »)
        private int ObtenirIdDuCompteConnecte()
        {
            return int.Parse(User.FindFirst("sub")!.Value);
        }
    }
}
