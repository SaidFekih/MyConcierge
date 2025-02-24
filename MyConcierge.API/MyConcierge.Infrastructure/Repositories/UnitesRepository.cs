using Microsoft.EntityFrameworkCore;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyConcierge.Infrastructure.Repositories
{
    public class UniteRepository : IUniteRepository
    {
        private readonly AppDbContext _context;

        public UniteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Unite>> GetAllAsync()
        {
            return await _context.Unites
                .Include(u => u.ReferenceType)
                .Include(u => u.ParentUnite)
                .Include(u => u.Proprietaire)
                .ToListAsync();
        }

        public async Task<Unite?> GetByIdAsync(int id)
        {
            return await _context.Unites
                .Include(u => u.ReferenceType)
                .Include(u => u.ParentUnite)
                .Include(u => u.Proprietaire)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AjouterAsync(Unite unite)
        {
            _context.Unites.Add(unite);
            await _context.SaveChangesAsync();
        }

        public async Task SupprimerAsync(int id)
        {
            var unite = await _context.Unites.FindAsync(id);
            if (unite != null)
            {
                _context.Unites.Remove(unite);
                await _context.SaveChangesAsync();
            }
        }
    }
}
