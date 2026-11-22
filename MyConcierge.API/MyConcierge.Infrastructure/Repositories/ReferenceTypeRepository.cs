using Microsoft.EntityFrameworkCore;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyConcierge.Infrastructure.Repositories
{
    public class ReferenceTypeRepository : IReferenceTypeRepository
    {
        private readonly AppDbContext _context;

        public ReferenceTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReferenceType>> GetAllAsync() => await _context.ReferenceTypes.ToListAsync();

        public async Task<ReferenceType?> GetByIdAsync(int id) => await _context.ReferenceTypes.FindAsync(id);

        public async Task AjouterAsync(ReferenceType referenceType)
        {
            _context.ReferenceTypes.Add(referenceType);
            await _context.SaveChangesAsync();
        }

        public async Task ModifierAsync(ReferenceType referenceType)
        {
            var referenceTypeExistant = await _context.ReferenceTypes.FindAsync(referenceType.Id);
            _context.Entry(referenceTypeExistant!).CurrentValues.SetValues(referenceType);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EstUtiliseAsync(int id)
        {
            bool utiliseParUneUnite = await _context.Unites.AnyAsync(unite => unite.ReferenceTypeId == id);
            bool utiliseParUnUtilisateur = await _context.Utilisateurs.AnyAsync(utilisateur => utilisateur.ReferenceTypeId == id);
            return utiliseParUneUnite || utiliseParUnUtilisateur;
        }

        public async Task SupprimerAsync(int id)
        {
            var referenceType = await _context.ReferenceTypes.FindAsync(id);
            if (referenceType != null)
            {
                _context.ReferenceTypes.Remove(referenceType);
                await _context.SaveChangesAsync();
            }
        }
    }
}
