using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;

namespace MyConcierge.Application.Services
{
    public class TypeEntiteService
    {
        private readonly ITypeEntiteRepository _repository;

        public TypeEntiteService(ITypeEntiteRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TypeEntite>> ObtenirTousAsync() => await _repository.GetAllAsync();

        public async Task<TypeEntite?> ObtenirParIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task AjouterAsync(TypeEntite typeEntite) => await _repository.AjouterAsync(typeEntite);

        public async Task SupprimerAsync(int id) => await _repository.SupprimerAsync(id);
    }
}
