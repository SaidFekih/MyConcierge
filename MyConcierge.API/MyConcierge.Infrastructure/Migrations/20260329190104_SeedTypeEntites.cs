using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyConcierge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedTypeEntites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TypeEntites",
                columns: new[] { "Id", "Categorie", "Description", "Nom" },
                values: new object[,]
                {
                    { 1, 1, "Personne qui loue une unité", "Locataire" },
                    { 2, 1, "Personne qui possède une unité", "Propriétaire" },
                    { 3, 1, "Personne qui gère les propriétés", "Gestionnaire" },
                    { 4, 0, "Unité résidentielle autonome", "Appartement" },
                    { 5, 0, "Chambre dans un logement partagé", "Chambre" },
                    { 6, 0, "Bâtiment contenant plusieurs unités", "Immeuble" },
                    { 7, 2, "Contrat de location d'un an", "Bail annuel" },
                    { 8, 2, "Contrat de location mois par mois", "Bail mensuel" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TypeEntites",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TypeEntites",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TypeEntites",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TypeEntites",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TypeEntites",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TypeEntites",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TypeEntites",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TypeEntites",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
