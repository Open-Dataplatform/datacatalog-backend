using System;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

namespace DataCatalog.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class DataSourceReplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginEnvironment",
                table: "DataSource",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "DataSource",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "DataSource",
                keyColumn: "Id",
                keyValue: new Guid("14c7e740-6104-4036-af76-73891fdf0033"),
                column: "OriginEnvironment",
                value: "prod");

            migrationBuilder.UpdateData(
                table: "DataSource",
                keyColumn: "Id",
                keyValue: new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab"),
                column: "OriginEnvironment",
                value: "prod");

            migrationBuilder.UpdateData(
                table: "DataSource",
                keyColumn: "Id",
                keyValue: new Guid("43a95e77-79e6-4da1-a4ea-2d64f3587b32"),
                column: "OriginEnvironment",
                value: "prod");

            migrationBuilder.UpdateData(
                table: "DataSource",
                keyColumn: "Id",
                keyValue: new Guid("44bbd193-dd57-4221-89ac-80e3509a0c73"),
                column: "OriginEnvironment",
                value: "prod");

            migrationBuilder.UpdateData(
                table: "DataSource",
                keyColumn: "Id",
                keyValue: new Guid("bec46f78-9ef7-49f1-b98b-748f85aa823b"),
                column: "OriginEnvironment",
                value: "prod");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginEnvironment",
                table: "DataSource");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "DataSource");
        }
    }
}
