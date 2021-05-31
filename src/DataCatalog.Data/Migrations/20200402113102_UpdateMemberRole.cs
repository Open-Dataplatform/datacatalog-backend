using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class UpdateMemberRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Member_MemberRole_MemberRoleId",
                table: "Member");

            migrationBuilder.DropTable(
                name: "MemberRole");

            migrationBuilder.DropIndex(
                name: "IX_Member_MemberRoleId",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "MemberRoleId",
                table: "Member");

            migrationBuilder.AddColumn<int>(
                name: "MemberRole",
                table: "Member",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"),
                column: "MemberRole",
                value: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberRole",
                table: "Member");

            migrationBuilder.AddColumn<Guid>(
                name: "MemberRoleId",
                table: "Member",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("f611f697-4deb-4cc6-901d-41dd76346359"));

            migrationBuilder.CreateTable(
                name: "MemberRole",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberRole", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "MemberRole",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("a32b86d0-1646-4ca7-b79e-e205b0d15868"), "Admin" });

            migrationBuilder.InsertData(
                table: "MemberRole",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("2375e551-d07a-4417-8357-6d8784474273"), "Data steward" });

            migrationBuilder.InsertData(
                table: "MemberRole",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("f611f697-4deb-4cc6-901d-41dd76346359"), "User" });

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"),
                column: "MemberRoleId",
                value: new Guid("2375e551-d07a-4417-8357-6d8784474273"));

            migrationBuilder.CreateIndex(
                name: "IX_Member_MemberRoleId",
                table: "Member",
                column: "MemberRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Member_MemberRole_MemberRoleId",
                table: "Member",
                column: "MemberRoleId",
                principalTable: "MemberRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
