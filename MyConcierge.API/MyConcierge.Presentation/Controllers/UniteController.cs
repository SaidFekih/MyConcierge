using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;

namespace MyConcierge.Presentation.Controllers
{
    [ApiController]
    [Route("api/unites")]
    public class UniteController : ControllerBase
    {
        private readonly IUniteRepository _repository;

        public UniteController(IUniteRepository repository)
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
            var unite = await _repository.GetByIdAsync(id);
            if (unite == null) return NotFound();
            return Ok(unite);
        }

        [HttpGet("disponibles")]
        public async Task<IActionResult> GetDisponibles()
        {
            var unites = await _repository.GetAllAsync();
            return Ok(unites.Where(u => u.Statut == StatutUnite.Disponible));
        }

        [HttpPost]
        public async Task<IActionResult> Ajouter([FromBody] Unite unite)
        {
            await _repository.AjouterAsync(unite);
            return CreatedAtAction(nameof(GetAll), new { id = unite.Id }, unite);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Modifier(int id, [FromBody] Unite unite)
        {
            if (id != unite.Id) return BadRequest();
            await _repository.ModifierAsync(unite);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Supprimer(int id)
        {
            await _repository.SupprimerAsync(id);
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ModifierPartiel(int id, [FromBody] JsonPatchDocument<Unite> patch)
        {
            if (patch == null) return BadRequest();
            await _repository.ModifierPartielAsync(id, patch);
            return NoContent();
        }

    }
}
