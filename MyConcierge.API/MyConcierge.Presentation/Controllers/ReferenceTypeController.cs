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
    }
}
