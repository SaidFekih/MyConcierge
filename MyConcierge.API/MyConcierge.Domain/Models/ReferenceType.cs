using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConcierge.Domain.Models
{
    public class ReferenceType
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty; // Ex: Appartement, Locataire
        public string Categorie { get; set; } = string.Empty; // Ex: "Unite" ou "Utilisateur"
    }
}
