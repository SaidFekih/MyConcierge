using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyConcierge.Domain.Models
{
    public class ContratsLocation
    {
        [Key]
        public int Id { get; set; }

        // Relation avec Utilisateur (Locataire)
        [ForeignKey("Locataire")]
        public int LocataireId { get; set; }
        public Utilisateur? Locataire { get; set; }

        // Relation avec Unite (Appartement ou Chambre)
        [ForeignKey("Unite")]
        public int UniteId { get; set; }
        public Unite? Unite { get; set; }

        [Required]
        public DateTime DateDebut { get; set; }

        public DateTime? DateFin { get; set; } // Peut être null pour un contrat en cours

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Montant { get; set; }
    }
}
