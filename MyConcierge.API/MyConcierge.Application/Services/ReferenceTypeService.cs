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

        public async Task SupprimerAsync(int id) => await _repository.SupprimerAsync(id);
    }
}
