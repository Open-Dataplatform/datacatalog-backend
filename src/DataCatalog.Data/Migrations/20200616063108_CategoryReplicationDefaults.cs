using System;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

namespace DataCatalog.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class CategoryReplicationDefaults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("084e6353-82a8-4f99-b9f3-b247081c34c8"),
                column: "OriginEnvironment",
                value: "prod");

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("08e58f12-51bf-4af9-af3a-02022cab77c7"),
                column: "OriginEnvironment",
                value: "prod");

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("23ab1cd5-ae16-4aa4-bab7-25380c1634d5"),
                column: "OriginEnvironment",
                value: "prod");

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("418e657d-3efb-4a30-ad3f-3f95a6ab8888"),
                column: "OriginEnvironment",
                value: "prod");

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("5873b2e5-5db1-42db-bf25-83fd9dded293"),
                column: "OriginEnvironment",
                value: "prod");

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("5b39fbdd-76c5-44df-9f7d-d314db760fc1"),
                column: "OriginEnvironment",
                value: "prod");

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("7434d8a3-7ebb-4212-967c-061d5a2666d7"),
                column: "OriginEnvironment",
                value: "prod");

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("8713b259-0294-480a-960c-08a9c9983961"),
                column: "OriginEnvironment",
                value: "prod");

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("8a95d290-417e-4a53-a807-a13293f3117d"),
                column: "OriginEnvironment",
                value: "prod");

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("90909fcd-4a5a-455a-a543-9c2c8a45e453"),
                column: "OriginEnvironment",
                value: "prod");

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("9ac5af8d-4165-448a-ace5-2edcf0c3e202"),
                column: "OriginEnvironment",
                value: "prod");

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("a90abc39-c2de-4c29-a9f8-4e10f41a50f0"),
                column: "OriginEnvironment",
                value: "prod");

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("b4cef11a-d0f4-4c33-9796-b80ea6d2489a"),
                column: "OriginEnvironment",
                value: "prod");

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("b94863a9-685a-44d6-aae4-2cfb3a63c489"),
                column: "OriginEnvironment",
                value: "prod");

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("d6944781-c069-4c14-859f-07865164763f"),
                column: "OriginEnvironment",
                value: "prod");

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("e5b2f760-e965-452f-b0ab-70ca506d4c20"),
                column: "OriginEnvironment",
                value: "prod");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("084e6353-82a8-4f99-b9f3-b247081c34c8"),
                column: "OriginEnvironment",
                value: null);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("08e58f12-51bf-4af9-af3a-02022cab77c7"),
                column: "OriginEnvironment",
                value: null);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("23ab1cd5-ae16-4aa4-bab7-25380c1634d5"),
                column: "OriginEnvironment",
                value: null);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("418e657d-3efb-4a30-ad3f-3f95a6ab8888"),
                column: "OriginEnvironment",
                value: null);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("5873b2e5-5db1-42db-bf25-83fd9dded293"),
                column: "OriginEnvironment",
                value: null);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("5b39fbdd-76c5-44df-9f7d-d314db760fc1"),
                column: "OriginEnvironment",
                value: null);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("7434d8a3-7ebb-4212-967c-061d5a2666d7"),
                column: "OriginEnvironment",
                value: null);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("8713b259-0294-480a-960c-08a9c9983961"),
                column: "OriginEnvironment",
                value: null);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("8a95d290-417e-4a53-a807-a13293f3117d"),
                column: "OriginEnvironment",
                value: null);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("90909fcd-4a5a-455a-a543-9c2c8a45e453"),
                column: "OriginEnvironment",
                value: null);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("9ac5af8d-4165-448a-ace5-2edcf0c3e202"),
                column: "OriginEnvironment",
                value: null);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("a90abc39-c2de-4c29-a9f8-4e10f41a50f0"),
                column: "OriginEnvironment",
                value: null);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("b4cef11a-d0f4-4c33-9796-b80ea6d2489a"),
                column: "OriginEnvironment",
                value: null);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("b94863a9-685a-44d6-aae4-2cfb3a63c489"),
                column: "OriginEnvironment",
                value: null);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("d6944781-c069-4c14-859f-07865164763f"),
                column: "OriginEnvironment",
                value: null);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("e5b2f760-e965-452f-b0ab-70ca506d4c20"),
                column: "OriginEnvironment",
                value: null);
        }
    }
}
