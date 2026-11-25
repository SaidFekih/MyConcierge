using MyConcierge.Application.Dtos;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;

namespace MyConcierge.Application.Services
{
    // Règles du MVP :
    // 1. Un paiement a un montant plus grand que zéro, une date, un mois couvert et un contrat existant.
    // 2. Le mois couvert est entre le mois de début et le mois de fin du contrat.
    // 3. Un loyer complet est dû pour chaque mois civil, du mois de début du contrat
    //    jusqu'au mois en cours (ou jusqu'au mois de fin si le contrat est terminé).
    // 4. Statut d'un mois : Payé (tout payé), Partiel (payé en partie),
    //    En retard (rien payé, mois déjà commencé), À venir (rien payé, mois pas encore commencé).
    // 5. Solde d'un contrat = ce qui manque sur les mois déjà commencés.
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

            if (paiement.MoisCouvert == default)
                return "Le mois couvert est requis.";

            var contrat = await _contrats.GetByIdAsync(paiement.ContratsLocationId);
            if (contrat == null)
                return "Contrat introuvable.";

            // On garde seulement l'année et le mois : toujours le 1er du mois
            paiement.MoisCouvert = PremierJourDuMois(paiement.MoisCouvert);

            if (paiement.MoisCouvert < PremierJourDuMois(contrat.DateDebut))
                return "Ce mois est avant le début du contrat.";

            if (contrat.DateFin != null && paiement.MoisCouvert > PremierJourDuMois(contrat.DateFin.Value))
                return "Ce mois est après la fin du contrat.";

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
            var moisCourant = PremierJourDuMois(DateTime.Today);

            var soldes = new List<SoldeContrat>();
            foreach (var contrat in contrats)
            {
                var paiementsDuContrat = paiements
                    .Where(paiement => paiement.ContratsLocationId == contrat.Id)
                    .ToList();

                var loyers = CalculerLoyersParMois(contrat, paiementsDuContrat);

                // Seuls les mois déjà commencés comptent dans le solde
                var moisCommences = loyers.Where(loyer => loyer.Mois <= moisCourant).ToList();

                soldes.Add(new SoldeContrat
                {
                    ContratId = contrat.Id,
                    LoyerDu = moisCommences.Sum(loyer => loyer.MontantDu),
                    TotalPaye = paiementsDuContrat.Sum(paiement => paiement.Montant),
                    Solde = moisCommences.Sum(loyer => Math.Max(0, loyer.MontantDu - loyer.MontantPaye)),
                    Loyers = loyers
                });
            }

            return soldes;
        }

        // Un loyer par mois, du mois de début du contrat jusqu'à son dernier mois.
        // Sans date de fin : jusqu'au mois en cours (ou jusqu'au dernier mois payé d'avance).
        private static List<LoyerMensuel> CalculerLoyersParMois(ContratsLocation contrat, List<Paiement> paiementsDuContrat)
        {
            var moisCourant = PremierJourDuMois(DateTime.Today);
            var premierMois = PremierJourDuMois(contrat.DateDebut);

            DateTime dernierMois;
            if (contrat.DateFin != null)
            {
                dernierMois = PremierJourDuMois(contrat.DateFin.Value);
            }
            else
            {
                dernierMois = moisCourant;
                if (paiementsDuContrat.Any() && paiementsDuContrat.Max(paiement => paiement.MoisCouvert) > dernierMois)
                    dernierMois = paiementsDuContrat.Max(paiement => paiement.MoisCouvert); // mois payé d'avance
            }

            if (dernierMois < premierMois)
                dernierMois = premierMois; // contrat à venir : on affiche au moins son premier mois

            var loyers = new List<LoyerMensuel>();
            for (var mois = premierMois; mois <= dernierMois; mois = mois.AddMonths(1))
            {
                var montantPaye = paiementsDuContrat
                    .Where(paiement => paiement.MoisCouvert == mois)
                    .Sum(paiement => paiement.Montant);

                loyers.Add(new LoyerMensuel
                {
                    Mois = mois,
                    MontantDu = contrat.Montant,
                    MontantPaye = montantPaye,
                    Statut = CalculerStatut(mois, moisCourant, contrat.Montant, montantPaye)
                });
            }

            return loyers;
        }

        private static string CalculerStatut(DateTime mois, DateTime moisCourant, decimal montantDu, decimal montantPaye)
        {
            if (montantPaye >= montantDu) return "Payé";
            if (montantPaye > 0) return "Partiel";
            return mois <= moisCourant ? "En retard" : "À venir";
        }

        // Le 1er du mois d'une date (ex : 2026-11-24 → 2026-11-01)
        private static DateTime PremierJourDuMois(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }
    }
}
