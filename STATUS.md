# MyConcierge — État du projet

_Dernière mise à jour : 2026-11-21_

## Objectif MVP
Créer / lire / modifier / supprimer des unités, des utilisateurs (locataires, propriétaires) et des contrats de location. Sans authentification ni paiements pour l'instant.

## Fait
- Backend .NET 8, 4 couches (Domain / Application / Infrastructure / Presentation), EF Core, SQL Server
- Entités : Utilisateur, Unite, ContratsLocation, ReferenceType
- CRUD complet (GET, GET par id, POST, PUT, DELETE) : unités, utilisateurs, contrats
- ReferenceType : PUT / DELETE (suppression refusée si le type est utilisé)
- Suppression logique (EstSupprime / DateSuppression) pour les contrats, unités et utilisateurs, avec règles de refus
- Frontend : liste, ajout, modification et suppression pour les utilisateurs, les contrats et les types
- Écran Unités : recherche, filtres (type, propriétaire), nouvelle unité, panneau latéral (détails, loyer cible / actuel / écart, contrat en cours, historique), modifier / dupliquer / supprimer l'unité, terminer un contrat, ajouter un contrat

## Décisions
- On garde `ReferenceType` pour le MVP.
- Les tables `ReferenceLists` / `ReferenceValues` existent en base mais ne sont pas utilisées : on les ignore pour l'instant.
- Règles des contrats (`ContratsLocationService`) : pas de chevauchement sur une même unité, DateFin >= DateDebut, unité et locataire doivent exister, `EstLouee` recalculé à chaque ajout / modification / suppression de contrat
- Limite connue : `EstLouee` (en base) n'est pas recalculé tout seul quand une date passe (seulement quand un contrat change). L'écran Unités n'en dépend pas : il calcule Louée / Vacante avec les dates des contrats.
- Les contrats, unités et utilisateurs ne sont jamais effacés : ils sont marqués EstSupprime. Les anciens contrats gardent le locataire et l'unité, mais ne sont plus modifiables si l'un d'eux est supprimé.
- Suppression refusée : une unité avec un contrat en cours ou à venir ; un utilisateur propriétaire d'unités ou locataire d'un contrat en cours ou à venir ; un type utilisé.
- Frontend Vue 3 + Quasar (MyConcierge.Web) : onglets Unités, Utilisateurs, Contrats et Types. Les erreurs de l'API s'affichent en notification, les textes de Quasar sont en français. Sur la page Unités, la modification et la suppression se font dans le panneau latéral de l'unité.

## Prochaine étape
Phase B : Renouveler / Ajuster le loyer d'une unité (dans le panneau latéral)

## Ensuite (une à la fois)
1. Phase C : paiements (table, migration, règles), onglet Paiements & Soldes et statut de paiement réel
2. Même panneau latéral pour les utilisateurs (locataire : contrats et unités ; propriétaire : unités)
3. Vrais tests unitaires (le projet Tests est vide)
4. Notes, maintenance, tableau de bord (taux d'occupation, contrats qui se terminent bientôt)
5. Authentification
6. Décider du sort de `ReferenceLists` / `ReferenceValues`

## Pour reprendre
- API : lancer `MyConcierge.Presentation` (F5) : Swagger s'ouvre à la racine (https://localhost:7216).
- Frontend : `cd MyConcierge.Web`, `npm install` (la première fois), `npm run dev`, puis ouvrir http://localhost:5173 (l'API doit tourner).