using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Api.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class SeedCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "Colour", "CreatedDate", "ModifiedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("8713b259-0294-480a-960c-08a9c9983961"), "#2A939B", new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(4390), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(3411), "Ancillary Services" },
                    { new Guid("b94863a9-685a-44d6-aae4-2cfb3a63c489"), "#389B88", new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6851), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6844), "Auctions, Transmission Capacity" },
                    { new Guid("e5b2f760-e965-452f-b0ab-70ca506d4c20"), "#452A9B", new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6893), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6893), "Emissions" },
                    { new Guid("08e58f12-51bf-4af9-af3a-02022cab77c7"), "#2A9B65", new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6900), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6900), "Day Ahead Market" },
                    { new Guid("23ab1cd5-ae16-4aa4-bab7-25380c1634d5"), "#B27736", new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6904), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6904), "Electric Boilers" },
                    { new Guid("8a95d290-417e-4a53-a807-a13293f3117d"), "#F8AE3C", new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6911), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6911), "Gas" },
                    { new Guid("5873b2e5-5db1-42db-bf25-83fd9dded293"), "#663BCC", new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6939), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6939), "Production and Consumption" },
                    { new Guid("084e6353-82a8-4f99-b9f3-b247081c34c8"), "#A0C1C2", new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6943), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6943), "Electricity Consumption" },
                    { new Guid("5b39fbdd-76c5-44df-9f7d-d314db760fc1"), "#819B38", new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6950), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6950), "Intra Day Market" },
                    { new Guid("a90abc39-c2de-4c29-a9f8-4e10f41a50f0"), "#293A4C", new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6953), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6953), "Electricity Production" },
                    { new Guid("9ac5af8d-4165-448a-ace5-2edcf0c3e202"), "#548E80", new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6957), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6957), "Regulating Power" },
                    { new Guid("7434d8a3-7ebb-4212-967c-061d5a2666d7"), "#7D8E1C", new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6960), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6960), "Whole Sale Market" },
                    { new Guid("90909fcd-4a5a-455a-a543-9c2c8a45e453"), "#398C22", new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6964), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6964), "Reserves" },
                    { new Guid("d6944781-c069-4c14-859f-07865164763f"), "#FFD424", new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6967), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6967), "Solar Power" },
                    { new Guid("b4cef11a-d0f4-4c33-9796-b80ea6d2489a"), "#547B8E", new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6974), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6974), "Transmission Lines" },
                    { new Guid("418e657d-3efb-4a30-ad3f-3f95a6ab8888"), "#C2E4F0", new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6978), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6978), "Wind Power" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("084e6353-82a8-4f99-b9f3-b247081c34c8"));

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("08e58f12-51bf-4af9-af3a-02022cab77c7"));

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("23ab1cd5-ae16-4aa4-bab7-25380c1634d5"));

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("418e657d-3efb-4a30-ad3f-3f95a6ab8888"));

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("5873b2e5-5db1-42db-bf25-83fd9dded293"));

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("5b39fbdd-76c5-44df-9f7d-d314db760fc1"));

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("7434d8a3-7ebb-4212-967c-061d5a2666d7"));

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("8713b259-0294-480a-960c-08a9c9983961"));

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("8a95d290-417e-4a53-a807-a13293f3117d"));

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("90909fcd-4a5a-455a-a543-9c2c8a45e453"));

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("9ac5af8d-4165-448a-ace5-2edcf0c3e202"));

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("a90abc39-c2de-4c29-a9f8-4e10f41a50f0"));

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("b4cef11a-d0f4-4c33-9796-b80ea6d2489a"));

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("b94863a9-685a-44d6-aae4-2cfb3a63c489"));

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("d6944781-c069-4c14-859f-07865164763f"));

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("e5b2f760-e965-452f-b0ab-70ca506d4c20"));
        }
    }
}
