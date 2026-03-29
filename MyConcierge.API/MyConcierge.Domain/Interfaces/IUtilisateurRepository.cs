using Microsoft.AspNetCore.JsonPatch;
using MyConcierge.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConcierge.Domain.Interfaces
{
    public interface IUtilisateurRepository
    {
        Task<List<Utilisateur>> GetAllAsync();
        Task<Utilisateur?> GetByIdAsync(int id);
        Task AjouterAsync(Utilisateur utilisateur);

        Task ModifierAsync(Utilisateur utilisateur);
        Task SupprimerAsync(int id);
        Task ModifierUtilisateurlAsync(int id, JsonPatchDocument<Utilisateur> patch);

    }
}
