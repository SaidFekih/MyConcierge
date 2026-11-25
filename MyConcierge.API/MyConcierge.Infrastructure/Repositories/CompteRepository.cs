using Microsoft.EntityFrameworkCore;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyConcierge.Infrastructure.Repositories
{
    public class CompteRepository : ICompteRepository
    {
        private readonly AppDbContext _context;

        public CompteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Compte>> GetAllAsync()
        {
            return await _context.Comptes
                .AsNoTracking()
                .OrderBy(compte => compte.Nom)
                .ToListAsync();
        }

        public async Task<Compte?> GetByIdAsync(int id)
        {
            return await _context.Comptes
                .AsNoTracking()
                .FirstOrDefaultAsync(compte => compte.Id == id);
        }

        // Le courriel doit déjà être en minuscules
        public async Task<Compte?> GetByCourrielAsync(string courriel)
        {
            return await _context.Comptes
                .AsNoTracking()
                .FirstOrDefaultAsync(compte => compte.Courriel == courriel);
        }

        public async Task<bool> ExisteAuMoinsUnAsync()
        {
            return await _context.Comptes.AnyAsync();
        }

        public async Task AjouterAsync(Compte compte)
        {
            _context.Comptes.Add(compte);
            await _context.SaveChangesAsync();
        }

        public async Task ChangerEstActifAsync(int id, bool estActif)
        {
            var compte = await _context.Comptes.FindAsync(id);
            if (compte != null)
            {
                compte.EstActif = estActif;
                await _context.SaveChangesAsync();
            }
        }
    }
}
