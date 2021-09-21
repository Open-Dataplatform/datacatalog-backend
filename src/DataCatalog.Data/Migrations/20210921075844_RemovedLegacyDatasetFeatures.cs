using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Data.Migrations
{
    public partial class RemovedLegacyDatasetFeatures : Migration
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

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "RefinementLevel",
                table: "Dataset");

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceLevelAgreementId",
                table: "Dataset",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "DataField",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServiceLevelAgreement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLevelAgreement", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dataset_ServiceLevelAgreementId",
                table: "Dataset",
                column: "ServiceLevelAgreementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dataset_ServiceLevelAgreement_ServiceLevelAgreementId",
                table: "Dataset",
                column: "ServiceLevelAgreementId",
                principalTable: "ServiceLevelAgreement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dataset_ServiceLevelAgreement_ServiceLevelAgreementId",
                table: "Dataset");

            migrationBuilder.DropTable(
                name: "ServiceLevelAgreement");

            migrationBuilder.DropIndex(
                name: "IX_Dataset_ServiceLevelAgreementId",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "ServiceLevelAgreementId",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "DataField");

            migrationBuilder.AddColumn<Guid>(
                name: "HierarchyId",
                table: "Dataset",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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
