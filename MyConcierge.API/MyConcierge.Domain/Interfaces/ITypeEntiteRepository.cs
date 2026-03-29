using MyConcierge.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConcierge.Domain.Interfaces
{
    public interface ITypeEntiteRepository
    {
        Task<List<TypeEntite>> GetAllAsync();
        Task<List<TypeEntite>> GetByCategorieAsync(CategorieName categorie);
        Task<TypeEntite?> GetByIdAsync(int id);
        Task AjouterAsync(TypeEntite typeEntite);
        Task SupprimerAsync(int id);
    }
}
