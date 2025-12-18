using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConcierge.Domain.Models
{
    public class ReferenceList
    {
        public int Id { get; set; }   // Ex: "UNIT_TYPE", "USER_ROLE"
        public string Code { get; set; } = string.Empty;   // Ex: "Types d’unités", "Rôles utilisateurs"
        public string Name { get; set; } = string.Empty;
        public ICollection<ReferenceValue> Values { get; set; } = new List<ReferenceValue>();
    }
}
