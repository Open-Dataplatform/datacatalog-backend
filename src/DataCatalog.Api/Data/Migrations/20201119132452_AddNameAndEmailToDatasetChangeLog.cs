using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Api.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddNameAndEmailToDatasetChangeLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DatasetChangeLog",
                nullable: true,
                defaultValue: "");
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "DatasetChangeLog",
                nullable: true,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                table: "DatasetChangeLog",
                name: "Name"
            );
            migrationBuilder.DropColumn(
                table: "DatasetChangeLog",
                name: "Email"
            );
        }
    }
}
