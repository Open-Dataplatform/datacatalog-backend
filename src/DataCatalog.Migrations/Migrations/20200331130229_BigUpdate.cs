using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Api.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class BigUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "FrequencyDescription",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "Resolution",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "ResolutionDescription",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "SLA",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "VersionDate",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "Access",
                table: "DataField");

            migrationBuilder.AddColumn<Guid>(
                name: "ConfidentialityId",
                table: "Dataset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IsArchived",
                table: "Dataset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SlaDescription",
                table: "Dataset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SlaLink",
                table: "Dataset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourceId",
                table: "Dataset",
                nullable: false,
                defaultValue: new Guid("5777efb5-f55c-4f16-bb83-0125b810f995"));

            migrationBuilder.AddColumn<Guid>(
                name: "ConfidentialityId",
                table: "DataField",
                nullable: false,
                defaultValue: new Guid("62c544b2-ee11-4434-b9b7-39ffb1ab9616"));

            migrationBuilder.AddColumn<string>(
                name: "ImageUri",
                table: "Category",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Confidentiality",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Confidentiality", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DatasetChangeLog",
                columns: table => new
                {
                    DatasetId = table.Column<Guid>(nullable: false),
                    MemberId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetChangeLog", x => new { x.DatasetId, x.MemberId });
                    table.ForeignKey(
                        name: "FK_DatasetChangeLog_Dataset_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Dataset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DatasetChangeLog_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Duration",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    Code = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Duration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Source",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    Name = table.Column<string>(nullable: false),
                    ContactInfo = table.Column<string>(nullable: false),
                    SourceType = table.Column<int>(nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Source", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DatasetDuration",
                columns: table => new
                {
                    DatasetId = table.Column<Guid>(nullable: false),
                    DurationId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    DurationType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetDuration", x => new { x.DatasetId, x.DurationId });
                    table.ForeignKey(
                        name: "FK_DatasetDuration_Dataset_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Dataset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DatasetDuration_Duration_DurationId",
                        column: x => x.DurationId,
                        principalTable: "Duration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Confidentiality",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("62c544b2-ee11-4434-b9b7-39ffb1ab9616"), "Freely available", "Nonconfidential" },
                    { new Guid("5cf6fe55-13f4-4503-8ae0-394f6e09bf3d"), "Sensitive, but available for work-related tasks", "Confidential" },
                    { new Guid("568f7357-fce8-4733-b15d-2e0c4c3d8bd6"), "Only available with special clearance", "Strictly confidential" }
                });

            migrationBuilder.InsertData(
                table: "Duration",
                columns: new[] { "Id", "Code", "Description" },
                values: new object[] { new Guid("f745f308-1542-4a7a-975f-0cd6f1e73668"), "P1H", "Every hour" });

            migrationBuilder.InsertData(
                table: "Member",
                columns: new[] { "Id", "Email", "MemberRoleId", "Name" },
                values: new object[] { new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"), "dummy@fake.com", new Guid("2375e551-d07a-4417-8357-6d8784474273"), "Dummy Steward" });

            migrationBuilder.InsertData(
                table: "Source",
                columns: new[] { "Id", "ContactInfo", "Name" },
                values: new object[] { new Guid("5777efb5-f55c-4f16-bb83-0125b810f995"), "dataplatform@energinet.dk", "Data Platform" });

            migrationBuilder.CreateIndex(
                name: "IX_Dataset_ConfidentialityId",
                table: "Dataset",
                column: "ConfidentialityId");

            migrationBuilder.CreateIndex(
                name: "IX_Dataset_SourceId",
                table: "Dataset",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_DataField_ConfidentialityId",
                table: "DataField",
                column: "ConfidentialityId");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetChangeLog_MemberId",
                table: "DatasetChangeLog",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetDuration_DurationId",
                table: "DatasetDuration",
                column: "DurationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DataField_Confidentiality_ConfidentialityId",
                table: "DataField",
                column: "ConfidentialityId",
                principalTable: "Confidentiality",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dataset_Confidentiality_ConfidentialityId",
                table: "Dataset",
                column: "ConfidentialityId",
                principalTable: "Confidentiality",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Dataset_Source_SourceId",
                table: "Dataset",
                column: "SourceId",
                principalTable: "Source",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataField_Confidentiality_ConfidentialityId",
                table: "DataField");

            migrationBuilder.DropForeignKey(
                name: "FK_Dataset_Confidentiality_ConfidentialityId",
                table: "Dataset");

            migrationBuilder.DropForeignKey(
                name: "FK_Dataset_Source_SourceId",
                table: "Dataset");

            migrationBuilder.DropTable(
                name: "Confidentiality");

            migrationBuilder.DropTable(
                name: "DatasetChangeLog");

            migrationBuilder.DropTable(
                name: "DatasetDuration");

            migrationBuilder.DropTable(
                name: "Source");

            migrationBuilder.DropTable(
                name: "Duration");

            migrationBuilder.DropIndex(
                name: "IX_Dataset_ConfidentialityId",
                table: "Dataset");

            migrationBuilder.DropIndex(
                name: "IX_Dataset_SourceId",
                table: "Dataset");

            migrationBuilder.DropIndex(
                name: "IX_DataField_ConfidentialityId",
                table: "DataField");

            migrationBuilder.DeleteData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"));

            migrationBuilder.DropColumn(
                name: "ConfidentialityId",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "SlaDescription",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "SlaLink",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "ConfidentialityId",
                table: "DataField");

            migrationBuilder.DropColumn(
                name: "ImageUri",
                table: "Category");

            migrationBuilder.AddColumn<string>(
                name: "Frequency",
                table: "Dataset",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FrequencyDescription",
                table: "Dataset",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Resolution",
                table: "Dataset",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResolutionDescription",
                table: "Dataset",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SLA",
                table: "Dataset",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Dataset",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "Dataset",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VersionDate",
                table: "Dataset",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Access",
                table: "DataField",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("084e6353-82a8-4f99-b9f3-b247081c34c8"),
                column: "CreatedDate",
                value: new DateTime(2020, 3, 30, 9, 46, 42, 44, DateTimeKind.Utc).AddTicks(7316));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("08e58f12-51bf-4af9-af3a-02022cab77c7"),
                column: "CreatedDate",
                value: new DateTime(2020, 3, 30, 9, 46, 42, 44, DateTimeKind.Utc).AddTicks(7302));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("23ab1cd5-ae16-4aa4-bab7-25380c1634d5"),
                column: "CreatedDate",
                value: new DateTime(2020, 3, 30, 9, 46, 42, 44, DateTimeKind.Utc).AddTicks(7305));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("418e657d-3efb-4a30-ad3f-3f95a6ab8888"),
                column: "CreatedDate",
                value: new DateTime(2020, 3, 30, 9, 46, 42, 44, DateTimeKind.Utc).AddTicks(7457));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("5873b2e5-5db1-42db-bf25-83fd9dded293"),
                column: "CreatedDate",
                value: new DateTime(2020, 3, 30, 9, 46, 42, 44, DateTimeKind.Utc).AddTicks(7312));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("5b39fbdd-76c5-44df-9f7d-d314db760fc1"),
                column: "CreatedDate",
                value: new DateTime(2020, 3, 30, 9, 46, 42, 44, DateTimeKind.Utc).AddTicks(7319));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("7434d8a3-7ebb-4212-967c-061d5a2666d7"),
                column: "CreatedDate",
                value: new DateTime(2020, 3, 30, 9, 46, 42, 44, DateTimeKind.Utc).AddTicks(7443));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("8713b259-0294-480a-960c-08a9c9983961"),
                column: "CreatedDate",
                value: new DateTime(2020, 3, 30, 9, 46, 42, 44, DateTimeKind.Utc).AddTicks(4819));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("8a95d290-417e-4a53-a807-a13293f3117d"),
                column: "CreatedDate",
                value: new DateTime(2020, 3, 30, 9, 46, 42, 44, DateTimeKind.Utc).AddTicks(7309));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("90909fcd-4a5a-455a-a543-9c2c8a45e453"),
                column: "CreatedDate",
                value: new DateTime(2020, 3, 30, 9, 46, 42, 44, DateTimeKind.Utc).AddTicks(7447));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("9ac5af8d-4165-448a-ace5-2edcf0c3e202"),
                column: "CreatedDate",
                value: new DateTime(2020, 3, 30, 9, 46, 42, 44, DateTimeKind.Utc).AddTicks(7439));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("a90abc39-c2de-4c29-a9f8-4e10f41a50f0"),
                column: "CreatedDate",
                value: new DateTime(2020, 3, 30, 9, 46, 42, 44, DateTimeKind.Utc).AddTicks(7323));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("b4cef11a-d0f4-4c33-9796-b80ea6d2489a"),
                column: "CreatedDate",
                value: new DateTime(2020, 3, 30, 9, 46, 42, 44, DateTimeKind.Utc).AddTicks(7454));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("b94863a9-685a-44d6-aae4-2cfb3a63c489"),
                column: "CreatedDate",
                value: new DateTime(2020, 3, 30, 9, 46, 42, 44, DateTimeKind.Utc).AddTicks(7256));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("d6944781-c069-4c14-859f-07865164763f"),
                column: "CreatedDate",
                value: new DateTime(2020, 3, 30, 9, 46, 42, 44, DateTimeKind.Utc).AddTicks(7454));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("e5b2f760-e965-452f-b0ab-70ca506d4c20"),
                column: "CreatedDate",
                value: new DateTime(2020, 3, 30, 9, 46, 42, 44, DateTimeKind.Utc).AddTicks(7298));

            migrationBuilder.UpdateData(
                table: "MemberRole",
                keyColumn: "Id",
                keyValue: new Guid("2375e551-d07a-4417-8357-6d8784474273"),
                column: "CreatedDate",
                value: new DateTime(2020, 3, 30, 9, 46, 42, 46, DateTimeKind.Utc).AddTicks(4637));

            migrationBuilder.UpdateData(
                table: "MemberRole",
                keyColumn: "Id",
                keyValue: new Guid("a32b86d0-1646-4ca7-b79e-e205b0d15868"),
                column: "CreatedDate",
                value: new DateTime(2020, 3, 30, 9, 46, 42, 46, DateTimeKind.Utc).AddTicks(3733));

            migrationBuilder.UpdateData(
                table: "MemberRole",
                keyColumn: "Id",
                keyValue: new Guid("f611f697-4deb-4cc6-901d-41dd76346359"),
                column: "CreatedDate",
                value: new DateTime(2020, 3, 30, 9, 46, 42, 46, DateTimeKind.Utc).AddTicks(4665));
        }
    }
}
