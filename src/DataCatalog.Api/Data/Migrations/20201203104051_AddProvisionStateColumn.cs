using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Api.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddProvisionStateColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProvisionStatus",
                table: "Dataset",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProvisionStatus",
                table: "Dataset");
        }
    }
}
