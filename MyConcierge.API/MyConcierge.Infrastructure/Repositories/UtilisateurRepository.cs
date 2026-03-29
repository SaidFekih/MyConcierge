using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;

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
            try
            {
                return await _context.Utilisateurs.Include(u => u.TypeEntite).ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public async Task<Utilisateur?> GetByIdAsync(int id)
        {
            return await _context.Utilisateurs
                .Include(u => u.TypeEntite)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AjouterAsync(Utilisateur utilisateur)
        {
            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();
        }

        public async Task ModifierAsync(Utilisateur utilisateur)
        {
            _context.Utilisateurs.Update(utilisateur);
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

        public async Task ModifierUtilisateurlAsync(int id, JsonPatchDocument<Utilisateur> patch)
        {
            var Utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (Utilisateur == null) return;

            patch.ApplyTo(Utilisateur);
            await _context.SaveChangesAsync();
        }

    }
}
