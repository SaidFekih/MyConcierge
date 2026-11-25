using Microsoft.AspNetCore.Mvc;
using MyConcierge.Application.Services;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;
using System.Threading.Tasks;

namespace MyConcierge.Presentation.Controllers
{
    [ApiController]
    [Route("api/paiements")]
    public class PaiementController : ControllerBase
    {
        private readonly IPaiementRepository _repository;
        private readonly PaiementService _service;

        public PaiementController(IPaiementRepository repository, PaiementService service)
        {
            _repository = repository;
            _service = service;
        }

        // GET api/paiements            → tous les paiements
        // GET api/paiements?contratId=5 → les paiements d'un contrat
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? contratId)
        {
            if (contratId != null)
                return Ok(await _repository.GetByContratIdAsync(contratId.Value));

            return Ok(await _repository.GetAllAsync());
        }

        // Un solde par contrat : loyer dû, total payé, solde
        [HttpGet("soldes")]
        public async Task<IActionResult> GetSoldes()
        {
            return Ok(await _service.CalculerSoldesAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var paiement = await _repository.GetByIdAsync(id);
            if (paiement == null) return NotFound();
            return Ok(paiement);
        }

        [HttpPost]
        public async Task<IActionResult> Ajouter([FromBody] Paiement paiement)
        {
            var erreur = await _service.AjouterAsync(paiement);
            if (erreur != null) return BadRequest(erreur);

            return CreatedAtAction(nameof(GetById), new { id = paiement.Id }, paiement);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Supprimer(int id)
        {
            if (await _repository.GetByIdAsync(id) == null) return NotFound();

            await _service.SupprimerAsync(id);
            return NoContent();
        }
    }
}
