using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyConcierge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTypeEntite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Unites_ReferenceTypes_ReferenceTypeId",
                table: "Unites");

            migrationBuilder.DropForeignKey(
                name: "FK_Utilisateurs_ReferenceTypes_ReferenceTypeId",
                table: "Utilisateurs");

            migrationBuilder.DropTable(
                name: "ReferenceTypes");

            migrationBuilder.DropTable(
                name: "ReferenceValues");

            migrationBuilder.DropTable(
                name: "ReferenceLists");

            migrationBuilder.DropColumn(
                name: "EstLouee",
                table: "Unites");

            migrationBuilder.RenameColumn(
                name: "ReferenceTypeId",
                table: "Utilisateurs",
                newName: "TypeEntiteId");

            migrationBuilder.RenameIndex(
                name: "IX_Utilisateurs_ReferenceTypeId",
                table: "Utilisateurs",
                newName: "IX_Utilisateurs_TypeEntiteId");

            migrationBuilder.RenameColumn(
                name: "ReferenceTypeId",
                table: "Unites",
                newName: "TypeEntiteId");

            migrationBuilder.RenameColumn(
                name: "Prix",
                table: "Unites",
                newName: "MontantLoyer");

            migrationBuilder.RenameIndex(
                name: "IX_Unites_ReferenceTypeId",
                table: "Unites",
                newName: "IX_Unites_TypeEntiteId");

            migrationBuilder.AddColumn<int>(
                name: "Statut",
                table: "Unites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TypeEntites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Categorie = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeEntites", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Unites_TypeEntites_TypeEntiteId",
                table: "Unites",
                column: "TypeEntiteId",
                principalTable: "TypeEntites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Utilisateurs_TypeEntites_TypeEntiteId",
                table: "Utilisateurs",
                column: "TypeEntiteId",
                principalTable: "TypeEntites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Unites_TypeEntites_TypeEntiteId",
                table: "Unites");

            migrationBuilder.DropForeignKey(
                name: "FK_Utilisateurs_TypeEntites_TypeEntiteId",
                table: "Utilisateurs");

            migrationBuilder.DropTable(
                name: "TypeEntites");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "Unites");

            migrationBuilder.RenameColumn(
                name: "TypeEntiteId",
                table: "Utilisateurs",
                newName: "ReferenceTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Utilisateurs_TypeEntiteId",
                table: "Utilisateurs",
                newName: "IX_Utilisateurs_ReferenceTypeId");

            migrationBuilder.RenameColumn(
                name: "TypeEntiteId",
                table: "Unites",
                newName: "ReferenceTypeId");

            migrationBuilder.RenameColumn(
                name: "MontantLoyer",
                table: "Unites",
                newName: "Prix");

            migrationBuilder.RenameIndex(
                name: "IX_Unites_TypeEntiteId",
                table: "Unites",
                newName: "IX_Unites_ReferenceTypeId");

            migrationBuilder.AddColumn<bool>(
                name: "EstLouee",
                table: "Unites",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ReferenceLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenceLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReferenceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Categorie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReferenceValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceListId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenceValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferenceValues_ReferenceLists_ReferenceListId",
                        column: x => x.ReferenceListId,
                        principalTable: "ReferenceLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceLists_Code",
                table: "ReferenceLists",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceValues_ReferenceListId_Code",
                table: "ReferenceValues",
                columns: new[] { "ReferenceListId", "Code" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Unites_ReferenceTypes_ReferenceTypeId",
                table: "Unites",
                column: "ReferenceTypeId",
                principalTable: "ReferenceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Utilisateurs_ReferenceTypes_ReferenceTypeId",
                table: "Utilisateurs",
                column: "ReferenceTypeId",
                principalTable: "ReferenceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
