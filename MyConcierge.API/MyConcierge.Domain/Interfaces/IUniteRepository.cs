using MyConcierge.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyConcierge.Domain.Interfaces
{
    public interface IUniteRepository
    {
        Task<List<Unite>> GetAllAsync();
        Task<Unite?> GetByIdAsync(int id);
        Task AjouterAsync(Unite unite);
        Task SupprimerAsync(int id);
    }
}
