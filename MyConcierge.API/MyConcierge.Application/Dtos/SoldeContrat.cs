namespace MyConcierge.Application.Dtos
{
    // Le solde d'un contrat : ce qui est dû moins ce qui est payé.
    // Solde > 0 : le locataire est en retard. Solde <= 0 : tout est payé.
    public class SoldeContrat
    {
        public int ContratId { get; set; }
        public decimal LoyerDu { get; set; }
        public decimal TotalPaye { get; set; }
        public decimal Solde { get; set; }
    }
}
