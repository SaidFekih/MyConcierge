using MyConcierge.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConcierge.Domain.Interfaces
{
    public interface IReferenceTypeRepository
    {
        Task<List<ReferenceType>> GetAllAsync();
        Task<ReferenceType?> GetByIdAsync(int id);
        Task AjouterAsync(ReferenceType referenceType);
        Task SupprimerAsync(int id);
    }
}
