using MyConcierge.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyConcierge.Domain.Interfaces
{
    public interface IPaiementRepository
    {
        Task<List<Paiement>> GetAllAsync();
        Task<Paiement?> GetByIdAsync(int id);
        Task<List<Paiement>> GetByContratIdAsync(int contratId);
        Task AjouterAsync(Paiement paiement);
        Task SupprimerAsync(int id);
    }
}
