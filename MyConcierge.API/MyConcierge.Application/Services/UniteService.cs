using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyConcierge.Domain.Interfaces;

namespace MyConcierge.Application.Services
{
    public class UniteService
    {
        private readonly IUniteRepository _unites;
        private readonly IContratsLocationRepository _contrats;

        public UniteService(IUniteRepository unites, IContratsLocationRepository contrats)
        {
            _unites = unites;
            _contrats = contrats;
        }

        // Retourne un message d'erreur, ou null si la suppression a réussi
        public async Task<string?> SupprimerAsync(int id)
        {
            var contratsDeLUnite = await _contrats.GetByUniteIdAsync(id);
            var aujourdhui = DateTime.Today;

            bool aUnContratEnCoursOuAVenir = contratsDeLUnite.Any(contrat =>
                contrat.DateFin == null || contrat.DateFin >= aujourdhui);

            if (aUnContratEnCoursOuAVenir)
                return "Cette unité a un contrat en cours ou à venir : impossible de la supprimer.";

            await _unites.SupprimerAsync(id);
            return null;
        }
    }
}
