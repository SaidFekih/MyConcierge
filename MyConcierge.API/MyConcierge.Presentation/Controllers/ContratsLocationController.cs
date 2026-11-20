using Microsoft.AspNetCore.Mvc;
using MyConcierge.Application.Services;
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
        private readonly ContratsLocationService _service;

        public ContratsLocationController(IContratsLocationRepository repository, ContratsLocationService service)
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
            var contrat = await _repository.GetByIdAsync(id);
            if (contrat == null) return NotFound();
            return Ok(contrat);
        }

        [HttpPost]
        public async Task<IActionResult> Ajouter([FromBody] ContratsLocation contrat)
        {
            var erreur = await _service.AjouterAsync(contrat);
            if (erreur != null) return BadRequest(erreur);

            return CreatedAtAction(nameof(GetById), new { id = contrat.Id }, contrat);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Modifier(int id, [FromBody] ContratsLocation contrat)
        {
            if (id != contrat.Id) return BadRequest("L'id de l'URL ne correspond pas à l'id du corps.");
            if (await _repository.GetByIdAsync(id) == null) return NotFound();

            var erreur = await _service.ModifierAsync(contrat);
            if (erreur != null) return BadRequest(erreur);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Supprimer(int id)
        {
            if (await _repository.GetByIdAsync(id) == null) return NotFound();

            await _service.SupprimerAsync(id);
            return NoContent();
        }
    }
}
