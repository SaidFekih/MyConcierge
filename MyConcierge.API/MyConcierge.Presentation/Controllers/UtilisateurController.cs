using Microsoft.AspNetCore.Mvc;
using MyConcierge.Application.Services;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;

namespace MyConcierge.Presentation.Controllers
{
    [ApiController]
    [Route("api/utilisateurs")]
    public class UtilisateurController : ControllerBase
    {
        private readonly IUtilisateurRepository _repository;
        private readonly UtilisateurService _service;

        public UtilisateurController(IUtilisateurRepository repository, UtilisateurService service)
        {
            _repository = repository;
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var utilisateur = await _repository.GetByIdAsync(id);
            if (utilisateur == null) return NotFound();
            return Ok(utilisateur);
        }

        [HttpPost]
        public async Task<IActionResult> Ajouter([FromBody] Utilisateur utilisateur)
        {
            await _repository.AjouterAsync(utilisateur);
            return CreatedAtAction(nameof(GetById), new { id = utilisateur.Id }, utilisateur);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Modifier(int id, [FromBody] Utilisateur utilisateur)
        {
            if (id != utilisateur.Id) return BadRequest("L'id de l'URL ne correspond pas à l'id du corps.");
            if (await _repository.GetByIdAsync(id) == null) return NotFound();

            await _repository.ModifierAsync(utilisateur);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Supprimer(int id)
        {
            if (await _repository.GetByIdAsync(id) == null) return NotFound();

            var erreur = await _service.SupprimerAsync(id);
            if (erreur != null) return BadRequest(erreur);

            return NoContent();
        }
    }
}
