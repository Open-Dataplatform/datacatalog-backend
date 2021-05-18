using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Api.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddDPUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Member",
                columns: new[] { "Id", "Email", "Name", "Password", "MemberRole" },
                values: new object[] { new Guid("8c30dee9-af3f-4bd1-bca4-641492e7690b"), "dataplatform@energinet.dk", "Data Platform", "GoesWithoutSaying", 0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("8c30dee9-af3f-4bd1-bca4-641492e7690b"));
        }
    }
}