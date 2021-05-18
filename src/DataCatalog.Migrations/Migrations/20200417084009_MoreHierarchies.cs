using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Api.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class MoreHierarchies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Hierarchy",
                columns: new[] { "Id", "Description", "Name", "ParentHierarchyId" },
                values: new object[,]
                {
                    { new Guid("276af4b1-72ea-4654-8ce3-37e73b2657c0"), "Electricity production", "Production", new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                    { new Guid("a9a780d7-8f2c-4d86-b863-1327bd264575"), "Electricity consumption and production", "Consumption and Production", new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                    { new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"), "Electricity reserves", "Reserves", new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                    { new Guid("a6b40fdf-9831-49ec-87b0-cfed8f447577"), "Electricity ancillary services", "Ancillary Services", new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                    { new Guid("5f2d33fc-decc-4061-af3c-5d58e65bbc9c"), "Electricity CO2 Emissions", "CO2 Emissions", new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                    { new Guid("d27b19d3-e589-4f98-809a-351a062635e8"), "Electricity regulating power", "Regulating Power", new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                    { new Guid("fb668c70-219c-4edf-b31a-7ad36ee43d88"), "Electricity solar power", "Solar Power", new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                    { new Guid("ded03663-ec2e-445f-b897-b5b78c00f2d4"), "Electricity wind power", "Wind Power", new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                    { new Guid("49ca837b-79ba-4a4f-bbc5-e01a8f23befd"), "Gas Production", "Production", new Guid("586c321c-3f01-4cd2-8228-000670114e32") },
                    { new Guid("7330f2ef-f86f-4b7d-8592-e7c56ef52d15"), "Gas consumption and production", "Consumption and Production", new Guid("586c321c-3f01-4cd2-8228-000670114e32") },
                    { new Guid("334740dd-7158-475d-84e9-c8e83403b515"), "Gas storage", "Storage", new Guid("586c321c-3f01-4cd2-8228-000670114e32") },
                    { new Guid("1653a169-9548-46f6-aa47-ea9a90088b0f"), "Gas CO2 Emissions", "CO2 Emissions", new Guid("586c321c-3f01-4cd2-8228-000670114e32") },
                    { new Guid("077a87f9-1842-40b4-b132-1c966b41d761"), "Weather historical data", "Historical Data", new Guid("596203b3-743a-4482-8b20-f4bc6eb844b6") }
                });

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("64564746-b34b-4416-a412-df3503bf45db"),
                column: "MemberRole",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"),
                column: "MemberRole",
                value: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Hierarchy",
                keyColumn: "Id",
                keyValue: new Guid("077a87f9-1842-40b4-b132-1c966b41d761"));

            migrationBuilder.DeleteData(
                table: "Hierarchy",
                keyColumn: "Id",
                keyValue: new Guid("1653a169-9548-46f6-aa47-ea9a90088b0f"));

            migrationBuilder.DeleteData(
                table: "Hierarchy",
                keyColumn: "Id",
                keyValue: new Guid("276af4b1-72ea-4654-8ce3-37e73b2657c0"));

            migrationBuilder.DeleteData(
                table: "Hierarchy",
                keyColumn: "Id",
                keyValue: new Guid("334740dd-7158-475d-84e9-c8e83403b515"));

            migrationBuilder.DeleteData(
                table: "Hierarchy",
                keyColumn: "Id",
                keyValue: new Guid("49ca837b-79ba-4a4f-bbc5-e01a8f23befd"));

            migrationBuilder.DeleteData(
                table: "Hierarchy",
                keyColumn: "Id",
                keyValue: new Guid("5f2d33fc-decc-4061-af3c-5d58e65bbc9c"));

            migrationBuilder.DeleteData(
                table: "Hierarchy",
                keyColumn: "Id",
                keyValue: new Guid("7330f2ef-f86f-4b7d-8592-e7c56ef52d15"));

            migrationBuilder.DeleteData(
                table: "Hierarchy",
                keyColumn: "Id",
                keyValue: new Guid("a6b40fdf-9831-49ec-87b0-cfed8f447577"));

            migrationBuilder.DeleteData(
                table: "Hierarchy",
                keyColumn: "Id",
                keyValue: new Guid("a9a780d7-8f2c-4d86-b863-1327bd264575"));

            migrationBuilder.DeleteData(
                table: "Hierarchy",
                keyColumn: "Id",
                keyValue: new Guid("d27b19d3-e589-4f98-809a-351a062635e8"));

            migrationBuilder.DeleteData(
                table: "Hierarchy",
                keyColumn: "Id",
                keyValue: new Guid("ded03663-ec2e-445f-b897-b5b78c00f2d4"));

            migrationBuilder.DeleteData(
                table: "Hierarchy",
                keyColumn: "Id",
                keyValue: new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"));

            migrationBuilder.DeleteData(
                table: "Hierarchy",
                keyColumn: "Id",
                keyValue: new Guid("fb668c70-219c-4edf-b31a-7ad36ee43d88"));

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("64564746-b34b-4416-a412-df3503bf45db"),
                column: "MemberRole",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"),
                column: "MemberRole",
                value: 1);
        }
    }
}
