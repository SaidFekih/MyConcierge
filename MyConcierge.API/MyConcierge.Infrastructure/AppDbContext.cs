using Microsoft.EntityFrameworkCore;
using MyConcierge.Domain.Models;
using System.Collections.Generic;
using System.Net;


namespace MyConcierge.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }
        public DbSet<ReferenceType> ReferenceTypes { get; set; }
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Unite> Unites { get; set; }
        public DbSet<ContratsLocation> ContratsLocations { get; set; }
        public DbSet<ReferenceList> ReferenceLists { get; set; }
        public DbSet<ReferenceValue> ReferenceValues { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ReferenceType>().ToTable("ReferenceTypes");// Mapping SQL
            modelBuilder.Entity<Utilisateur>().ToTable("Utilisateurs");// Mapping SQL
            modelBuilder.Entity<Unite>().ToTable("Unites");// Mapping SQL
            modelBuilder.Entity<ContratsLocation>().ToTable("ContratsLocations");// Mapping SQL
            modelBuilder.Entity<ReferenceList>().ToTable("ReferenceLists");
            modelBuilder.Entity<ReferenceValue>().ToTable("ReferenceValues");
            /*
             HasIndex ca évite :
                    2 listes avec le même Code (UNIT_TYPE en double)
                    2 valeurs identiques dans la même liste (UNIT_TYPE + APARTMENT en double)
            */
            modelBuilder.Entity<ReferenceList>().HasIndex(x => x.Code).IsUnique();
            modelBuilder.Entity<ReferenceValue>().HasIndex(x => new { x.ReferenceListId, x.Code }).IsUnique();
            modelBuilder.Entity<ReferenceValue>().HasOne(x => x.ReferenceList).WithMany(x => x.Values).HasForeignKey(x => x.ReferenceListId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}
