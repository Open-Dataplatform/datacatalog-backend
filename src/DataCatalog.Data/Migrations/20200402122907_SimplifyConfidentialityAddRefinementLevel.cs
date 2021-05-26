using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class SimplifyConfidentialityAddRefinementLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataField_Confidentiality_ConfidentialityId",
                table: "DataField");

            migrationBuilder.DropTable(
                name: "Confidentiality");

            migrationBuilder.DropIndex(
                name: "IX_DataField_ConfidentialityId",
                table: "DataField");

            migrationBuilder.DropColumn(
                name: "ConfidentialityId",
                table: "DataField");

            migrationBuilder.AddColumn<int>(
                name: "Confidentiality",
                table: "Dataset",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RefinementLevel",
                table: "Dataset",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Confidentiality",
                table: "DataField",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"),
                column: "MemberRole",
                value: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confidentiality",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "RefinementLevel",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "Confidentiality",
                table: "DataField");

            migrationBuilder.AddColumn<Guid>(
                name: "ConfidentialityId",
                table: "DataField",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("62c544b2-ee11-4434-b9b7-39ffb1ab9616"));

            migrationBuilder.CreateTable(
                name: "Confidentiality",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Confidentiality", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Confidentiality",
                columns: new[] { "Id", "Description", "Level", "Name" },
                values: new object[,]
                {
                    { new Guid("62c544b2-ee11-4434-b9b7-39ffb1ab9616"), "Freely available", 0, "Nonconfidential" },
                    { new Guid("5cf6fe55-13f4-4503-8ae0-394f6e09bf3d"), "Sensitive, but available for work-related tasks", 1, "Confidential" },
                    { new Guid("568f7357-fce8-4733-b15d-2e0c4c3d8bd6"), "Only available with special clearance", 2, "Strictly confidential" }
                });

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"),
                column: "MemberRole",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_DataField_ConfidentialityId",
                table: "DataField",
                column: "ConfidentialityId");

            migrationBuilder.AddForeignKey(
                name: "FK_DataField_Confidentiality_ConfidentialityId",
                table: "DataField",
                column: "ConfidentialityId",
                principalTable: "Confidentiality",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
