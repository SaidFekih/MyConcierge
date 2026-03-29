using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;

namespace MyConcierge.Presentation.Controllers
{
    [ApiController]
    [Route("api/contrats")]
    public class ContratsLocationController : ControllerBase
    {
        private readonly IContratsLocationRepository _repository;

        public ContratsLocationController(IContratsLocationRepository repository)
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
            var contrat = await _repository.GetByIdAsync(id);
            if (contrat == null) return NotFound();
            return Ok(contrat);
        }

        [HttpPost]
        public async Task<IActionResult> Ajouter([FromBody] ContratsLocation contrat)
        {
            try
            {
                await _repository.AjouterAsync(contrat);
                return CreatedAtAction(nameof(GetById), new { id = contrat.Id }, contrat);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Modifier(int id, [FromBody] ContratsLocation contrat)
        {
            if (id != contrat.Id) return BadRequest();
            await _repository.ModifierAsync(contrat);
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ModifierPartiel(int id, [FromBody] JsonPatchDocument<ContratsLocation> patch)
        {
            if (patch == null) return BadRequest();
            await _repository.ModifierPartielAsync(id, patch);
            return NoContent();
        }

        [HttpPatch("{id}/terminer")]
        public async Task<IActionResult> Terminer(int id)
        {
            var contrat = await _repository.GetByIdAsync(id);
            if (contrat == null) return NotFound();

            contrat.DateFin = DateTime.Now;
            contrat.Unite!.Statut = StatutUnite.Disponible;
            await _repository.ModifierAsync(contrat);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Supprimer(int id)
        {
            await _repository.SupprimerAsync(id);
            return NoContent();
        }
    }
}
