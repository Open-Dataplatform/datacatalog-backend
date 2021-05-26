using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class UpdateSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"),
                columns: new[] { "Email", "MemberRoleId" },
                values: new object[] { "dummy@member.dk", new Guid("2375e551-d07a-4417-8357-6d8784474273") });

            migrationBuilder.InsertData(
                table: "MemberGroup",
                columns: new[] { "Id", "Description", "Email", "Name" },
                values: new object[] { new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, "dummy@steward.dk", "Dummy steward group" });

            migrationBuilder.InsertData(
                table: "MemberGroupMember",
                columns: new[] { "MemberGroupId", "MemberId" },
                values: new object[] { new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f") });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MemberGroupMember",
                keyColumns: new[] { "MemberGroupId", "MemberId" },
                keyValues: new object[] { new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f") });

            migrationBuilder.DeleteData(
                table: "MemberGroup",
                keyColumn: "Id",
                keyValue: new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"));

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"),
                columns: new[] { "Email", "MemberRoleId" },
                values: new object[] { "dummy@fake.com", new Guid("2375e551-d07a-4417-8357-6d8784474273") });
        }
    }
}
