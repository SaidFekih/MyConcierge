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
  
        }
    }
}
