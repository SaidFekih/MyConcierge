using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConcierge.Application.Services
{
    public class ReferenceTypeService
    {
        private readonly IReferenceTypeRepository _repository;

        public ReferenceTypeService(IReferenceTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ReferenceType>> ObtenirTousAsync() => await _repository.GetAllAsync();

        public async Task<ReferenceType?> ObtenirParIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task AjouterAsync(ReferenceType referenceType) => await _repository.AjouterAsync(referenceType);

        public async Task ModifierAsync(ReferenceType referenceType) => await _repository.ModifierAsync(referenceType);

        // Retourne un message d'erreur, ou null si la suppression a réussi
        public async Task<string?> SupprimerAsync(int id)
        {
            if (await _repository.EstUtiliseAsync(id))
                return "Ce type est utilisé par des unités ou des utilisateurs : impossible de le supprimer.";

            await _repository.SupprimerAsync(id);
            return null;
        }

    }
}
