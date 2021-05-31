using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dataset_Confidentiality_ConfidentialityId",
                table: "Dataset");

            migrationBuilder.DropForeignKey(
                name: "FK_Dataset_Source_SourceId",
                table: "Dataset");

            migrationBuilder.DropTable(
                name: "Source");

            migrationBuilder.DropIndex(
                name: "IX_Dataset_ConfidentialityId",
                table: "Dataset");

            migrationBuilder.DropIndex(
                name: "IX_Dataset_SourceId",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "ConfidentialityId",
                table: "Dataset");

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Confidentiality",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Confidentiality",
                keyColumn: "Id",
                keyValue: new Guid("568f7357-fce8-4733-b15d-2e0c4c3d8bd6"),
                column: "Level",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Confidentiality",
                keyColumn: "Id",
                keyValue: new Guid("5cf6fe55-13f4-4503-8ae0-394f6e09bf3d"),
                column: "Level",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"),
                column: "MemberRoleId",
                value: new Guid("2375e551-d07a-4417-8357-6d8784474273"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "Confidentiality");

            migrationBuilder.AddColumn<Guid>(
                name: "ConfidentialityId",
                table: "Dataset",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Source",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceType = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Source", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"),
                column: "MemberRoleId",
                value: new Guid("2375e551-d07a-4417-8357-6d8784474273"));

            migrationBuilder.InsertData(
                table: "Source",
                columns: new[] { "Id", "ContactInfo", "Name" },
                values: new object[] { new Guid("5777efb5-f55c-4f16-bb83-0125b810f995"), "dataplatform@energinet.dk", "Data Platform" });

            migrationBuilder.CreateIndex(
                name: "IX_Dataset_ConfidentialityId",
                table: "Dataset",
                column: "ConfidentialityId");

            migrationBuilder.CreateIndex(
                name: "IX_Dataset_SourceId",
                table: "Dataset",
                column: "SourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dataset_Confidentiality_ConfidentialityId",
                table: "Dataset",
                column: "ConfidentialityId",
                principalTable: "Confidentiality",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Dataset_Source_SourceId",
                table: "Dataset",
                column: "SourceId",
                principalTable: "Source",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
