using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyConcierge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Unites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ReferenceTypeId = table.Column<int>(type: "int", nullable: false),
                    ParentUniteId = table.Column<int>(type: "int", nullable: true),
                    ProprietaireId = table.Column<int>(type: "int", nullable: false),
                    Prix = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstLouee = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Unites_ReferenceTypes_ReferenceTypeId",
                        column: x => x.ReferenceTypeId,
                        principalTable: "ReferenceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Unites_Unites_ParentUniteId",
                        column: x => x.ParentUniteId,
                        principalTable: "Unites",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Unites_Utilisateurs_ProprietaireId",
                        column: x => x.ProprietaireId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Unites_ParentUniteId",
                table: "Unites",
                column: "ParentUniteId");

            migrationBuilder.CreateIndex(
                name: "IX_Unites_ProprietaireId",
                table: "Unites",
                column: "ProprietaireId");

            migrationBuilder.CreateIndex(
                name: "IX_Unites_ReferenceTypeId",
                table: "Unites",
                column: "ReferenceTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Unites");
        }
    }
}
