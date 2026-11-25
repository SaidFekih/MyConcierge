using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyConcierge.Domain.Models
{
    // Un paiement de loyer reçu pour un contrat.
    // Chaque paiement est rattaché à un mois (MoisCouvert) : le loyer de ce mois-là.
    public class Paiement
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ContratsLocation")]
        public int ContratsLocationId { get; set; }
        public ContratsLocation? ContratsLocation { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Montant { get; set; }

        // Le jour où l'argent a été reçu
        [Required]
        public DateTime DatePaiement { get; set; }

        // Le mois payé, toujours le 1er du mois (ex : 2026-11-01 pour novembre 2026)
        [Required]
        public DateTime MoisCouvert { get; set; }

        public bool EstSupprime { get; set; }

        public DateTime? DateSuppression { get; set; }
    }
}
