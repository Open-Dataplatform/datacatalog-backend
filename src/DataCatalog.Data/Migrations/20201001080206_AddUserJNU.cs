using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DataCatalog.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddUserJNU : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Member",
                columns: new[] { "Id", "Email", "Name", "Password", "MemberRole" },
                values: new object[] { new Guid("68dfdbe8-5ea1-4912-b4b1-108999c3f8f0"), "jnu@energinet.dk", "Janus Kjær Andersen", "4X%1@RWm", 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("68dfdbe8-5ea1-4912-b4b1-108999c3f8f0"));
        }
    }
}
