using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Api.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class DatasetStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Dataset");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Dataset",
                nullable: false,
                defaultValue: 0);

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
                name: "Status",
                table: "Dataset");

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Dataset",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"),
                column: "MemberRoleId",
                value: new Guid("2375e551-d07a-4417-8357-6d8784474273"));
        }
    }
}
