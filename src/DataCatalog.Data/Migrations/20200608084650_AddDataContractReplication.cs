using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddDataContractReplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginEnvironment",
                table: "DataContract",
                nullable: true);

            migrationBuilder.UpdateData("DataContract", "Id", "3fb7650e-4f4d-4033-8746-618e28f06cc6", "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("DataContract", "Id", "3700071e-0899-4161-a730-36b21cdd8660", "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("DataContract", "Id", "9f609764-32cf-426a-8943-4d7b3ef2469b", "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("DataContract", "Id", "51b1f28e-5406-4c4d-ba3c-161f8ecb4010", "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("DataContract", "Id", "e607cc4f-ddbb-465c-b041-047c065e898b", "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("DataContract", "Id", "a9d1aadd-9915-4884-9659-40d0cc63e410", "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("DataContract", "Id", "922142e8-07cc-468b-8b59-e8b93981a1e2", "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("DataContract", "Id", "3f34b30d-93e8-4505-ad03-9362714e2113", "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("DataContract", "Id", "81f8160d-7ac4-4e26-99da-e5188f0bee4a", "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("DataContract", "Id", "cf70172d-c271-4607-a04e-a17b34044d6b", "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("DataContract", "Id", "34ab15a6-0bb1-40b0-9407-3971f41a0c68", "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("DataContract", "Id", "ea425ed9-7130-48cb-8464-edb4cbb5ebc2", "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("DataContract", "Id", "7a534e40-97f3-4d6c-aff5-b443ba06b13e", "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("DataContract", "Id", "ddacd6e0-4a26-4793-8c14-8bc843315963", "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("DataContract", "Id", "f1e409c1-81d0-4c2c-97a3-bfd8bbe22aeb", "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("DataContract", "Id", "899c4fef-892e-4a91-8191-42e7f970f58b", "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("DataContract", "Id", "21389a17-bf24-4be8-ab38-5d8f8a012488", "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("DataContract", "Id", "c7f1cb06-2441-4c3f-b4e2-d02cf4cc6b56", "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("DataContract", "Id", "e52f8eb1-9ae8-40e1-acc2-052ec8e578ff", "OriginEnvironment", "prod");
            migrationBuilder.UpdateData("DataContract", "Id", "b9e55105-16d2-4ecf-9633-1c221d5abc82", "OriginEnvironment", "prod");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginEnvironment",
                table: "DataContract");
        }
    }
}
