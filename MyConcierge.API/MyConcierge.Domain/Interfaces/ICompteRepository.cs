using MyConcierge.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyConcierge.Domain.Interfaces
{
    public interface ICompteRepository
    {
        Task<List<Compte>> GetAllAsync();
        Task<Compte?> GetByIdAsync(int id);
        Task<Compte?> GetByCourrielAsync(string courriel);
        Task<bool> ExisteAuMoinsUnAsync();
        Task AjouterAsync(Compte compte);
        Task ChangerEstActifAsync(int id, bool estActif);
    }
}
