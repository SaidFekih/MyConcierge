using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyConcierge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjoutPaiements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Paiements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContratsLocationId = table.Column<int>(type: "int", nullable: false),
                    Montant = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DatePaiement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstSupprime = table.Column<bool>(type: "bit", nullable: false),
                    DateSuppression = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paiements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Paiements_ContratsLocations_ContratsLocationId",
                        column: x => x.ContratsLocationId,
                        principalTable: "ContratsLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Paiements_ContratsLocationId",
                table: "Paiements",
                column: "ContratsLocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Paiements");
        }
    }
}
