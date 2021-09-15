using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Data.Migrations
{
    public partial class RemoveDataContracts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataContract");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataContract",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    DataSourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DatasetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    OriginDeleted = table.Column<bool>(type: "bit", nullable: false),
                    OriginEnvironment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataContract", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataContract_Dataset_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Dataset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DataContract_DataSource_DataSourceId",
                        column: x => x.DataSourceId,
                        principalTable: "DataSource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataContract_DatasetId",
                table: "DataContract",
                column: "DatasetId");

            migrationBuilder.CreateIndex(
                name: "IX_DataContract_DataSourceId",
                table: "DataContract",
                column: "DataSourceId");
        }
    }
}
