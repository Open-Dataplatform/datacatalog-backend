using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class DataContractAndLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HierarchyId",
                table: "Dataset",
                nullable: false,
                defaultValue: new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0"));

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Dataset",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DataSource",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ContactInfo = table.Column<string>(nullable: true),
                    SourceType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hierarchy",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ParentHierarchyId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hierarchy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hierarchy_Hierarchy_ParentHierarchyId",
                        column: x => x.ParentHierarchyId,
                        principalTable: "Hierarchy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DataContract",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    DatasetId = table.Column<Guid>(nullable: false),
                    DataSourceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataContract", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataContract_DataSource_DataSourceId",
                        column: x => x.DataSourceId,
                        principalTable: "DataSource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DataContract_Dataset_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Dataset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DataSource",
                columns: new[] { "Id", "ContactInfo", "Description", "Name", "SourceType" },
                values: new object[,]
                {
                    { new Guid("14c7e740-6104-4036-af76-73891fdf0033"), "dataplatform@energinet.dk", "Use for any dataset that originates from within the data platform itself", "DataPlatform", 0 },
                    { new Guid("76d869e6-254f-40e3-8212-91e31cac1f7e"), "dps@energinet.dk", "Energinet system DPS", "DPS", 1 },
                    { new Guid("bec46f78-9ef7-49f1-b98b-748f85aa823b"), "dmi.dk/kontakt", "Danish Meteorological Institute", "DMI", 2 }
                });

            migrationBuilder.InsertData(
                table: "Hierarchy",
                columns: new[] { "Id", "Description", "Name", "ParentHierarchyId" },
                values: new object[,]
                {
                    { new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0"), "Anything mainly related to electricity", "Electricity", null },
                    { new Guid("586c321c-3f01-4cd2-8228-000670114e32"), "Anything mainly related to gas", "Gas", null },
                    { new Guid("596203b3-743a-4482-8b20-f4bc6eb844b6"), "Anything mainly related to weather", "Weather", null }
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

            migrationBuilder.InsertData(
                table: "Hierarchy",
                columns: new[] { "Id", "Description", "Name", "ParentHierarchyId" },
                values: new object[] { new Guid("ad240fb2-ca95-4ec0-9bc2-58c73bc4ac31"), "Electricity consumption", "Consumption", new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") });

            migrationBuilder.InsertData(
                table: "Hierarchy",
                columns: new[] { "Id", "Description", "Name", "ParentHierarchyId" },
                values: new object[] { new Guid("5b22fc05-6837-40f4-a8a2-184e9aa25aff"), "Gas consumption", "Consumption", new Guid("586c321c-3f01-4cd2-8228-000670114e32") });

            migrationBuilder.InsertData(
                table: "Hierarchy",
                columns: new[] { "Id", "Description", "Name", "ParentHierarchyId" },
                values: new object[] { new Guid("a8adde4d-4dc9-4028-8993-0ef9bc153d7d"), "Weather forecasts", "Forecasts", new Guid("596203b3-743a-4482-8b20-f4bc6eb844b6") });

            migrationBuilder.CreateIndex(
                name: "IX_Dataset_HierarchyId",
                table: "Dataset",
                column: "HierarchyId");

            migrationBuilder.CreateIndex(
                name: "IX_DataContract_DataSourceId",
                table: "DataContract",
                column: "DataSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_DataContract_DatasetId",
                table: "DataContract",
                column: "DatasetId");

            migrationBuilder.CreateIndex(
                name: "IX_Hierarchy_ParentHierarchyId",
                table: "Hierarchy",
                column: "ParentHierarchyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dataset_Hierarchy_HierarchyId",
                table: "Dataset",
                column: "HierarchyId",
                principalTable: "Hierarchy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dataset_Hierarchy_HierarchyId",
                table: "Dataset");

            migrationBuilder.DropTable(
                name: "DataContract");

            migrationBuilder.DropTable(
                name: "Hierarchy");

            migrationBuilder.DropTable(
                name: "DataSource");

            migrationBuilder.DropIndex(
                name: "IX_Dataset_HierarchyId",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "HierarchyId",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Dataset");

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
