using MyConcierge.Application.Dtos;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;

namespace MyConcierge.Application.Services
{
    // Règles du MVP :
    // 1. Un paiement a un montant plus grand que zéro, une date, et un contrat existant.
    // 2. Un loyer complet est dû pour chaque mois civil, du mois de début du contrat
    //    jusqu'au mois en cours (ou jusqu'au mois de fin si le contrat est terminé).
    // 3. Solde = loyer dû - total payé. Un solde plus grand que zéro veut dire « en retard ».
    public class PaiementService
    {
        private readonly IPaiementRepository _paiements;
        private readonly IContratsLocationRepository _contrats;

        public PaiementService(IPaiementRepository paiements, IContratsLocationRepository contrats)
        {
            _paiements = paiements;
            _contrats = contrats;
        }

        // Retourne un message d'erreur, ou null si tout est correct.
        public async Task<string?> AjouterAsync(Paiement paiement)
        {
            if (paiement.Montant <= 0)
                return "Le montant du paiement doit être plus grand que zéro.";

            if (paiement.DatePaiement == default)
                return "La date du paiement est requise.";

            if (await _contrats.GetByIdAsync(paiement.ContratsLocationId) == null)
                return "Contrat introuvable.";

            await _paiements.AjouterAsync(paiement);
            return null;
        }

        public async Task SupprimerAsync(int id)
        {
            await _paiements.SupprimerAsync(id);
        }

        // Un solde par contrat (les contrats supprimés sont ignorés)
        public async Task<List<SoldeContrat>> CalculerSoldesAsync()
        {
            var contrats = await _contrats.GetAllAsync();
            var paiements = await _paiements.GetAllAsync();

            var soldes = new List<SoldeContrat>();
            foreach (var contrat in contrats)
            {
                var loyerDu = CalculerNombreDeMoisDus(contrat) * contrat.Montant;
                var totalPaye = paiements
                    .Where(paiement => paiement.ContratsLocationId == contrat.Id)
                    .Sum(paiement => paiement.Montant);

                soldes.Add(new SoldeContrat
                {
                    ContratId = contrat.Id,
                    LoyerDu = loyerDu,
                    TotalPaye = totalPaye,
                    Solde = loyerDu - totalPaye
                });
            }

            return soldes;
        }

        // Nombre de mois civils commencés depuis le début du contrat
        // (ex : début le 15 janvier, aujourd'hui le 3 mars → 3 mois : janvier, février, mars)
        private static int CalculerNombreDeMoisDus(ContratsLocation contrat)
        {
            var aujourdhui = DateTime.Today;
            if (contrat.DateDebut > aujourdhui) return 0; // contrat à venir : rien n'est encore dû

            var dernierJour = aujourdhui;
            if (contrat.DateFin != null && contrat.DateFin.Value < aujourdhui)
                dernierJour = contrat.DateFin.Value; // contrat terminé : on s'arrête à sa fin

            return (dernierJour.Year - contrat.DateDebut.Year) * 12
                   + dernierJour.Month - contrat.DateDebut.Month + 1;
        }
    }
}
