using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

namespace DataCatalog.Api.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class CategoryReplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginEnvironment",
                table: "Category",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Category",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginEnvironment",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Category");
        }
    }
}
