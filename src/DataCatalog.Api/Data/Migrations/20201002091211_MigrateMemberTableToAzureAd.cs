using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DataCatalog.Api.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class MigrateMemberTableToAzureAd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                table: "Member",
                name: "Name"
            );

            migrationBuilder.DropColumn(
                table: "Member",
                name: "Email"
            );

            migrationBuilder.DropColumn(
                table: "Member",
                name: "MemberRole"
            );

            migrationBuilder.DropColumn(
                table: "Member",
                name: "Password"
            );

            migrationBuilder.AddColumn<Guid>(
                table: "Member",
                name: "IdentityProviderId",
                defaultValue: "75030760-f7f8-40d8-a1ab-718bcb7327b7"
            );

            migrationBuilder.AddForeignKey(
                table: "Member",
                column: "IdentityProviderId",
                name: "FK_Member_IdentityProvider_IdentityProviderId",
                principalTable: "IdentityProvider",
                principalColumn: "Id"
            );

            migrationBuilder.AddColumn<string>(
                table: "Member",
                name: "ExternalId",
                maxLength: 1024,
                defaultValue: "not mapped"
            );

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: "327BF4E6-BCCB-4702-8966-0F7AFEFDD617",
                column: "ExternalId",
                value: "ba298fec-81f1-44d7-8e02-4110de0af7f6"
                );

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: "A2EDFE21-83D9-4FB1-9365-101F4F0D38C3",
                column: "ExternalId",
                value: "780e6b40-2a37-445b-b2e3-63e40df59e9d"
                );

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: "A5CC7E91-CFA4-4FEC-B70F-715978162561",
                column: "ExternalId",
                value: "9eb1e082-daac-4ad2-add5-64fa99e12dd4"
                );

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: "2FE99CAD-BEE1-4B48-9336-A793EBDC6E92",
                column: "ExternalId",
                value: "a6cc9ff7-45c1-4043-8532-076e0f829533"
                );

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: "905984A3-0E70-4FF5-85BE-6325B3D1BAB8",
                column: "ExternalId",
                value: "not mapped - " + Guid.NewGuid().ToString()
            );

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: "8C30DEE9-AF3F-4BD1-BCA4-641492E7690B",
                column: "ExternalId",
                value: "not mapped - " + Guid.NewGuid().ToString()
            );

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: "64564746-B34B-4416-A412-DF3503BF45DB",
                column: "ExternalId",
                value: "not mapped - " + Guid.NewGuid().ToString()
            );

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: "D81769D1-B643-41E6-AD5A-EC6B86F7E608",
                column: "ExternalId",
                value: "not mapped - " + Guid.NewGuid().ToString()
            );

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: "1755f50d-ff77-43ff-bd31-dc6df11b46e8",
                column: "ExternalId",
                value: "c33f245a-be24-4377-8d43-c473973e6ef0"
            );

            migrationBuilder.UpdateData(
                table: "Member",
                keyColumn: "Id",
                keyValue: "b62c4f4b-427e-4de6-b8d8-5c46e5265a64",
                column: "ExternalId",
                value: "7fa01c78-dad1-4046-902b-beb4a2ac490a"
            );

            migrationBuilder.UpdateData(
                 table: "Member",
                 keyColumn: "Id",
                 keyValue: "68dfdbe8-5ea1-4912-b4b1-108999c3f8f0",
                 column: "ExternalId",
                value: "106b38d8-b334-4fe7-a252-3b48ab625b68"
            );

            migrationBuilder.AddUniqueConstraint(
               table: "Member",
               name: "UQ_Member_IdentityProviderId_ExternalId",
               columns: new string[]
               {
                   "IdentityProviderId", "ExternalId"
               }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                table: "Member",
                name: "UQ_Member_IdentityProviderId_ExternalId"
            );

            migrationBuilder.DropForeignKey(
                table: "Member",
                name: "FK_Member_IdentityProvider_IdentityProviderId"
            );
            
            migrationBuilder.DropColumn(
                table: "Member",
                name: "IdentityProviderId"
            );

            migrationBuilder.AddColumn<string>(
                table: "Member",
                name: "Name"
            );

            migrationBuilder.AddColumn<string>(
                table: "Member",
                name: "Email"
            );

            migrationBuilder.AddColumn<int>(
                table: "Member",
                name: "MemberRole"
            );

            migrationBuilder.AddColumn<int>(
                table: "Member",
                name: "Password"
            );
        }
    }
}
