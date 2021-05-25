using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataCatalog.Api.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class SeedDatasets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DataSource",
                columns: new[] { "Id", "ContactInfo", "Description", "Name", "SourceType" },
                values: new object[,]
                {
                    { new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab"), "dps@energinet.dk", "Energinet system DPS", "DPS", 1 },
                    { new Guid("43a95e77-79e6-4da1-a4ea-2d64f3587b32"), "propel@energinet.dk", "Energinet system Propel", "Propel", 1 },
                    { new Guid("44bbd193-dd57-4221-89ac-80e3509a0c73"), "neptun@energinet.dk", "Energinet system Neptun", "Neptun", 1 }
                });

            migrationBuilder.Sql("delete from DataContract where DataSourceId = '76d869e6-254f-40e3-8212-91e31cac1f7e'");

            migrationBuilder.DeleteData(
                table: "DataSource",
                keyColumn: "Id",
                keyValue: new Guid("76d869e6-254f-40e3-8212-91e31cac1f7e"));

            migrationBuilder.InsertData(
                table: "Dataset",
                columns: new[] { "Id", "ContactId", "Description", "HierarchyId", "Location", "Name", "Owner", "SlaDescription", "SlaLink" },
                values: new object[,]
                {
                    { new Guid("ed32e705-65ea-4cf8-be34-3a5df2e568d4"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("334740dd-7158-475d-84e9-c8e83403b515"), "gas/neptun/hlnt_archive_1day", "hlnt_archive_1day", null, null, null },
                    { new Guid("b946363a-1b9c-4f59-b8dd-35c93f42c9ab"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("334740dd-7158-475d-84e9-c8e83403b515"), "gas/neptun/hlnt_archive_1hour", "hlnt_archive_1hour", null, null, null },
                    { new Guid("0d43e440-5306-4cc9-94f4-37559cf304e0"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("334740dd-7158-475d-84e9-c8e83403b515"), "gas/neptun/hlnt_archive_3min", "hlnt_archive_3min", null, null, null },
                    { new Guid("9b181dcf-1062-4906-9efd-ebabd73e18b1"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"), "electricity/reserves/mfrr-eam-activations", "mFRR EAM Activations", null, null, null },
                    { new Guid("538e9138-f2c9-4c92-bca6-dc33d05d9419"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"), "electricity/reserves/mfrr-eam-bids", "mFRR EAM Bids", null, null, null },
                    { new Guid("bdd2c5a5-720e-4e1d-8cfc-16ecfe924b5e"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"), "electricity/reserves/fcr-cm-results", "FCR CM Results", null, null, null },
                    { new Guid("23949217-0f4b-425b-94b9-e80725054b11"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"), "electricity/reserves/fcr-cm-bids", "FCR CM Bids", null, null, null },
                    { new Guid("d05be7e3-fa3a-4124-aed8-a8009fe466c5"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"), "electricity/reserves/fcr-cm-needs", "FCR CM Needs", null, null, null },
                    { new Guid("d396f455-310e-4221-b2ec-c4666d4fa572"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"), "electricity/reserves/mfrr-cm-bids", "mFRR CM Bids", null, null, null },
                    { new Guid("ec93f72c-2693-44ed-ab8c-1199ce7051b6"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"), "electricity/reserves/mfrr-cm-needs", "mFRR CM Needs", null, null, null },
                    { new Guid("04476b3c-fa63-4034-bc19-3808c07fb0b4"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("a9a780d7-8f2c-4d86-b863-1327bd264575"), "electricity/nminusone-calculations/shunt", "N-1 Shunt", null, null, null },
                    { new Guid("768890fc-cd26-4d53-a2ef-0dd51a5fc821"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("a9a780d7-8f2c-4d86-b863-1327bd264575"), "electricity/nminusone-calculations/load", "N-1 Load", null, null, null },
                    { new Guid("a8ba149d-15ff-45bb-b910-cdb639d40a20"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("a9a780d7-8f2c-4d86-b863-1327bd264575"), "electricity/nminusone-calculations/gen", "N-1 Gen", null, null, null },
                    { new Guid("38774081-f7d3-4c79-b2f3-1580d86c3bf5"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("a9a780d7-8f2c-4d86-b863-1327bd264575"), "electricity/nminusone-calculations/bus", "N-1 Bus", null, null, null },
                    { new Guid("913cd17d-f378-49bc-b96b-0bf28b31712d"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("a9a780d7-8f2c-4d86-b863-1327bd264575"), "electricity/nminusone-calculations/branch", "N-1 Branch", null, null, null },
                    { new Guid("a949bcc8-d8f7-4aec-870b-5a10c2808b3b"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"), "electricity/reserves/mfrr-cm-results", "mFRR CM Results", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Hierarchy",
                columns: new[] { "Id", "Description", "Name", "ParentHierarchyId" },
                values: new object[,]
                {
                    { new Guid("c5dfc671-dcb3-4f10-bf11-758c1d3e97be"), "Electricity capacities", "Capacities", new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                    { new Guid("fc20e468-5ab4-48db-8a0b-09e86935c3aa"), "Electricity forecasts", "Forecasts", new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") }
                });

            migrationBuilder.InsertData(
                table: "DataContract",
                columns: new[] { "Id", "DataSourceId", "DatasetId" },
                values: new object[,]
                {
                    { new Guid("3fb7650e-4f4d-4033-8746-618e28f06cc6"), new Guid("43a95e77-79e6-4da1-a4ea-2d64f3587b32"), new Guid("913cd17d-f378-49bc-b96b-0bf28b31712d") },
                    { new Guid("b9e55105-16d2-4ecf-9633-1c221d5abc82"), new Guid("44bbd193-dd57-4221-89ac-80e3509a0c73"), new Guid("ed32e705-65ea-4cf8-be34-3a5df2e568d4") },
                    { new Guid("e52f8eb1-9ae8-40e1-acc2-052ec8e578ff"), new Guid("44bbd193-dd57-4221-89ac-80e3509a0c73"), new Guid("b946363a-1b9c-4f59-b8dd-35c93f42c9ab") },
                    { new Guid("c7f1cb06-2441-4c3f-b4e2-d02cf4cc6b56"), new Guid("44bbd193-dd57-4221-89ac-80e3509a0c73"), new Guid("0d43e440-5306-4cc9-94f4-37559cf304e0") },
                    { new Guid("7a534e40-97f3-4d6c-aff5-b443ba06b13e"), new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab"), new Guid("9b181dcf-1062-4906-9efd-ebabd73e18b1") },
                    { new Guid("ea425ed9-7130-48cb-8464-edb4cbb5ebc2"), new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab"), new Guid("538e9138-f2c9-4c92-bca6-dc33d05d9419") },
                    { new Guid("34ab15a6-0bb1-40b0-9407-3971f41a0c68"), new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab"), new Guid("bdd2c5a5-720e-4e1d-8cfc-16ecfe924b5e") },
                    { new Guid("81f8160d-7ac4-4e26-99da-e5188f0bee4a"), new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab"), new Guid("d05be7e3-fa3a-4124-aed8-a8009fe466c5") },
                    { new Guid("cf70172d-c271-4607-a04e-a17b34044d6b"), new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab"), new Guid("23949217-0f4b-425b-94b9-e80725054b11") },
                    { new Guid("922142e8-07cc-468b-8b59-e8b93981a1e2"), new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab"), new Guid("d396f455-310e-4221-b2ec-c4666d4fa572") },
                    { new Guid("a9d1aadd-9915-4884-9659-40d0cc63e410"), new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab"), new Guid("ec93f72c-2693-44ed-ab8c-1199ce7051b6") },
                    { new Guid("e607cc4f-ddbb-465c-b041-047c065e898b"), new Guid("43a95e77-79e6-4da1-a4ea-2d64f3587b32"), new Guid("a8ba149d-15ff-45bb-b910-cdb639d40a20") },
                    { new Guid("51b1f28e-5406-4c4d-ba3c-161f8ecb4010"), new Guid("43a95e77-79e6-4da1-a4ea-2d64f3587b32"), new Guid("a8ba149d-15ff-45bb-b910-cdb639d40a20") },
                    { new Guid("9f609764-32cf-426a-8943-4d7b3ef2469b"), new Guid("43a95e77-79e6-4da1-a4ea-2d64f3587b32"), new Guid("a8ba149d-15ff-45bb-b910-cdb639d40a20") },
                    { new Guid("3700071e-0899-4161-a730-36b21cdd8660"), new Guid("43a95e77-79e6-4da1-a4ea-2d64f3587b32"), new Guid("38774081-f7d3-4c79-b2f3-1580d86c3bf5") },
                    { new Guid("3f34b30d-93e8-4505-ad03-9362714e2113"), new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab"), new Guid("a949bcc8-d8f7-4aec-870b-5a10c2808b3b") }
                });

            migrationBuilder.InsertData(
                table: "Dataset",
                columns: new[] { "Id", "ContactId", "Description", "HierarchyId", "Location", "Name", "Owner", "SlaDescription", "SlaLink" },
                values: new object[,]
                {
                    { new Guid("3c19ee43-3b3a-43bd-9b0d-101436bad4d7"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("c5dfc671-dcb3-4f10-bf11-758c1d3e97be"), "electricity/capacities/interconnector-general", "Interconnector Capacities General", null, null, null },
                    { new Guid("e287af45-a2fe-4210-9634-05fa7a160077"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("c5dfc671-dcb3-4f10-bf11-758c1d3e97be"), "electricity/capacities/interconnector-day-ahead", "Interconnector Capacities Day Ahead", null, null, null },
                    { new Guid("4556afbe-a7ee-4e02-bebd-9f3cbbb6f7e4"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("c5dfc671-dcb3-4f10-bf11-758c1d3e97be"), "electricity/capacities/interconnector-intraday", "Interconnector Capacities Intraday", null, null, null },
                    { new Guid("7c10db2a-ed65-4ff5-b9c7-8aba184941ca"), new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), null, new Guid("fc20e468-5ab4-48db-8a0b-09e86935c3aa"), "electricity/forecasts/renewable-energy", "Renewable Energy Forecasts", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "DataContract",
                columns: new[] { "Id", "DataSourceId", "DatasetId" },
                values: new object[,]
                {
                    { new Guid("ddacd6e0-4a26-4793-8c14-8bc843315963"), new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab"), new Guid("e287af45-a2fe-4210-9634-05fa7a160077") },
                    { new Guid("f1e409c1-81d0-4c2c-97a3-bfd8bbe22aeb"), new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab"), new Guid("4556afbe-a7ee-4e02-bebd-9f3cbbb6f7e4") },
                    { new Guid("899c4fef-892e-4a91-8191-42e7f970f58b"), new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab"), new Guid("3c19ee43-3b3a-43bd-9b0d-101436bad4d7") },
                    { new Guid("21389a17-bf24-4be8-ab38-5d8f8a012488"), new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab"), new Guid("7c10db2a-ed65-4ff5-b9c7-8aba184941ca") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("21389a17-bf24-4be8-ab38-5d8f8a012488"));

            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("34ab15a6-0bb1-40b0-9407-3971f41a0c68"));

            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("3700071e-0899-4161-a730-36b21cdd8660"));

            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("3f34b30d-93e8-4505-ad03-9362714e2113"));

            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("3fb7650e-4f4d-4033-8746-618e28f06cc6"));

            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("51b1f28e-5406-4c4d-ba3c-161f8ecb4010"));

            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("7a534e40-97f3-4d6c-aff5-b443ba06b13e"));

            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("81f8160d-7ac4-4e26-99da-e5188f0bee4a"));

            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("899c4fef-892e-4a91-8191-42e7f970f58b"));

            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("922142e8-07cc-468b-8b59-e8b93981a1e2"));

            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("9f609764-32cf-426a-8943-4d7b3ef2469b"));

            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("a9d1aadd-9915-4884-9659-40d0cc63e410"));

            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("b9e55105-16d2-4ecf-9633-1c221d5abc82"));

            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("c7f1cb06-2441-4c3f-b4e2-d02cf4cc6b56"));

            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("cf70172d-c271-4607-a04e-a17b34044d6b"));

            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("ddacd6e0-4a26-4793-8c14-8bc843315963"));

            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("e52f8eb1-9ae8-40e1-acc2-052ec8e578ff"));

            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("e607cc4f-ddbb-465c-b041-047c065e898b"));

            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("ea425ed9-7130-48cb-8464-edb4cbb5ebc2"));

            migrationBuilder.DeleteData(
                table: "DataContract",
                keyColumn: "Id",
                keyValue: new Guid("f1e409c1-81d0-4c2c-97a3-bfd8bbe22aeb"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("04476b3c-fa63-4034-bc19-3808c07fb0b4"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("768890fc-cd26-4d53-a2ef-0dd51a5fc821"));

            migrationBuilder.DeleteData(
                table: "DataSource",
                keyColumn: "Id",
                keyValue: new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab"));

            migrationBuilder.DeleteData(
                table: "DataSource",
                keyColumn: "Id",
                keyValue: new Guid("43a95e77-79e6-4da1-a4ea-2d64f3587b32"));

            migrationBuilder.DeleteData(
                table: "DataSource",
                keyColumn: "Id",
                keyValue: new Guid("44bbd193-dd57-4221-89ac-80e3509a0c73"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("0d43e440-5306-4cc9-94f4-37559cf304e0"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("23949217-0f4b-425b-94b9-e80725054b11"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("38774081-f7d3-4c79-b2f3-1580d86c3bf5"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("3c19ee43-3b3a-43bd-9b0d-101436bad4d7"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("4556afbe-a7ee-4e02-bebd-9f3cbbb6f7e4"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("538e9138-f2c9-4c92-bca6-dc33d05d9419"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("7c10db2a-ed65-4ff5-b9c7-8aba184941ca"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("913cd17d-f378-49bc-b96b-0bf28b31712d"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("9b181dcf-1062-4906-9efd-ebabd73e18b1"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("a8ba149d-15ff-45bb-b910-cdb639d40a20"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("a949bcc8-d8f7-4aec-870b-5a10c2808b3b"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("b946363a-1b9c-4f59-b8dd-35c93f42c9ab"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("bdd2c5a5-720e-4e1d-8cfc-16ecfe924b5e"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("d05be7e3-fa3a-4124-aed8-a8009fe466c5"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("d396f455-310e-4221-b2ec-c4666d4fa572"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("e287af45-a2fe-4210-9634-05fa7a160077"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("ec93f72c-2693-44ed-ab8c-1199ce7051b6"));

            migrationBuilder.DeleteData(
                table: "Dataset",
                keyColumn: "Id",
                keyValue: new Guid("ed32e705-65ea-4cf8-be34-3a5df2e568d4"));

            migrationBuilder.DeleteData(
                table: "Hierarchy",
                keyColumn: "Id",
                keyValue: new Guid("c5dfc671-dcb3-4f10-bf11-758c1d3e97be"));

            migrationBuilder.DeleteData(
                table: "Hierarchy",
                keyColumn: "Id",
                keyValue: new Guid("fc20e468-5ab4-48db-8a0b-09e86935c3aa"));

            migrationBuilder.InsertData(
                table: "DataSource",
                columns: new[] { "Id", "ContactInfo", "Description", "Name", "SourceType" },
                values: new object[] { new Guid("76d869e6-254f-40e3-8212-91e31cac1f7e"), "dps@energinet.dk", "Energinet system DPS", "DPS", 1 });
        }
    }
}