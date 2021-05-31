using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddPassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Member",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("64564746-b34b-4416-a412-df3503bf45db"),
                columns: new[] { "Email", "MemberRole", "Password" },
                values: new object[] { "user@dummy.com", 2, "dummy" });

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"),
                columns: new[] { "Email", "MemberRole", "Password" },
                values: new object[] { "steward@dummy.com", 1, "dummy" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Member");

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("64564746-b34b-4416-a412-df3503bf45db"),
                columns: new[] { "Email", "MemberRole" },
                values: new object[] { "dummyuser@member.dk", 2 });

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"),
                columns: new[] { "Email", "MemberRole" },
                values: new object[] { "dummy@member.dk", 1 });
        }
    }
}
