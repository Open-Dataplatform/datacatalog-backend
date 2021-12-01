using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Data.Migrations
{
    public partial class AddedDataAvailabilityInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataAvailabilityInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstAvailableData = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LatestAvailableData = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatasetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataAvailabilityInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataAvailabilityInfo_Dataset_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Dataset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataAvailabilityInfo_DatasetId",
                table: "DataAvailabilityInfo",
                column: "DatasetId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataAvailabilityInfo");
        }
    }
}
