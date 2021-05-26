using System;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

namespace DataCatalog.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AchAccounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Member",
                columns: new[] { "Id", "Email", "MemberRole", "Name", "Password" },
                values: new object[,]
                {
                    { new Guid("a2edfe21-83d9-4fb1-9365-101f4f0d38c3"), "ach-user@energinet.dk", 2, "André Bryde Alnor (bruger)", "ÆNådDoKaGæt" },
                    { new Guid("905984a3-0e70-4ff5-85be-6325b3d1bab8"), "ach-datasteward@energinet.dk", 1, "André Bryde Alnor (data steward)", "ÆNådDoKaGæt" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("905984a3-0e70-4ff5-85be-6325b3d1bab8"));

            migrationBuilder.DeleteData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("a2edfe21-83d9-4fb1-9365-101f4f0d38c3"));
        }
    }
}