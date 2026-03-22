# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**MyConcierge** is a real estate rental management platform (French-language domain). It is a .NET 8 Web API backend following Clean Architecture / DDD principles, with a React frontend (not yet in this repo).

## Solution Structure

The solution lives under `MyConcierge.API/MyConcierge.sln` with four layers:

| Project | Role |
|---|---|
| `MyConcierge.Domain` | Entities, repository interfaces — no dependencies |
| `MyConcierge.Application` | Application services (business logic) — depends on Domain |
| `MyConcierge.Infrastructure` | EF Core `AppDbContext`, repository implementations, migrations — depends on Domain + Application |
| `MyConcierge.Presentation` | ASP.NET Core Web API controllers, DI wiring (`Program.cs`) — depends on Application + Infrastructure |
| `MyConcierge.Tests` | xUnit tests — depends on Application |

Dependency flow: `Presentation` → `Application` → `Domain` ← `Infrastructure`

## Commands

All commands run from the solution root `MyConcierge.API/`.

**Build**
```bash
dotnet build MyConcierge.sln
```

**Run the API**
```bash
dotnet run --project MyConcierge.Presentation/MyConcierge.Presentation.csproj
```

Swagger UI is available at `http://localhost:<port>/swagger` in Development mode. The root `/` redirects to Swagger.

**Run tests**
```bash
dotnet test MyConcierge.sln
```

**Run a single test**
```bash
dotnet test MyConcierge.Tests/MyConcierge.Tests.csproj --filter "FullyQualifiedName~TestMethodName"
```

**EF Core migrations** (run from `MyConcierge.Presentation/` or pass `--startup-project`):
```bash
# Add a migration
dotnet ef migrations add <MigrationName> --project ../MyConcierge.Infrastructure --startup-project MyConcierge.Presentation.csproj

# Apply migrations
dotnet ef database update --project ../MyConcierge.Infrastructure --startup-project MyConcierge.Presentation.csproj
```

## Architecture Patterns

**Repository pattern**: Domain defines interfaces (`IUniteRepository`, `IUtilisateurRepository`, `IContratsLocationRepository`, `IReferenceTypeRepository`). Infrastructure implements them. Both the interface and concrete class are registered in DI (`Program.cs`).

**Application services**: Thin wrappers around repository interfaces (e.g., `ReferenceTypeService`). Controllers inject the concrete service class directly (not an interface).

**Database**: SQL Server via EF Core. Connection string in `appsettings.json` targets `.\SQLEXPRESS` / `MyConciergeDb` with Windows auth. `AppDbContext` is in `MyConcierge.Infrastructure`.

## Domain Model

- `Utilisateur` — owners and tenants
- `Unite` — rental unit (apartment, room, building); supports parent/child hierarchy via `ParentUniteId`; linked to `ReferenceType` for unit type; references `Proprietaire` (Utilisateur)
- `ContratsLocation` — lease contracts linking a tenant (Utilisateur) to a Unite
- `ReferenceType` — legacy type enum table
- `ReferenceList` / `ReferenceValue` — newer flexible reference data system (e.g., `UNIT_TYPE` list with `APARTMENT`, `ROOM` values); `ReferenceList.Code` is unique; `(ReferenceListId, Code)` is unique on `ReferenceValue`

## Naming Conventions

The codebase is written in **French** for domain/method names (e.g., `ObtenirTousAsync`, `AjouterAsync`, `SupprimerAsync`, `Proprietaire`, `Locataire`). English is used for infrastructure/framework concerns. Follow this convention when adding new code.
