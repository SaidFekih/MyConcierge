using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MyConcierge.Infrastructure.Repositories
{
    public class UtilisateurRepository : IUtilisateurRepository
    {
        private readonly AppDbContext _context;

        public UtilisateurRepository(AppDbContext context)
        {
            _context = context;
        }

       public async Task<List<Utilisateur>> GetAllAsync()
        {
            return await _context.Utilisateurs.Include(u => u.ReferenceType).ToListAsync();
        }

        public async Task<Utilisateur?> GetByIdAsync(int id)
        {
            return await _context.Utilisateurs
                .Include(u => u.ReferenceType)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AjouterAsync(Utilisateur utilisateur)
        {
            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();
        }

        public async Task SupprimerAsync(int id)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur != null)
            {
                _context.Utilisateurs.Remove(utilisateur);
                await _context.SaveChangesAsync();
            }
        }
    }
}
