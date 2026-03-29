using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;

namespace MyConcierge.Presentation.Controllers
{
    [ApiController]
    [Route("api/utilisateurs")]
    public class UtilisateurController : ControllerBase
    {
        private readonly IUtilisateurRepository _repository;

        public UtilisateurController(IUtilisateurRepository repository)
        {
            _repository = repository;
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
            return CreatedAtAction(nameof(GetAll), new { id = utilisateur.Id }, utilisateur);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Modifier(int id, [FromBody] Utilisateur utilisateur)
        {
            if (id != utilisateur.Id) return BadRequest();
            await _repository.ModifierAsync(utilisateur);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Supprimer(int id)
        {
            await _repository.SupprimerAsync(id);
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ModifierUtilisateurlAsync(int id, [FromBody] JsonPatchDocument<Utilisateur> patch)
        {
            if (patch == null) return BadRequest();
            await _repository.ModifierUtilisateurlAsync(id, patch);
            return NoContent();
        }

    }
}
