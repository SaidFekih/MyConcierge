using MyConcierge.Domain.Interfaces;

namespace MyConcierge.Application.Services
{
    public class UtilisateurService
    {
        private readonly IUtilisateurRepository _utilisateurs;
        private readonly IUniteRepository _unites;
        private readonly IContratsLocationRepository _contrats;

        public UtilisateurService(
            IUtilisateurRepository utilisateurs,
            IUniteRepository unites,
            IContratsLocationRepository contrats)
        {
            _utilisateurs = utilisateurs;
            _unites = unites;
            _contrats = contrats;
        }

        // Retourne un message d'erreur, ou null si la suppression a réussi
        public async Task<string?> SupprimerAsync(int id)
        {
            var unites = await _unites.GetAllAsync();
            if (unites.Any(unite => unite.ProprietaireId == id))
                return "Cet utilisateur est propriétaire d'unités : impossible de le supprimer.";

            var contrats = await _contrats.GetAllAsync();
            var aujourdhui = DateTime.Today;

            bool aUnContratEnCoursOuAVenir = contrats.Any(contrat =>
                contrat.LocataireId == id && (contrat.DateFin == null || contrat.DateFin >= aujourdhui));

            if (aUnContratEnCoursOuAVenir)
                return "Cet utilisateur a un contrat en cours ou à venir : impossible de le supprimer.";

            await _utilisateurs.SupprimerAsync(id);
            return null;
        }
    }
}