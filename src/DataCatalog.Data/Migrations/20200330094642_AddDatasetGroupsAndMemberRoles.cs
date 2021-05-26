using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddDatasetGroupsAndMemberRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TransformationDataset",
                nullable: false,
                defaultValueSql: "getutcdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Transformation",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Transformation",
                nullable: false,
                defaultValueSql: "getutcdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Transformation",
                nullable: false,
                defaultValueSql: "getutcdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "MemberGroupMember",
                nullable: false,
                defaultValueSql: "getutcdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MemberGroup",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "MemberGroup",
                nullable: false,
                defaultValueSql: "getutcdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "MemberGroup",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "MemberGroup",
                nullable: false,
                defaultValueSql: "getutcdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Member",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Member",
                nullable: false,
                defaultValueSql: "getutcdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Member",
                nullable: false,
                defaultValueSql: "getutcdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Member",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "MemberRoleId",
                table: "Member",
                nullable: false,
                defaultValue: new Guid("f611f697-4deb-4cc6-901d-41dd76346359"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "DatasetCategory",
                nullable: false,
                defaultValueSql: "getutcdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Dataset",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Dataset",
                nullable: false,
                defaultValueSql: "getutcdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Dataset",
                nullable: false,
                defaultValueSql: "getutcdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "DataField",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DataField",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "DataField",
                nullable: false,
                defaultValueSql: "getutcdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "DataField",
                nullable: false,
                defaultValueSql: "getutcdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Category",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Category",
                nullable: false,
                defaultValueSql: "getutcdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Category",
                nullable: false,
                defaultValueSql: "getutcdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Colour",
                table: "Category",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DatasetGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    MemberId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DatasetGroup_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberRole",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DatasetGroupDataset",
                columns: table => new
                {
                    DatasetGroupId = table.Column<Guid>(nullable: false),
                    DatasetId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetGroupDataset", x => new { x.DatasetGroupId, x.DatasetId });
                    table.ForeignKey(
                        name: "FK_DatasetGroupDataset_DatasetGroup_DatasetGroupId",
                        column: x => x.DatasetGroupId,
                        principalTable: "DatasetGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DatasetGroupDataset_Dataset_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Dataset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.InsertData(
                table: "MemberRole",
                columns: new[] { "Id", "CreatedDate", "Name" },
                values: new object[,]
                {
                    { new Guid("f611f697-4deb-4cc6-901d-41dd76346359"), new DateTime(2020, 3, 30, 9, 46, 42, 46, DateTimeKind.Utc).AddTicks(4665), "User" },
                    { new Guid("2375e551-d07a-4417-8357-6d8784474273"), new DateTime(2020, 3, 30, 9, 46, 42, 46, DateTimeKind.Utc).AddTicks(4637), "Data steward" },
                    { new Guid("a32b86d0-1646-4ca7-b79e-e205b0d15868"), new DateTime(2020, 3, 30, 9, 46, 42, 46, DateTimeKind.Utc).AddTicks(3733), "Admin" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Member_MemberRoleId",
                table: "Member",
                column: "MemberRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetGroup_MemberId",
                table: "DatasetGroup",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetGroupDataset_DatasetId",
                table: "DatasetGroupDataset",
                column: "DatasetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Member_MemberRole_MemberRoleId",
                table: "Member",
                column: "MemberRoleId",
                principalTable: "MemberRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Member_MemberRole_MemberRoleId",
                table: "Member");

            migrationBuilder.DropTable(
                name: "DatasetGroupDataset");

            migrationBuilder.DropTable(
                name: "MemberRole");

            migrationBuilder.DropTable(
                name: "DatasetGroup");

            migrationBuilder.DropIndex(
                name: "IX_Member_MemberRoleId",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "MemberRoleId",
                table: "Member");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TransformationDataset",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getutcdate()");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Transformation",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Transformation",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getutcdate()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Transformation",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getutcdate()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "MemberGroupMember",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getutcdate()");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MemberGroup",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "MemberGroup",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getutcdate()");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "MemberGroup",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "MemberGroup",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getutcdate()");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Member",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Member",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getutcdate()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Member",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getutcdate()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "DatasetCategory",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getutcdate()");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Dataset",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Dataset",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getutcdate()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Dataset",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getutcdate()");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "DataField",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DataField",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "DataField",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getutcdate()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "DataField",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getutcdate()");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Category",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Category",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getutcdate()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Category",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "getutcdate()");

            migrationBuilder.AlterColumn<string>(
                name: "Colour",
                table: "Category",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("084e6353-82a8-4f99-b9f3-b247081c34c8"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6943), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6943) });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("08e58f12-51bf-4af9-af3a-02022cab77c7"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6900), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6900) });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("23ab1cd5-ae16-4aa4-bab7-25380c1634d5"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6904), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6904) });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("418e657d-3efb-4a30-ad3f-3f95a6ab8888"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6978), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6978) });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("5873b2e5-5db1-42db-bf25-83fd9dded293"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6939), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6939) });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("5b39fbdd-76c5-44df-9f7d-d314db760fc1"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6950), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6950) });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("7434d8a3-7ebb-4212-967c-061d5a2666d7"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6960), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6960) });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("8713b259-0294-480a-960c-08a9c9983961"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(4390), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(3411) });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("8a95d290-417e-4a53-a807-a13293f3117d"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6911), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6911) });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("90909fcd-4a5a-455a-a543-9c2c8a45e453"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6964), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6964) });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("9ac5af8d-4165-448a-ace5-2edcf0c3e202"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6957), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6957) });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("a90abc39-c2de-4c29-a9f8-4e10f41a50f0"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6953), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6953) });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("b4cef11a-d0f4-4c33-9796-b80ea6d2489a"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6974), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6974) });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("b94863a9-685a-44d6-aae4-2cfb3a63c489"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6851), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6844) });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("d6944781-c069-4c14-859f-07865164763f"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6967), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6967) });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("e5b2f760-e965-452f-b0ab-70ca506d4c20"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6893), new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6893) });
        }
    }
}
