using System;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

namespace DataCatalog.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddUserMny : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Member",
                columns: new[] { "Id", "Email", "Name", "Password", "MemberRole" },
                values: new object[] { new Guid("a5cc7e91-cfa4-4fec-b70f-715978162561"), "mny@energinet.dk", "Mikkel Ladekarl Folmersen Nygaard", "S3cr3tP4ssw0rd", 0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("a5cc7e91-cfa4-4fec-b70f-715978162561"));
        }
    }
}
