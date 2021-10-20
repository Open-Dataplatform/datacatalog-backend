using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Data.Migrations
{
    public partial class AddedDatasetPermissionChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DatasetChangeType",
                table: "DatasetChangeLog",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Update");

            migrationBuilder.CreateTable(
                name: "DatasetPermissionChange",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionChangeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccessType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccessMemberType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DirectoryObjectId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatasetChangeLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetPermissionChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DatasetPermissionChange_DatasetChangeLog_DatasetChangeLogId",
                        column: x => x.DatasetChangeLogId,
                        principalTable: "DatasetChangeLog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatasetPermissionChange_DatasetChangeLogId",
                table: "DatasetPermissionChange",
                column: "DatasetChangeLogId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatasetPermissionChange");

            migrationBuilder.DropColumn(
                name: "DatasetChangeType",
                table: "DatasetChangeLog");
        }
    }
}
