using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DataCatalog.Api.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class CreateGTMSDataSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DataSource",
                columns: new[] { "Id", "Name", "Description", "ContactInfo", "SourceType", "OriginEnvironment", "Version", "OriginDeleted" },
                values: new object[] { new Guid("3880d4cd-64d9-4205-a688-83919a0a958b"), "GTMS", "Gas Transmission Rate Measurement System data", "mjp@energinet.dk", 1, "prod", 0, false }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DataSource",
                keyColumn: "Id",
                keyValue: new Guid("3880d4cd-64d9-4205-a688-83919a0a958b")
            );
        }
    }
}
