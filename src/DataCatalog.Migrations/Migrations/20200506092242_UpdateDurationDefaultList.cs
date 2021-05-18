using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Api.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class UpdateDurationDefaultList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Duration",
                keyColumn: "Id",
                keyValue: new Guid("f745f308-1542-4a7a-975f-0cd6f1e73668"),
                columns: new[] { "Code", "Description" },
                values: new object[] { "PT1H", "1 hour" });

            migrationBuilder.InsertData(
                table: "Duration",
                columns: new[] { "Id", "Code", "Description" },
                values: new object[,]
                {
                    { new Guid("2ff90ae1-27a8-4cb8-a697-6e81a5e36fb0"), "P1Y", "1 year" },
                    { new Guid("6817a5e6-7c80-470f-a1b2-fa79ed3c125d"), "P1M", "1 month" },
                    { new Guid("c70a3773-3473-4074-8c08-4a1bfffaa1d1"), "P7D", "7 days" },
                    { new Guid("20986017-c3a8-41a0-8221-b5e1a5995c05"), "P1D", "1 day" },
                    { new Guid("2b3d0756-0817-4281-a2ca-76c2b0eb90d6"), "PT12H", "12 hours" },
                    { new Guid("984b1eb4-0420-4354-8ae7-a9b6c9c33aa8"), "PT6H", "6 hours" },
                    { new Guid("2fb37506-3252-4065-a81b-5c633e158a7c"), "PT15M", "15 minutes" },
                    { new Guid("b64a8a70-cb40-4117-93ba-093571ea4aeb"), "PT5M", "5 minutes" },
                    { new Guid("d05a8570-6155-41bd-95b1-20e6f2856cc2"), "PT3M", "3 minutes" },
                    { new Guid("b57f0531-0eb0-4bc2-b81c-266a3f52368e"), "PT1M", "1 minute" }
                });

            migrationBuilder.InsertData(
                table: "Member",
                columns: new[] { "Id", "Email", "Name", "Password", "MemberRole" },
                values: new object[] { new Guid("d81769d1-b643-41e6-ad5a-ec6b86f7e608"), "admin@dummy.com", "Dummy Admin", "IAmAdmin", 0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Duration",
                keyColumn: "Id",
                keyValue: new Guid("20986017-c3a8-41a0-8221-b5e1a5995c05"));

            migrationBuilder.DeleteData(
                table: "Duration",
                keyColumn: "Id",
                keyValue: new Guid("2b3d0756-0817-4281-a2ca-76c2b0eb90d6"));

            migrationBuilder.DeleteData(
                table: "Duration",
                keyColumn: "Id",
                keyValue: new Guid("2fb37506-3252-4065-a81b-5c633e158a7c"));

            migrationBuilder.DeleteData(
                table: "Duration",
                keyColumn: "Id",
                keyValue: new Guid("2ff90ae1-27a8-4cb8-a697-6e81a5e36fb0"));

            migrationBuilder.DeleteData(
                table: "Duration",
                keyColumn: "Id",
                keyValue: new Guid("6817a5e6-7c80-470f-a1b2-fa79ed3c125d"));

            migrationBuilder.DeleteData(
                table: "Duration",
                keyColumn: "Id",
                keyValue: new Guid("984b1eb4-0420-4354-8ae7-a9b6c9c33aa8"));

            migrationBuilder.DeleteData(
                table: "Duration",
                keyColumn: "Id",
                keyValue: new Guid("b57f0531-0eb0-4bc2-b81c-266a3f52368e"));

            migrationBuilder.DeleteData(
                table: "Duration",
                keyColumn: "Id",
                keyValue: new Guid("b64a8a70-cb40-4117-93ba-093571ea4aeb"));

            migrationBuilder.DeleteData(
                table: "Duration",
                keyColumn: "Id",
                keyValue: new Guid("c70a3773-3473-4074-8c08-4a1bfffaa1d1"));

            migrationBuilder.DeleteData(
                table: "Duration",
                keyColumn: "Id",
                keyValue: new Guid("d05a8570-6155-41bd-95b1-20e6f2856cc2"));

            migrationBuilder.DeleteData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("d81769d1-b643-41e6-ad5a-ec6b86f7e608"));

            migrationBuilder.UpdateData(
                table: "Duration",
                keyColumn: "Id",
                keyValue: new Guid("f745f308-1542-4a7a-975f-0cd6f1e73668"),
                columns: new[] { "Code", "Description" },
                values: new object[] { "P1H", "Every hour" });            
        }
    }
}