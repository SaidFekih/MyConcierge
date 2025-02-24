using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyConcierge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContratsLocationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContratsLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocataireId = table.Column<int>(type: "int", nullable: false),
                    UniteId = table.Column<int>(type: "int", nullable: false),
                    DateDebut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Montant = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContratsLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContratsLocations_Unites_UniteId",
                        column: x => x.UniteId,
                        principalTable: "Unites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContratsLocations_Utilisateurs_LocataireId",
                        column: x => x.LocataireId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContratsLocations_LocataireId",
                table: "ContratsLocations",
                column: "LocataireId");

            migrationBuilder.CreateIndex(
                name: "IX_ContratsLocations_UniteId",
                table: "ContratsLocations",
                column: "UniteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContratsLocations");
        }
    }
}
