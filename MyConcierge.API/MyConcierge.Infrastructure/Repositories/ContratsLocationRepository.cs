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
                .Include(c => c.Locataire)
                .Include(c => c.Unite)
                .ToListAsync();
        }

        public async Task<ContratsLocation?> GetByIdAsync(int id)
        {
            return await _context.ContratsLocations
                .Include(c => c.Locataire)
                .Include(c => c.Unite)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AjouterAsync(ContratsLocation contrat)
        {
            _context.ContratsLocations.Add(contrat);
            await _context.SaveChangesAsync();
        }

        public async Task SupprimerAsync(int id)
        {
            var contrat = await _context.ContratsLocations.FindAsync(id);
            if (contrat != null)
            {
                _context.ContratsLocations.Remove(contrat);
                await _context.SaveChangesAsync();
            }
        }
    }
}
