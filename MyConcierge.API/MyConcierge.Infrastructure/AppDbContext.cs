using Microsoft.EntityFrameworkCore;
using MyConcierge.Domain.Models;


namespace MyConcierge.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }
        public DbSet<ReferenceType> ReferenceTypes { get; set; }
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Unite> Unites { get; set; }
        public DbSet<ContratsLocation> ContratsLocations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ReferenceType>().ToTable("ReferenceTypes");// Mapping SQL
            modelBuilder.Entity<Utilisateur>().ToTable("Utilisateurs");// Mapping SQL
            modelBuilder.Entity<Unite>().ToTable("Unites");// Mapping SQL
            modelBuilder.Entity<ContratsLocation>().ToTable("ContratsLocations");// Mapping SQL
        }
    }
}
