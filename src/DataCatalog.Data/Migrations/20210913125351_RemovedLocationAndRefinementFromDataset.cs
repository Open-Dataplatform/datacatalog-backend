using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Data.Migrations
{
    public partial class RemovedLocationAndRefinementFromDataset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "RefinementLevel",
                table: "Dataset");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Dataset",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RefinementLevel",
                table: "Dataset",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
