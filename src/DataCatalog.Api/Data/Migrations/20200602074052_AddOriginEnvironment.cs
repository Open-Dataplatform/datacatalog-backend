using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DataCatalog.Api.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddOriginEnvironment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginEnvironment",
                table: "Dataset",
                nullable: true);

            migrationBuilder.UpdateData("Dataset", "Id", new Guid("913cd17d-f378-49bc-b96b-0bf28b31712d"), "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("Dataset", "Id", new Guid("38774081-f7d3-4c79-b2f3-1580d86c3bf5"), "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("Dataset", "Id", new Guid("a8ba149d-15ff-45bb-b910-cdb639d40a20"), "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("Dataset", "Id", new Guid("768890fc-cd26-4d53-a2ef-0dd51a5fc821"), "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("Dataset", "Id", new Guid("04476b3c-fa63-4034-bc19-3808c07fb0b4"), "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("Dataset", "Id", new Guid("ec93f72c-2693-44ed-ab8c-1199ce7051b6"), "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("Dataset", "Id", new Guid("d396f455-310e-4221-b2ec-c4666d4fa572"), "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("Dataset", "Id", new Guid("a949bcc8-d8f7-4aec-870b-5a10c2808b3b"), "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("Dataset", "Id", new Guid("d05be7e3-fa3a-4124-aed8-a8009fe466c5"), "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("Dataset", "Id", new Guid("23949217-0f4b-425b-94b9-e80725054b11"), "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("Dataset", "Id", new Guid("bdd2c5a5-720e-4e1d-8cfc-16ecfe924b5e"), "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("Dataset", "Id", new Guid("538e9138-f2c9-4c92-bca6-dc33d05d9419"), "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("Dataset", "Id", new Guid("9b181dcf-1062-4906-9efd-ebabd73e18b1"), "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("Dataset", "Id", new Guid("e287af45-a2fe-4210-9634-05fa7a160077"), "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("Dataset", "Id", new Guid("4556afbe-a7ee-4e02-bebd-9f3cbbb6f7e4"), "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("Dataset", "Id", new Guid("3c19ee43-3b3a-43bd-9b0d-101436bad4d7"), "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("Dataset", "Id", new Guid("7c10db2a-ed65-4ff5-b9c7-8aba184941ca"), "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("Dataset", "Id", new Guid("0d43e440-5306-4cc9-94f4-37559cf304e0"), "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("Dataset", "Id", new Guid("b946363a-1b9c-4f59-b8dd-35c93f42c9ab"), "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("Dataset", "Id", new Guid("ed32e705-65ea-4cf8-be34-3a5df2e568d4"), "OriginEnvironment", "prod");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginEnvironment",
                table: "Dataset");
        }
    }
}