using Microsoft.AspNetCore.JsonPatch;
using MyConcierge.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyConcierge.Domain.Interfaces
{
    public interface IContratsLocationRepository
    {
        Task<List<ContratsLocation>> GetAllAsync();
        Task<ContratsLocation?> GetByIdAsync(int id);
        Task AjouterAsync(ContratsLocation contrat);
        Task ModifierAsync(ContratsLocation contrat);
        Task ModifierPartielAsync(int id, JsonPatchDocument<ContratsLocation> patch);
        Task SupprimerAsync(int id);
    }
}
