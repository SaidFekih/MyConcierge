# MyConcierge — État du projet

_Dernière mise à jour : 2026-11-21_

## Objectif MVP
Créer / lire / modifier / supprimer des unités, des utilisateurs (locataires, propriétaires) et des contrats de location. Sans authentification ni paiements pour l'instant.

## Fait
- Backend .NET 8, 4 couches (Domain / Application / Infrastructure / Presentation), EF Core, SQL Server
- Entités : Utilisateur, Unite, ContratsLocation, ReferenceType
- CRUD complet (GET, GET par id, POST, PUT, DELETE) : unités, utilisateurs, contrats
- Frontend : liste, ajout, modification et suppression pour les unités, les utilisateurs et les contrats
- ReferenceType : PUT / DELETE (suppression refusée si le type est utilisé)
et change la ligne Frontend en « …pour les unités, les utilisateurs, les contrats et les types »

## Décisions
- On garde `ReferenceType` pour le MVP.
- Les tables `ReferenceLists` / `ReferenceValues` existent en base mais ne sont pas utilisées : on les ignore pour l'instant.

- Règles des contrats (`ContratsLocationService`) : pas de chevauchement sur une même unité, DateFin >= DateDebut, unité et locataire doivent exister, `EstLouee` recalculé à chaque ajout / modification / suppression de contrat
- Limite connue : `EstLouee` n'est pas recalculé tout seul quand une date passe (seulement quand un contrat change)

- Frontend Vue 3 + Quasar minimal (MyConcierge.Web) : onglets Unités, Utilisateurs, Contrats et Types (liste, ajout, modification, suppression ; les erreurs de l'API s'affichent en notification)
- Les clés étrangères sont en suppression en cascade : supprimer un propriétaire supprime ses unités et leurs contrats (à revoir)
## Prochaine étape
Vrais tests unitaires (le projet Tests est vide)

## Ensuite (une à la fois)
1. Authentification
2. Décider du sort de `ReferenceLists` / `ReferenceValues`

## Pour reprendre
- API : lancer `MyConcierge.Presentation` (F5) : Swagger s'ouvre à la racine (https://localhost:7216).
- Frontend : `cd MyConcierge.Web`, `npm install` (la première fois), `npm run dev`, puis ouvrir http://localhost:5173 (l'API doit tourner).
