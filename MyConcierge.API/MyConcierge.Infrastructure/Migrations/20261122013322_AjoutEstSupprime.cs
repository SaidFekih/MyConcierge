using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyConcierge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjoutEstSupprime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateSuppression",
                table: "Utilisateurs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EstSupprime",
                table: "Utilisateurs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateSuppression",
                table: "Unites",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EstSupprime",
                table: "Unites",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateSuppression",
                table: "ContratsLocations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EstSupprime",
                table: "ContratsLocations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateSuppression",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "EstSupprime",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "DateSuppression",
                table: "Unites");

            migrationBuilder.DropColumn(
                name: "EstSupprime",
                table: "Unites");

            migrationBuilder.DropColumn(
                name: "DateSuppression",
                table: "ContratsLocations");

            migrationBuilder.DropColumn(
                name: "EstSupprime",
                table: "ContratsLocations");
        }
    }
}
