using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Data.Migrations
{
    public partial class AddedServiceLevelAgreementAsSeparateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServiceLevelAgreementId",
                table: "Dataset",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServiceLevelAgreement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLevelAgreement", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dataset_ServiceLevelAgreementId",
                table: "Dataset",
                column: "ServiceLevelAgreementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dataset_ServiceLevelAgreement_ServiceLevelAgreementId",
                table: "Dataset",
                column: "ServiceLevelAgreementId",
                principalTable: "ServiceLevelAgreement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dataset_ServiceLevelAgreement_ServiceLevelAgreementId",
                table: "Dataset");

            migrationBuilder.DropTable(
                name: "ServiceLevelAgreement");

            migrationBuilder.DropIndex(
                name: "IX_Dataset_ServiceLevelAgreementId",
                table: "Dataset");

            migrationBuilder.DropColumn(
                name: "ServiceLevelAgreementId",
                table: "Dataset");
        }
    }
}
