using Microsoft.EntityFrameworkCore;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyConcierge.Infrastructure.Repositories
{
    public class PaiementRepository : IPaiementRepository
    {
        private readonly AppDbContext _context;

        public PaiementRepository(AppDbContext context)
        {
            _context = context;
        }

        // Le plus récent en premier
        public async Task<List<Paiement>> GetAllAsync()
        {
            return await _context.Paiements
                .AsNoTracking()
                .Where(paiement => !paiement.EstSupprime)
                .OrderByDescending(paiement => paiement.DatePaiement)
                .ToListAsync();
        }

        public async Task<Paiement?> GetByIdAsync(int id)
        {
            return await _context.Paiements
                .AsNoTracking()
                .FirstOrDefaultAsync(paiement => paiement.Id == id && !paiement.EstSupprime);
        }

        public async Task<List<Paiement>> GetByContratIdAsync(int contratId)
        {
            return await _context.Paiements
                .AsNoTracking()
                .Where(paiement => paiement.ContratsLocationId == contratId && !paiement.EstSupprime)
                .OrderByDescending(paiement => paiement.DatePaiement)
                .ToListAsync();
        }

        public async Task AjouterAsync(Paiement paiement)
        {
            _context.Paiements.Add(paiement);
            await _context.SaveChangesAsync();
        }

        public async Task SupprimerAsync(int id)
        {
            var paiement = await _context.Paiements.FindAsync(id);
            if (paiement != null)
            {
                paiement.EstSupprime = true;
                paiement.DateSuppression = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}
