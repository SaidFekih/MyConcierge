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
        Task SupprimerAsync(int id);
    }
}
