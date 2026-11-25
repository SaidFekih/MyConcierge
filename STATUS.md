# MyConcierge — État du projet

_Dernière mise à jour : 2026-11-25_

## Objectif MVP
Créer / lire / modifier / supprimer des unités, des utilisateurs (locataires, propriétaires) et des contrats de location, et suivre les paiements de loyer. Authentification en cours de préparation.

## Fait
- Backend .NET 8, 4 couches (Domain / Application / Infrastructure / Presentation), EF Core, SQL Server
- Entités : Utilisateur, Unite, ContratsLocation, ReferenceType, Paiement
- CRUD complet (GET, GET par id, POST, PUT, DELETE) : unités, utilisateurs, contrats, types
- ReferenceType : PUT / DELETE (suppression refusée si le type est utilisé)
- Suppression logique (EstSupprime / DateSuppression) pour les contrats, unités, utilisateurs et paiements, avec règles de refus
- Frontend : liste, ajout, modification et suppression pour les utilisateurs, les contrats et les types
- Écran Unités : recherche, filtres (type, propriétaire), statut Louée / Vacante, panneau latéral (détails, modifier, dupliquer, supprimer, contrat en cours, terminer le contrat, ajouter un contrat, historique)
- Panneau Unités : renouveler le contrat en cours et ajuster le loyer (loyer cible et montant du contrat)
- Paiements : chaque paiement est rattaché à un mois (MoisCouvert). API `/api/paiements` et `/api/paiements/soldes` (un solde et un loyer par mois pour chaque contrat)
- Panneau Unités : badge « Loyer payé / en retard », tableau « Loyers par mois » (Payé / Partiel / En retard / À venir, bouton Payer), formulaire de paiement avec choix du mois, liste des paiements reçus
- Onglet « Paiements et soldes » : totaux, vue par contrat ou par locataire, filtre « seulement les retards »

## Décisions
- On garde `ReferenceType` pour le MVP.
- Les tables `ReferenceLists` / `ReferenceValues` existent en base mais ne sont pas utilisées : on les ignore pour l'instant.
- Suppression logique plutôt que suppression réelle (la suppression en cascade d'EF Core était dangereuse). Les anciens contrats gardent le locataire et l'unité, mais ne sont plus modifiables si l'un d'eux est supprimé. Clé étrangère des paiements en `Restrict`.
- Refus de suppression : unité avec contrat en cours ou à venir ; utilisateur propriétaire d'unités ou locataire d'un contrat en cours ou à venir ; type utilisé.
- Règles des contrats (`ContratsLocationService`) : pas de chevauchement sur une même unité, DateFin >= DateDebut, unité et locataire doivent exister, `EstLouee` recalculé à chaque ajout / modification / suppression de contrat.
- Limite connue : `EstLouee` n'est pas recalculé tout seul quand une date passe (seulement quand un contrat change). Le statut affiché dans le frontend est calculé à partir des contrats.
- Ajuster le montant d'un contrat en cours le change pour toute sa durée (mois passés compris). Pour garder l'historique des loyers, on renouvelle au lieu d'ajuster.
- Renouveler : seulement si le contrat en cours a une date de fin et qu'aucun contrat à venir n'existe.
- Paiements : un loyer complet est dû pour chaque mois civil, du mois de début du contrat jusqu'au mois en cours (ou jusqu'au mois de fin). Le mois couvert d'un paiement doit être entre le début et la fin du contrat. Statut d'un mois : Payé, Partiel, En retard (mois commencé, rien payé), À venir. Solde d'un contrat = ce qui manque sur les mois déjà commencés.
- Limite connue : l'écran ne propose pas de payer un mois futur d'avance (l'API l'accepte).
- Les fichiers `.vs`, `.user`, `PublishProfiles` et `.patch` ne sont pas versionnés. Les `appsettings` sont versionnés (aucun secret : connexion Windows locale).
- Frontend Vue 3 + Quasar en français (MyConcierge.Web) : onglets Unités, Utilisateurs, Contrats, Paiements et soldes, Types ; les erreurs de l'API s'affichent en notification.

## Prochaine étape
Authentification (connexion obligatoire avant tout accès à l'API et à l'écran)

## Ensuite (une à la fois)
1. Déploiement (API, base SQL Server, frontend compilé, configuration de production)
2. Même panneau latéral pour les utilisateurs
3. Vrais tests unitaires (le projet Tests est vide)
4. Notes, maintenance, tableau de bord
5. Décider du sort de `ReferenceLists` / `ReferenceValues`

## Pour reprendre
- API : lancer `MyConcierge.Presentation` (F5) : Swagger s'ouvre à la racine (https://localhost:7216).
- Frontend : `cd MyConcierge.Web`, `npm install` (la première fois), `npm run dev`, puis ouvrir http://localhost:5173 (l'API doit tourner).