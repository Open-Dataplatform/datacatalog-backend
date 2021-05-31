using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class LinkDatasetWithContact : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dataset_MemberGroup_StewardId1",
                table: "Dataset");

            migrationBuilder.DropIndex(
                name: "IX_Dataset_StewardId1",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "StewardId",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "StewardId1",
                table: "Dataset");

            migrationBuilder.AddColumn<Guid>(
                name: "ContactId",
                table: "Dataset",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Dataset_ContactId",
                table: "Dataset",
                column: "ContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dataset_MemberGroup_ContactId",
                table: "Dataset",
                column: "ContactId",
                principalTable: "MemberGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dataset_MemberGroup_ContactId",
                table: "Dataset");

            migrationBuilder.DropIndex(
                name: "IX_Dataset_ContactId",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "Dataset");

            migrationBuilder.AddColumn<int>(
                name: "StewardId",
                table: "Dataset",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "StewardId1",
                table: "Dataset",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dataset_StewardId1",
                table: "Dataset",
                column: "StewardId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Dataset_MemberGroup_StewardId1",
                table: "Dataset",
                column: "StewardId1",
                principalTable: "MemberGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
