using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Api.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class FixChangeLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DatasetChangeLog",
                table: "DatasetChangeLog");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "DatasetChangeLog",
                nullable: false,
                defaultValueSql: "newid()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DatasetChangeLog",
                table: "DatasetChangeLog",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"),
                column: "MemberRole",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_DatasetChangeLog_DatasetId",
                table: "DatasetChangeLog",
                column: "DatasetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DatasetChangeLog",
                table: "DatasetChangeLog");

            migrationBuilder.DropIndex(
                name: "IX_DatasetChangeLog_DatasetId",
                table: "DatasetChangeLog");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DatasetChangeLog");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DatasetChangeLog",
                table: "DatasetChangeLog",
                columns: new[] { "DatasetId", "MemberId" });

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"),
                column: "MemberRole",
                value: 1);
        }
    }
}
