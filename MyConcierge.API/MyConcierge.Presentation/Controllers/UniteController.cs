using Microsoft.AspNetCore.Mvc;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;
using MyConcierge.Application.Services;

namespace MyConcierge.Presentation.Controllers
{
    [ApiController]
    [Route("api/unites")]
    public class UniteController : ControllerBase
    {
        private readonly IUniteRepository _repository;
        private readonly UniteService _service;

        public UniteController(IUniteRepository repository, UniteService service)
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
            var unite = await _repository.GetByIdAsync(id);
            if (unite == null) return NotFound();
            return Ok(unite);
        }

        [HttpPost]
        public async Task<IActionResult> Ajouter([FromBody] Unite unite)
        {
            await _repository.AjouterAsync(unite);
            return CreatedAtAction(nameof(GetById), new { id = unite.Id }, unite);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Modifier(int id, [FromBody] Unite unite)
        {
            if (id != unite.Id) return BadRequest("L'id de l'URL ne correspond pas à l'id du corps.");
            if (await _repository.GetByIdAsync(id) == null) return NotFound();

            await _repository.ModifierAsync(unite);
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
