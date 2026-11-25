# MyConcierge — État du projet

_Dernière mise à jour : 2026-11-24_

## Objectif MVP
Créer / lire / modifier / supprimer des unités, des utilisateurs (locataires, propriétaires) et des contrats de location. Sans authentification ni paiements pour l'instant.

## Fait
- Backend .NET 8, 4 couches (Domain / Application / Infrastructure / Presentation), EF Core, SQL Server
- Entités : Utilisateur, Unite, ContratsLocation, ReferenceType
- CRUD complet (GET, GET par id, POST, PUT, DELETE) : unités, utilisateurs, contrats, types
- ReferenceType : PUT / DELETE (suppression refusée si le type est utilisé)
- Suppression logique (EstSupprime / DateSuppression) pour les contrats, unités et utilisateurs, avec règles de refus
- Frontend : liste, ajout, modification et suppression pour les utilisateurs, les contrats et les types
- Écran Unités : recherche, filtres (type, propriétaire), statut Louée / Vacante, panneau latéral (détails, modifier, dupliquer, supprimer, contrat en cours, terminer le contrat, ajouter un contrat, historique)
- Panneau Unités, phase B : renouveler le contrat en cours (nouveau contrat le lendemain de la fin) et ajuster le loyer (loyer cible de l'unité et montant du contrat en cours)

## Décisions
- On garde `ReferenceType` pour le MVP.
- Les tables `ReferenceLists` / `ReferenceValues` existent en base mais ne sont pas utilisées : on les ignore pour l'instant.
- Suppression logique plutôt que suppression réelle (la suppression en cascade d'EF Core était dangereuse). Les contrats, unités et utilisateurs ne sont jamais effacés : ils sont marqués EstSupprime. Les anciens contrats gardent le locataire et l'unité, mais ne sont plus modifiables si l'un d'eux est supprimé.
- Refus de suppression : unité avec contrat en cours ou à venir ; utilisateur propriétaire d'unités ou locataire d'un contrat en cours ou à venir ; type utilisé.
- Règles des contrats (`ContratsLocationService`) : pas de chevauchement sur une même unité, DateFin >= DateDebut, unité et locataire doivent exister, `EstLouee` recalculé à chaque ajout / modification / suppression de contrat.
- Limite connue : `EstLouee` n'est pas recalculé tout seul quand une date passe (seulement quand un contrat change). Le statut affiché dans le frontend est calculé à partir des contrats.
- Ajuster le montant d'un contrat en cours le change pour toute sa durée (mois passés compris). Pour garder l'historique des loyers, on renouvelle au lieu d'ajuster. À revoir avec les paiements.
- Renouveler : seulement si le contrat en cours a une date de fin et qu'aucun contrat à venir n'existe.
- Frontend Vue 3 + Quasar en français (MyConcierge.Web) : onglets Unités, Utilisateurs, Contrats et Types ; les erreurs de l'API s'affichent en notification.

## Prochaine étape
Phase C : paiements (table, migration, onglet Paiements et soldes, vrai statut de paiement)

## Ensuite (une à la fois)
1. Même panneau latéral pour les utilisateurs
2. Vrais tests unitaires (le projet Tests est vide)
3. Notes, maintenance, tableau de bord
4. Authentification
5. Décider du sort de `ReferenceLists` / `ReferenceValues`

## Pour reprendre
- API : lancer `MyConcierge.Presentation` (F5) : Swagger s'ouvre à la racine (https://localhost:7216).
- Frontend : `cd MyConcierge.Web`, `npm install` (la première fois), `npm run dev`, puis ouvrir http://localhost:5173 (l'API doit tourner).