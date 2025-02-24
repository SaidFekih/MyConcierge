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

        [HttpPost]
        public async Task<IActionResult> Ajouter([FromBody] ContratsLocation contrat)
        {
            await _repository.AjouterAsync(contrat);
            return CreatedAtAction(nameof(GetAll), new { id = contrat.Id }, contrat);
        }
    }
}
