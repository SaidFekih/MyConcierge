using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyConcierge.Domain.Models
{
    public enum StatutUnite
    {
        Disponible,
        Louee,
        EnRenovation,
        Reservee
    }
    public class Unite
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nom { get; set; } = string.Empty;

        public int TypeEntiteId { get; set; }
        public TypeEntite? TypeEntite { get; set; }

        public int? ParentUniteId { get; set; }
        public Unite? ParentUnite { get; set; }

        public int ProprietaireId { get; set; }
        public Utilisateur? Proprietaire { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MontantLoyer { get; set; }
        public StatutUnite Statut { get; set; }

    }
}
