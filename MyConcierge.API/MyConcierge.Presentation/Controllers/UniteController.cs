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

        [HttpPost]
        public async Task<IActionResult> Ajouter([FromBody] Unite unite)
        {
            await _repository.AjouterAsync(unite);
            return CreatedAtAction(nameof(GetAll), new { id = unite.Id }, unite);
        }
    }
}
