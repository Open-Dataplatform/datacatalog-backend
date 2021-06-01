using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class FixArchived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("IsArchived", "Dataset");

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Dataset",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("IsArchived", "Dataset");

            migrationBuilder.AddColumn<string>(
                name: "IsArchived",
                table: "Dataset",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
