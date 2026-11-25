using System;
using System.ComponentModel.DataAnnotations;

namespace MyConcierge.Domain.Models
{
    // Un compte pour se connecter à MyConcierge (toi et les gestionnaires).
    // Le mot de passe n'est jamais gardé en clair : seulement son hachage.
    public class Compte
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;

        // Toujours en minuscules, un seul compte par courriel
        [Required]
        [MaxLength(256)]
        public string Courriel { get; set; } = string.Empty;

        [Required]
        public string MotDePasseHache { get; set; } = string.Empty;

        // Un compte désactivé ne peut plus se connecter (on ne supprime pas les comptes)
        public bool EstActif { get; set; } = true;

        public DateTime DateCreation { get; set; }
    }
}
