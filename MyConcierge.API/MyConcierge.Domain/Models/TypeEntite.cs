using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConcierge.Domain.Models
{


    public enum CategorieName
    {
        Unite,
        Utilisateur,
        Contrat
    }
    public class TypeEntite
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public CategorieName Categorie { get; set; }
        public string? Description { get; set; }
    }
}
