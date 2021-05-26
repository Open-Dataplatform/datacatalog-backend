using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

namespace DataCatalog.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddReplicantOriginDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OriginDeleted",
                table: "DataSource",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OriginDeleted",
                table: "Dataset",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OriginDeleted",
                table: "DataContract",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OriginDeleted",
                table: "Category",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginDeleted",
                table: "DataSource");

            migrationBuilder.DropColumn(
                name: "OriginDeleted",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "OriginDeleted",
                table: "DataContract");

            migrationBuilder.DropColumn(
                name: "OriginDeleted",
                table: "Category");
        }
    }
}
