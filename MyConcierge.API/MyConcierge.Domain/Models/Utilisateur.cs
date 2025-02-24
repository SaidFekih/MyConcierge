using System;


namespace MyConcierge.Domain.Models
{
    public class Utilisateur
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;

        // Clé étrangère vers ReferenceTypes (ex: Locataire, Propriétaire, Concierge)
        public int ReferenceTypeId { get; set; }
        public ReferenceType? ReferenceType { get; set; }
    }
}
