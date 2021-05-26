using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DataCatalog.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddLZIWeatherAndResolution : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Member",
                columns: new[] { "Id", "Email", "Name", "Password", "MemberRole" },
                values: new object[] { new Guid("b62c4f4b-427e-4de6-b8d8-5c46e5265a64"), "lzi@energinet.dk", "Lasse Zink", "fy%C5^w$Jb7q", 1 });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "Name", "Colour", "ImageUri", "OriginEnvironment", "Version", "OriginDeleted" },
                values: new object[] { new Guid("6f4f675a-90c3-4504-955f-a823a897beb9"), "Weather", "#B27736", null, "prod", 0, false });

            migrationBuilder.InsertData(
                table: "Duration",
                columns: new[] { "Id", "Code", "Description" },
                values: new object[,]   { 
                                            { new Guid("e3c77675-d01b-4d5f-bdc5-7b5008e7a845"), "PT7M", "7 minutes" },
                                            { new Guid("827dfa85-1c1c-4c20-b11d-b5859b18d781"), "PT10M", "10 minutes" },
                                            { new Guid("7a94f1e9-1a7b-45e8-a74e-06eca8f2fb55"), "PT3H", "3 hours" },
                                            { new Guid("3732a912-e03a-428a-aa52-cb0f2961a1b8"), "PT4H", "4 hours" },
                                            { new Guid("4213523b-a7ac-4f24-85e5-23c57b13cb48"), "PT5H", "5 hours" },
                                            { new Guid("e0cca339-eb7c-4862-add0-9c83047f508d"), "P3D", "3 days" }
                                        });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("b62c4f4b-427e-4de6-b8d8-5c46e5265a64"));

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("6f4f675a-90c3-4504-955f-a823a897beb9"));

            migrationBuilder.DeleteData(
                table: "Duration",
                keyColumn: "Id",
                keyValue: new Guid("e3c77675-d01b-4d5f-bdc5-7b5008e7a845"));

            migrationBuilder.DeleteData(
                table: "Duration",
                keyColumn: "Id",
                keyValue: new Guid("827dfa85-1c1c-4c20-b11d-b5859b18d781"));

            migrationBuilder.DeleteData(
                table: "Duration",
                keyColumn: "Id",
                keyValue: new Guid("7a94f1e9-1a7b-45e8-a74e-06eca8f2fb55"));

            migrationBuilder.DeleteData(
                table: "Duration",
                keyColumn: "Id",
                keyValue: new Guid("3732a912-e03a-428a-aa52-cb0f2961a1b8"));

            migrationBuilder.DeleteData(
                table: "Duration",
                keyColumn: "Id",
                keyValue: new Guid("4213523b-a7ac-4f24-85e5-23c57b13cb48"));

            migrationBuilder.DeleteData(
                table: "Duration",
                keyColumn: "Id",
                keyValue: new Guid("e0cca339-eb7c-4862-add0-9c83047f508d"));
        }
    }
}
