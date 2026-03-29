using Microsoft.AspNetCore.Mvc;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;

namespace MyConcierge.Presentation.Controllers
{
    [ApiController]
    [Route("api/typeentites")]
    public class TypeEntiteController : ControllerBase
    {
        private readonly ITypeEntiteRepository _repository;

        public TypeEntiteController(ITypeEntiteRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [HttpGet("categorie/{categorie}")]
        public async Task<IActionResult> GetByCategorie(CategorieName categorie)
        {
            return Ok(await _repository.GetByCategorieAsync(categorie));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var typeEntite = await _repository.GetByIdAsync(id);
            if (typeEntite == null) return NotFound();
            return Ok(typeEntite);
        }

        [HttpPost]
        public async Task<IActionResult> Ajouter([FromBody] TypeEntite typeEntite)
        {
            await _repository.AjouterAsync(typeEntite);
            return CreatedAtAction(nameof(GetById), new { id = typeEntite.Id }, typeEntite);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Supprimer(int id)
        {
            await _repository.SupprimerAsync(id);
            return NoContent();
        }
    }
}
