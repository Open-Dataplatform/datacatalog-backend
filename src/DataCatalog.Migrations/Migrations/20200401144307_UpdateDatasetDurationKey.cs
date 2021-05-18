using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Api.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class UpdateDatasetDurationKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DatasetDuration",
                table: "DatasetDuration");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DatasetDuration",
                table: "DatasetDuration",
                columns: new[] { "DatasetId", "DurationId", "DurationType" });

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"),
                column: "MemberRoleId",
                value: new Guid("2375e551-d07a-4417-8357-6d8784474273"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DatasetDuration",
                table: "DatasetDuration");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DatasetDuration",
                table: "DatasetDuration",
                columns: new[] { "DatasetId", "DurationId" });

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"),
                column: "MemberRoleId",
                value: new Guid("2375e551-d07a-4417-8357-6d8784474273"));
        }
    }
}
