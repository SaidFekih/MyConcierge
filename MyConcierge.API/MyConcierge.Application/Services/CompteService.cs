using Microsoft.AspNetCore.Identity;
using MyConcierge.Domain.Interfaces;
using MyConcierge.Domain.Models;

namespace MyConcierge.Application.Services
{
    // Règles des comptes :
    // 1. Un nom, un courriel valide et un mot de passe d'au moins 10 caractères.
    // 2. Un seul compte par courriel (le courriel est gardé en minuscules).
    // 3. Un compte désactivé ne peut pas se connecter.
    // 4. On ne peut pas désactiver son propre compte (il reste toujours un compte actif).
    public class CompteService
    {
        private const int LongueurMinimaleDuMotDePasse = 10;

        private readonly ICompteRepository _comptes;

        // Le même hachage de mot de passe que celui d'ASP.NET Core Identity
        private readonly PasswordHasher<Compte> _hacheur = new PasswordHasher<Compte>();

        public CompteService(ICompteRepository comptes)
        {
            _comptes = comptes;
        }

        // Retourne le compte si le courriel et le mot de passe sont bons, sinon null.
        public async Task<Compte?> VerifierConnexionAsync(string courriel, string motDePasse)
        {
            var compte = await _comptes.GetByCourrielAsync(NormaliserCourriel(courriel));
            if (compte == null || !compte.EstActif)
                return null;

            var resultat = _hacheur.VerifyHashedPassword(compte, compte.MotDePasseHache, motDePasse);
            return resultat == PasswordVerificationResult.Failed ? null : compte;
        }

        // Retourne un message d'erreur, ou null si le compte est créé.
        public async Task<string?> CreerCompteAsync(string nom, string courriel, string motDePasse)
        {
            if (string.IsNullOrWhiteSpace(nom))
                return "Le nom est requis.";

            courriel = NormaliserCourriel(courriel);
            if (!courriel.Contains('@') || courriel.Length > 256)
                return "Le courriel est invalide.";

            if (string.IsNullOrEmpty(motDePasse) || motDePasse.Length < LongueurMinimaleDuMotDePasse)
                return "Le mot de passe doit contenir au moins " + LongueurMinimaleDuMotDePasse + " caractères.";

            if (await _comptes.GetByCourrielAsync(courriel) != null)
                return "Un compte existe déjà avec ce courriel.";

            var compte = new Compte
            {
                Nom = nom.Trim(),
                Courriel = courriel,
                EstActif = true,
                DateCreation = DateTime.UtcNow
            };
            compte.MotDePasseHache = _hacheur.HashPassword(compte, motDePasse);

            await _comptes.AjouterAsync(compte);
            return null;
        }

        // Au démarrage de l'API : crée le premier compte s'il n'y a encore aucun compte.
        // Retourne null si tout est en ordre (compte créé, ou des comptes existent déjà),
        // sinon la raison pour laquelle le compte n'a pas pu être créé.
        public async Task<string?> CreerCompteInitialSiNecessaireAsync(string? nom, string? courriel, string? motDePasse)
        {
            if (await _comptes.ExisteAuMoinsUnAsync())
                return null;

            return await CreerCompteAsync(nom ?? "", courriel ?? "", motDePasse ?? "");
        }

        public async Task<List<Compte>> ListerAsync()
        {
            return await _comptes.GetAllAsync();
        }

        // Active ou désactive un compte. Retourne un message d'erreur, ou null si tout est correct.
        public async Task<string?> ChangerEstActifAsync(int id, bool estActif, int idDuCompteConnecte)
        {
            if (!estActif && id == idDuCompteConnecte)
                return "Impossible de désactiver son propre compte.";

            if (await _comptes.GetByIdAsync(id) == null)
                return "Compte introuvable.";

            await _comptes.ChangerEstActifAsync(id, estActif);
            return null;
        }

        private static string NormaliserCourriel(string courriel)
        {
            return (courriel ?? "").Trim().ToLowerInvariant();
        }
    }
}
