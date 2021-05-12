using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Api.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddTempDataStewardLogins : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MemberGroupMember",
                keyColumns: new[] { "MemberGroupId", "MemberId" },
                keyValues: new object[] { new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f") });

            migrationBuilder.DeleteData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"));                      

            migrationBuilder.InsertData(
                table: "Member",
                columns: new[] { "Id", "Email", "MemberRole", "Name", "Password" },
                values: new object[,]
                {
                    { new Guid("2fe99cad-bee1-4b48-9336-a793ebdc6e92"), "sjj@energinet.dk", 1, "Sune Buss Vels Jensen", "MegetHemmeligt" },
                    { new Guid("327bf4e6-bccb-4702-8966-0f7afefdd617"), "jtg@energinet.dk", 1, "James Clifford Stegmann", "IngenProblemer" }
                });

            migrationBuilder.UpdateData(
                table: "MemberGroup",
                keyColumn: "Id",
                keyValue: new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"),
                columns: new[] { "Email", "Name" },
                values: new object[] { "sjj@energinet.dk", "Sune's Group" });

            migrationBuilder.InsertData(
                table: "MemberGroup",
                columns: new[] { "Id", "Description", "Email", "Name" },
                values: new object[] { new Guid("ce24f2c1-2156-4d4f-b330-5fc7c4988f09"), null, "jtg@energinet.dk", "James's Group" });

            migrationBuilder.InsertData(
                table: "MemberGroupMember",
                columns: new[] { "MemberGroupId", "MemberId" },
                values: new object[] { new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), new Guid("2fe99cad-bee1-4b48-9336-a793ebdc6e92") });

            migrationBuilder.InsertData(
                table: "MemberGroupMember",
                columns: new[] { "MemberGroupId", "MemberId" },
                values: new object[] { new Guid("ce24f2c1-2156-4d4f-b330-5fc7c4988f09"), new Guid("327bf4e6-bccb-4702-8966-0f7afefdd617") });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MemberGroupMember",
                keyColumns: new[] { "MemberGroupId", "MemberId" },
                keyValues: new object[] { new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), new Guid("2fe99cad-bee1-4b48-9336-a793ebdc6e92") });

            migrationBuilder.DeleteData(
                table: "MemberGroupMember",
                keyColumns: new[] { "MemberGroupId", "MemberId" },
                keyValues: new object[] { new Guid("ce24f2c1-2156-4d4f-b330-5fc7c4988f09"), new Guid("327bf4e6-bccb-4702-8966-0f7afefdd617") });

            migrationBuilder.DeleteData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("2fe99cad-bee1-4b48-9336-a793ebdc6e92"));

            migrationBuilder.DeleteData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("327bf4e6-bccb-4702-8966-0f7afefdd617"));

            migrationBuilder.DeleteData(
                table: "MemberGroup",
                keyColumn: "Id",
                keyValue: new Guid("ce24f2c1-2156-4d4f-b330-5fc7c4988f09"));

            migrationBuilder.InsertData(
                table: "Member",
                columns: new[] { "Id", "Email", "MemberRole", "Name", "Password" },
                values: new object[] { new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"), "steward@dummy.com", 1, "Dummy Steward", "dummy" });

            migrationBuilder.UpdateData(
                table: "MemberGroup",
                keyColumn: "Id",
                keyValue: new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"),
                columns: new[] { "Email", "Name" },
                values: new object[] { "dummy@steward.dk", "Dummy steward group" });

            migrationBuilder.InsertData(
                table: "MemberGroupMember",
                columns: new[] { "MemberGroupId", "MemberId" },
                values: new object[] { new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f") });
        }
    }
}
