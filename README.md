
# 🏠 MyConcierge - Gestion de Locations Immobilières

## 📌 Introduction
**MyConcierge** est une solution complète pour la gestion de locations immobilières.  
Elle permet aux **propriétaires** de gérer leurs unités locatives (**appartements, chambres, immeubles**), de suivre les **contrats de location** et de gérer les **locataires** de manière efficace.

Ce projet est conçu avec une architecture **scalable, modulaire et maintenable**, en suivant les bonnes pratiques **.NET et React**.

---

## 🏗 **Architecture de l'API**
L'API REST de **MyConcierge** suit une architecture **Clean Architecture** et **Domain-Driven Design (DDD)** pour assurer **séparation des préoccupations** et **maintenabilité**.

### 🔹 **Organisation du code**
📂 **MyConcierge.API** *(Backend .NET 8)*

/MyConcierge.API 
	├── /MyConcierge.Application # ➜ Services applicatifs (business logic) 
	├── /MyConcierge.Domain # ➜ Modèles et interfaces (core business) 
	├── /MyConcierge.Infrastructure # ➜ Accès aux données (EF Core, Repositories) 
	├── /MyConcierge.Presentation # ➜ API Controllers (ASP.NET Core Web API) 
	├── /MyConcierge.Tests # ➜ Tests unitaires et d'intégration 
	├── MyConcierge.sln # ➜ Solution Visual Studio
	
	
### 🔹 **Explication du choix d’architecture**
✅ **Séparation des responsabilités** : Chaque couche a un rôle distinct  
✅ **Facilité d'évolution** : Ajout de nouvelles fonctionnalités sans impacter l’existant  
✅ **Respect des principes SOLID** : Code plus propre et maintenable  
✅ **Utilisation d’Entity Framework Core** pour la gestion des données  

---

## 🛢 **Modèle de la Base de Données**
L'architecture de la **base de données** suit une **approche relationnelle avec SQL Server**, et les entités sont conçues pour **éviter la redondance et maximiser la flexibilité**.

### 🔹 **Schéma Simplifié**
📂 **MyConcierge.Database**
+------------------+ +---------------+ +----------------------+ 
| Utilisateurs | | Unites | | ContratsLocations | 
+------------------+ +---------------+ +----------------------+ 
| Id (PK) |
	◀─── | ProprietaireId | 
	◀── | LocataireId (FK) | | Nom | | ReferenceType | | UniteId (FK) | | Email | | ParentUniteId | | DateDebut | | Telephone | | Prix | | DateFin | | ReferenceTypeId | | EstLouee | | Montant | +------------------+ +---------------+ +----------------------+
	

### 🔹 **Explication des choix**
✅ **Relation Propriétaire → Unité → Contrat** : Structure logique et évolutive  
✅ **Flexibilité** : Un **propriétaire** peut louer **un appartement entier** ou **des chambres**  
✅ **Table `ReferenceTypes`** pour éviter la duplication des types (`Appartement`, `Chambre`, `Locataire`, etc.)  

---

## 🎨 **Frontend (React.js)**
📂 **MyConcierge.Web** *(Frontend React)*
