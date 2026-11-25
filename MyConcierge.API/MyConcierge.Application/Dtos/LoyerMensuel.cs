namespace MyConcierge.Application.Dtos
{
    // Le loyer d'un mois pour un contrat : combien est dû, combien est payé, et le statut.
    // Statut : "Payé", "Partiel", "En retard" ou "À venir".
    public class LoyerMensuel
    {
        public DateTime Mois { get; set; } // le 1er du mois
        public decimal MontantDu { get; set; }
        public decimal MontantPaye { get; set; }
        public string Statut { get; set; } = "";
    }
}
