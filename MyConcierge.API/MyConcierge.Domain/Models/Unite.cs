using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyConcierge.Domain.Models
{
    public class Unite
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nom { get; set; } = string.Empty;

        [ForeignKey("ReferenceType")]
        public int ReferenceTypeId { get; set; }
        public ReferenceType? ReferenceType { get; set; }

        [ForeignKey("ParentUnite")]
        public int? ParentUniteId { get; set; }
        public Unite? ParentUnite { get; set; }

        [ForeignKey("Proprietaire")]
        public int ProprietaireId { get; set; }
        public Utilisateur? Proprietaire { get; set; }

        public decimal Prix { get; set; }
        public bool EstLouee { get; set; }
    }
}
