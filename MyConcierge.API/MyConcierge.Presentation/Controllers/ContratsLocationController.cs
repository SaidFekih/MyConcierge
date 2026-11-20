using Microsoft.AspNetCore.Mvc;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            await _repository.AjouterAsync(contrat);
            return CreatedAtAction(nameof(GetById), new { id = contrat.Id }, contrat);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Modifier(int id, [FromBody] ContratsLocation contrat)
        {
            if (id != contrat.Id) return BadRequest("L'id de l'URL ne correspond pas à l'id du corps.");
            if (await _repository.GetByIdAsync(id) == null) return NotFound();

            await _repository.ModifierAsync(contrat);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Supprimer(int id)
        {
            if (await _repository.GetByIdAsync(id) == null) return NotFound();

            await _repository.SupprimerAsync(id);
            return NoContent();
        }
    }
}
