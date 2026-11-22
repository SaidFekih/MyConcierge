using Microsoft.EntityFrameworkCore;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyConcierge.Infrastructure.Repositories
{
    public class ContratsLocationRepository : IContratsLocationRepository
    {
        private readonly AppDbContext _context;

        public ContratsLocationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ContratsLocation>> GetAllAsync()
        {
            return await _context.ContratsLocations
                .Where(contrat => !contrat.EstSupprime)
                .Include(c => c.Locataire)
                .Include(c => c.Unite)
                .ToListAsync();
        }

        public async Task<ContratsLocation?> GetByIdAsync(int id)
        {
            return await _context.ContratsLocations
                .AsNoTracking()
                .Include(c => c.Locataire)
                .Include(c => c.Unite)
                .FirstOrDefaultAsync(contrat => contrat.Id == id && !contrat.EstSupprime);
        }

        public async Task<List<ContratsLocation>> GetByUniteIdAsync(int uniteId)
        {
            return await _context.ContratsLocations
                .AsNoTracking()
                .Where(contrat => contrat.UniteId == uniteId && !contrat.EstSupprime)
                .ToListAsync();
        }

        public async Task AjouterAsync(ContratsLocation contrat)
        {
            _context.ContratsLocations.Add(contrat);
            await _context.SaveChangesAsync();
        }

        public async Task ModifierAsync(ContratsLocation contrat)
        {
            _context.ContratsLocations.Update(contrat);
            await _context.SaveChangesAsync();
        }

        public async Task SupprimerAsync(int id)
        {
            var contrat = await _context.ContratsLocations.FindAsync(id);
            if (contrat != null)
            {
                contrat.EstSupprime = true;
                contrat.DateSuppression = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}
