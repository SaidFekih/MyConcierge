using Microsoft.EntityFrameworkCore;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;

namespace MyConcierge.Infrastructure.Repositories
{
    public class TypeEntiteRepository : ITypeEntiteRepository
    {
        private readonly AppDbContext _context;

        public TypeEntiteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TypeEntite>> GetAllAsync()
        {
            return await _context.TypeEntites.ToListAsync();
        }

        public async Task<List<TypeEntite>> GetByCategorieAsync(CategorieName categorie)
        {
            return await _context.TypeEntites
                .Where(t => t.Categorie == categorie)
                .ToListAsync();
        }

        public async Task<TypeEntite?> GetByIdAsync(int id)
        {
            return await _context.TypeEntites.FindAsync(id);
        }

        public async Task AjouterAsync(TypeEntite typeEntite)
        {
            _context.TypeEntites.Add(typeEntite);
            await _context.SaveChangesAsync();
        }

        public async Task SupprimerAsync(int id)
        {
            var typeEntite = await _context.TypeEntites.FindAsync(id);
            if (typeEntite != null)
            {
                _context.TypeEntites.Remove(typeEntite);
                await _context.SaveChangesAsync();
            }
        }
    }
}
