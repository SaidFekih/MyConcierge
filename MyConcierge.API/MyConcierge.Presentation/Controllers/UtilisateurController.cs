using Microsoft.AspNetCore.Mvc;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;

namespace MyConcierge.Presentation.Controllers
{
    [ApiController]
    [Route("api/utilisateurs")]
    public class UtilisateurController : ControllerBase
    {
        private readonly IUtilisateurRepository _repository;

        public UtilisateurController(IUtilisateurRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Ajouter([FromBody] Utilisateur utilisateur)
        {
            await _repository.AjouterAsync(utilisateur);
            return CreatedAtAction(nameof(GetAll), new { id = utilisateur.Id }, utilisateur);
        }
    }
}
