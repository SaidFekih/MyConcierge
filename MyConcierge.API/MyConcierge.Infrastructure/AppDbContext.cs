using Microsoft.EntityFrameworkCore;
using MyConcierge.Domain.Models;
using System.Collections.Generic;
using System.Net;


namespace MyConcierge.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Unite> Unites { get; set; }
        public DbSet<ContratsLocation> ContratsLocations { get; set; }
        public DbSet<TypeEntite> TypeEntites { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TypeEntite>().ToTable("TypeEntites");// Mapping SQL
            modelBuilder.Entity<Utilisateur>().ToTable("Utilisateurs");// Mapping SQL
            modelBuilder.Entity<Unite>().ToTable("Unites");// Mapping SQL
            modelBuilder.Entity<ContratsLocation>().ToTable("ContratsLocations");// Mapping SQL

            // Seed TypeEntites
            modelBuilder.Entity<TypeEntite>().HasData(
                // Types Utilisateur
                new TypeEntite { Id = 1, Nom = "Locataire", Categorie = CategorieName.Utilisateur, Description = "Personne qui loue une unité" },
                new TypeEntite { Id = 2, Nom = "Propriétaire", Categorie = CategorieName.Utilisateur, Description = "Personne qui possède une unité" },
                new TypeEntite { Id = 3, Nom = "Gestionnaire", Categorie = CategorieName.Utilisateur, Description = "Personne qui gère les propriétés" },

                // Types Unite
                new TypeEntite { Id = 4, Nom = "Appartement", Categorie = CategorieName.Unite, Description = "Unité résidentielle autonome" },
                new TypeEntite { Id = 5, Nom = "Chambre", Categorie = CategorieName.Unite, Description = "Chambre dans un logement partagé" },
                new TypeEntite { Id = 6, Nom = "Immeuble", Categorie = CategorieName.Unite, Description = "Bâtiment contenant plusieurs unités" },

                // Types Contrat
                new TypeEntite { Id = 7, Nom = "Bail annuel", Categorie = CategorieName.Contrat, Description = "Contrat de location d'un an" },
                new TypeEntite { Id = 8, Nom = "Bail mensuel", Categorie = CategorieName.Contrat, Description = "Contrat de location mois par mois" }
            );

        }
    }
}
