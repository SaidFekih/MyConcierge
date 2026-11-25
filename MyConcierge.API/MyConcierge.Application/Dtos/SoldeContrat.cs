namespace MyConcierge.Application.Dtos
{
    // Le solde d'un contrat.
    // LoyerDu : les loyers des mois déjà commencés. Solde : ce qui manque sur ces mois.
    // Solde > 0 : le locataire est en retard. Solde = 0 : tout est payé.
    // Loyers : le détail mois par mois.
    public class SoldeContrat
    {
        public int ContratId { get; set; }
        public decimal LoyerDu { get; set; }
        public decimal TotalPaye { get; set; }
        public decimal Solde { get; set; }
        public List<LoyerMensuel> Loyers { get; set; } = new();
    }
}
