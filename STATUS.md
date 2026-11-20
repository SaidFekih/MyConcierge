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

## Prochaine étape
Valider les règles de base : un contrat ne chevauche pas un autre sur la même unité, et `EstLouee` se met à jour selon les contrats.

## Ensuite (une à la fois)
1. Frontend Vue/Quasar minimal (liste + formulaire pour unités et contrats)
2. Contrôleur pour `ReferenceType` : PUT / DELETE
3. Vrais tests unitaires (le projet Tests est vide)
4. Authentification
5. Décider du sort de `ReferenceLists` / `ReferenceValues`

## Pour reprendre
`dotnet build` puis lancer `MyConcierge.Presentation` : Swagger s'ouvre à la racine.
