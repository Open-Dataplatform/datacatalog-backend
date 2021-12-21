using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Data.Migrations
{
    public partial class CascadeDeleteDataAvailability : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataAvailabilityInfo_Dataset_DatasetId",
                table: "DataAvailabilityInfo");

            migrationBuilder.AddForeignKey(
                name: "FK_DataAvailabilityInfo_Dataset_DatasetId",
                table: "DataAvailabilityInfo",
                column: "DatasetId",
                principalTable: "Dataset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataAvailabilityInfo_Dataset_DatasetId",
                table: "DataAvailabilityInfo");

            migrationBuilder.AddForeignKey(
                name: "FK_DataAvailabilityInfo_Dataset_DatasetId",
                table: "DataAvailabilityInfo",
                column: "DatasetId",
                principalTable: "Dataset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
