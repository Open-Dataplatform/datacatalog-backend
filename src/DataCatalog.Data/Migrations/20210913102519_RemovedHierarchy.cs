using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Data.Migrations
{
    public partial class RemovedHierarchy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dataset_Hierarchy_HierarchyId",
                table: "Dataset");

            migrationBuilder.DropTable(
                name: "Hierarchy");

            migrationBuilder.DropIndex(
                name: "IX_Dataset_HierarchyId",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "HierarchyId",
                table: "Dataset");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HierarchyId",
                table: "Dataset",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Hierarchy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentHierarchyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hierarchy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hierarchy_Hierarchy_ParentHierarchyId",
                        column: x => x.ParentHierarchyId,
                        principalTable: "Hierarchy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dataset_HierarchyId",
                table: "Dataset",
                column: "HierarchyId");

            migrationBuilder.CreateIndex(
                name: "IX_Hierarchy_ParentHierarchyId",
                table: "Hierarchy",
                column: "ParentHierarchyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dataset_Hierarchy_HierarchyId",
                table: "Dataset",
                column: "HierarchyId",
                principalTable: "Hierarchy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
