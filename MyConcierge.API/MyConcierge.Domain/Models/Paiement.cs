using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyConcierge.Domain.Models
{
    // Un paiement de loyer reçu pour un contrat.
    // Le paiement n'est pas rattaché à un mois : on compare le total payé au total dû.
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

        [Required]
        public DateTime DatePaiement { get; set; }

        public bool EstSupprime { get; set; }

        public DateTime? DateSuppression { get; set; }
    }
}
