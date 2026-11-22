using Microsoft.AspNetCore.Mvc;
using MyConcierge.Application.Services;
using MyConcierge.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyConcierge.Presentation.Controllers
{
    [ApiController]
    [Route("api/referencetypes")]
    public class ReferenceTypeController : ControllerBase
    {
        private readonly ReferenceTypeService _service;

        public ReferenceTypeController(ReferenceTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.ObtenirTousAsync());

        [HttpPost]
        public async Task<IActionResult> Ajouter([FromBody] ReferenceType referenceType)
        {
            await _service.AjouterAsync(referenceType);
            return CreatedAtAction(nameof(GetAll), new { id = referenceType.Id }, referenceType);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Modifier(int id, [FromBody] ReferenceType referenceType)
        {
            if (id != referenceType.Id) return BadRequest("L'id de l'URL ne correspond pas à l'id du corps.");
            if (await _service.ObtenirParIdAsync(id) == null) return NotFound();

            await _service.ModifierAsync(referenceType);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Supprimer(int id)
        {
            if (await _service.ObtenirParIdAsync(id) == null) return NotFound();

            var erreur = await _service.SupprimerAsync(id);
            if (erreur != null) return BadRequest(erreur);

            return NoContent();
        }
    }
}
