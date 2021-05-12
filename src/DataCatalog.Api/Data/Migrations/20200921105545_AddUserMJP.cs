using System;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

namespace DataCatalog.Api.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddUserMJP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Member",
                columns: new[] { "Id", "Email", "Name", "Password", "MemberRole" },
                values: new object[] { new Guid("1755f50d-ff77-43ff-bd31-dc6df11b46e8"), "mjp@energinet.dk", "Morten Jessen-Petersen", "S3cr3tP4ssw0rd", 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("1755f50d-ff77-43ff-bd31-dc6df11b46e8"));
        }
    }
}
