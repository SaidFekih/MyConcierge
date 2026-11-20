# MyConcierge — État du projet

_Dernière mise à jour : 2026-11-20_

## Objectif MVP
Créer / lire / modifier / supprimer des unités, des utilisateurs (locataires, propriétaires) et des contrats de location. Sans authentification ni paiements pour l'instant.

## Fait
- Backend .NET 8, 4 couches (Domain / Application / Infrastructure / Presentation), EF Core, SQL Server
- Entités : Utilisateur, Unite, ContratsLocation, ReferenceType
- CRUD complet (GET, GET par id, POST, PUT, DELETE) : unités, utilisateurs, contrats

## Décisions
- On garde `ReferenceType` pour le MVP.
- Les tables `ReferenceLists` / `ReferenceValues` existent en base mais ne sont pas utilisées : on les ignore pour l'instant.

- Règles des contrats (`ContratsLocationService`) : pas de chevauchement sur une même unité, DateFin >= DateDebut, unité et locataire doivent exister, `EstLouee` recalculé à chaque ajout / modification / suppression de contrat
- Limite connue : `EstLouee` n'est pas recalculé tout seul quand une date passe (seulement quand un contrat change)

## Prochaine étape
Tester les règles dans Swagger, puis commencer le frontend Vue/Quasar minimal.

## Ensuite (une à la fois)
1. Frontend Vue/Quasar minimal (liste + formulaire pour unités et contrats)
2. Contrôleur pour `ReferenceType` : PUT / DELETE
3. Vrais tests unitaires (le projet Tests est vide)
4. Authentification
5. Décider du sort de `ReferenceLists` / `ReferenceValues`

## Pour reprendre
`dotnet build` puis lancer `MyConcierge.Presentation` : Swagger s'ouvre à la racine.
