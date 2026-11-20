using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;

namespace MyConcierge.Application.Services
{
    // Règles du MVP :
    // 1. Un contrat ne peut pas chevaucher un autre contrat de la même unité.
    // 2. Un contrat sans DateFin est un contrat en cours (sans fin).
    // 3. Unite.EstLouee est recalculé à chaque ajout, modification ou suppression de contrat.
    public class ContratsLocationService
    {
        private readonly IContratsLocationRepository _contrats;
        private readonly IUniteRepository _unites;
        private readonly IUtilisateurRepository _utilisateurs;

        public ContratsLocationService(
            IContratsLocationRepository contrats,
            IUniteRepository unites,
            IUtilisateurRepository utilisateurs)
        {
            _contrats = contrats;
            _unites = unites;
            _utilisateurs = utilisateurs;
        }

        // Les méthodes Ajouter/Modifier retournent un message d'erreur, ou null si tout est correct.
        public async Task<string?> AjouterAsync(ContratsLocation contrat)
        {
            var erreur = await ValiderAsync(contrat);
            if (erreur != null) return erreur;

            await _contrats.AjouterAsync(contrat);
            await MettreAJourEstLoueeAsync(contrat.UniteId);
            return null;
        }

        public async Task<string?> ModifierAsync(ContratsLocation contrat)
        {
            var ancien = await _contrats.GetByIdAsync(contrat.Id);
            if (ancien == null) return "Contrat introuvable.";

            var erreur = await ValiderAsync(contrat);
            if (erreur != null) return erreur;

            await _contrats.ModifierAsync(contrat);
            await MettreAJourEstLoueeAsync(contrat.UniteId);

            // Si le contrat a changé d'unité, l'ancienne unité doit aussi être recalculée
            if (ancien.UniteId != contrat.UniteId)
                await MettreAJourEstLoueeAsync(ancien.UniteId);

            return null;
        }

        public async Task SupprimerAsync(int id)
        {
            var contrat = await _contrats.GetByIdAsync(id);
            if (contrat == null) return;

            await _contrats.SupprimerAsync(id);
            await MettreAJourEstLoueeAsync(contrat.UniteId);
        }

        private async Task<string?> ValiderAsync(ContratsLocation contrat)
        {
            if (contrat.DateFin != null && contrat.DateFin < contrat.DateDebut)
                return "La date de fin doit être après la date de début.";

            if (await _unites.GetByIdAsync(contrat.UniteId) == null)
                return "Unité introuvable.";

            if (await _utilisateurs.GetByIdAsync(contrat.LocataireId) == null)
                return "Locataire introuvable.";

            var contratsDeLUnite = await _contrats.GetByUniteIdAsync(contrat.UniteId);
            foreach (var autre in contratsDeLUnite)
            {
                if (autre.Id == contrat.Id) continue; // en modification, on ignore le contrat lui-même

                if (SeChevauchent(contrat, autre))
                    return "Cette unité est déjà louée sur cette période.";
            }

            return null;
        }

        private static bool SeChevauchent(ContratsLocation a, ContratsLocation b)
        {
            // Pas de DateFin = contrat sans fin
            var finA = a.DateFin ?? DateTime.MaxValue;
            var finB = b.DateFin ?? DateTime.MaxValue;

            return a.DateDebut <= finB && b.DateDebut <= finA;
        }

        private async Task MettreAJourEstLoueeAsync(int uniteId)
        {
            var aujourdhui = DateTime.Today;
            var contrats = await _contrats.GetByUniteIdAsync(uniteId);

            var estLouee = contrats.Any(c =>
                c.DateDebut <= aujourdhui && (c.DateFin == null || c.DateFin >= aujourdhui));

            await _unites.DefinirEstLoueeAsync(uniteId, estLouee);
        }
    }
}
