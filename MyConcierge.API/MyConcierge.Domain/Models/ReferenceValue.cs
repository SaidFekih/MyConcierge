using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConcierge.Domain.Models
{
    public class ReferenceValue
    {
        public int Id { get; set; }
        public int ReferenceListId { get; set; }
        public ReferenceList? ReferenceList { get; set; }     // Ex: "APARTMENT", "TENANT"
        public string Code { get; set; } = string.Empty;      // Ex: "Appartement", "Locataire"
        public string Label { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;

    }
}
