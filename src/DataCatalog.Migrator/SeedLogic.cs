using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;
using DataCatalog.Common.Interfaces;
using DataCatalog.Data;
using DataCatalog.Data.Model;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DataCatalog.Migrator
{
    public class SeedLogic
    {
        private readonly DataCatalogContext _context;

        public SeedLogic(DataCatalogContext context)
        {
            _context = context;
        }

        public async Task SeedData()
        {
            await SeedCategories();
            await SeedDurations();
            await SeedHierarchies();
            await SeedDataSources();
            await SeedIdentityProviders();
            await SeedContacts();
        }

        private async Task SeedContacts()
        {
            var contacts = new List<MemberGroup>
            {
                new()
                {
                    Id = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"),
                    Name = "Demo Group",
                    Email = "demo@energinet.dk"
                }
            };
            await UpsertData(contacts);
        }

        private async Task SeedCategories()
        {
            var categories = new List<Category>
            {
                new Category { Id = new Guid("8713b259-0294-480a-960c-08a9c9983961"), Name = "Ancillary Services", Colour = "#2A939B", OriginEnvironment = "prod" },
                new Category { Id = new Guid("b94863a9-685a-44d6-aae4-2cfb3a63c489"), Name = "Auctions, Transmission Capacity", Colour = "#389B88", OriginEnvironment = "prod" },
                new Category { Id = new Guid("e5b2f760-e965-452f-b0ab-70ca506d4c20"), Name = "Emissions", Colour = "#452A9B", OriginEnvironment = "prod" },
                new Category { Id = new Guid("08e58f12-51bf-4af9-af3a-02022cab77c7"), Name = "Day Ahead Market", Colour = "#2A9B65", OriginEnvironment = "prod" },
                new Category { Id = new Guid("23ab1cd5-ae16-4aa4-bab7-25380c1634d5"), Name = "Electric Boilers", Colour = "#B27736", OriginEnvironment = "prod" },
                new Category { Id = new Guid("8a95d290-417e-4a53-a807-a13293f3117d"), Name = "Gas", Colour = "#F8AE3C", OriginEnvironment = "prod" },
                new Category { Id = new Guid("5873b2e5-5db1-42db-bf25-83fd9dded293"), Name = "Production and Consumption", Colour = "#663BCC", OriginEnvironment = "prod" },
                new Category { Id = new Guid("084e6353-82a8-4f99-b9f3-b247081c34c8"), Name = "Electricity Consumption", Colour = "#A0C1C2", OriginEnvironment = "prod" },
                new Category { Id = new Guid("5b39fbdd-76c5-44df-9f7d-d314db760fc1"), Name = "Intra Day Market", Colour = "#819B38", OriginEnvironment = "prod" },
                new Category { Id = new Guid("a90abc39-c2de-4c29-a9f8-4e10f41a50f0"), Name = "Electricity Production", Colour = "#293A4C", OriginEnvironment = "prod" },
                new Category { Id = new Guid("9ac5af8d-4165-448a-ace5-2edcf0c3e202"), Name = "Regulating Power", Colour = "#548E80", OriginEnvironment = "prod" },
                new Category { Id = new Guid("7434d8a3-7ebb-4212-967c-061d5a2666d7"), Name = "Whole Sale Market", Colour = "#7D8E1C", OriginEnvironment = "prod" },
                new Category { Id = new Guid("90909fcd-4a5a-455a-a543-9c2c8a45e453"), Name = "Reserves", Colour = "#398C22", OriginEnvironment = "prod" },
                new Category { Id = new Guid("d6944781-c069-4c14-859f-07865164763f"), Name = "Solar Power", Colour = "#FFD424", OriginEnvironment = "prod" },
                new Category { Id = new Guid("b4cef11a-d0f4-4c33-9796-b80ea6d2489a"), Name = "Transmission Lines", Colour = "#547B8E", OriginEnvironment = "prod" },
                new Category { Id = new Guid("418e657d-3efb-4a30-ad3f-3f95a6ab8888"), Name = "Wind Power", Colour = "#C2E4F0", OriginEnvironment = "prod" },
                new Category { Id = new Guid("6f4f675a-90c3-4504-955f-a823a897beb9"), Name = "Weather", Colour = "#B27736", OriginEnvironment = "prod" }
            };

            await UpsertData(categories);
        }
        private async Task SeedDurations()
        {
            var durations = new List<Duration>
            {
                new Duration { Id = new Guid("f745f308-1542-4a7a-975f-0cd6f1e73668"), Code = "PT1H", Description = "1 hour" },
                new Duration { Id = new Guid("2ff90ae1-27a8-4cb8-a697-6e81a5e36fb0"), Code = "P1Y", Description = "1 year" },
                new Duration { Id = new Guid("6817a5e6-7c80-470f-a1b2-fa79ed3c125d"), Code = "P1M", Description = "1 month" },
                new Duration { Id = new Guid("c70a3773-3473-4074-8c08-4a1bfffaa1d1"), Code = "P7D", Description = "7 days" },
                new Duration { Id = new Guid("20986017-c3a8-41a0-8221-b5e1a5995c05"), Code = "P1D", Description = "1 day" },
                new Duration { Id = new Guid("2b3d0756-0817-4281-a2ca-76c2b0eb90d6"), Code = "PT12H", Description = "12 hours" },
                new Duration { Id = new Guid("984b1eb4-0420-4354-8ae7-a9b6c9c33aa8"), Code = "PT6H", Description = "6 hours" },
                new Duration { Id = new Guid("2fb37506-3252-4065-a81b-5c633e158a7c"), Code = "PT15M", Description = "15 minutes" },
                new Duration { Id = new Guid("b64a8a70-cb40-4117-93ba-093571ea4aeb"), Code = "PT5M", Description = "5 minutes" },
                new Duration { Id = new Guid("d05a8570-6155-41bd-95b1-20e6f2856cc2"), Code = "PT3M", Description = "3 minutes" },
                new Duration { Id = new Guid("b57f0531-0eb0-4bc2-b81c-266a3f52368e"), Code = "PT1M", Description = "1 minute" }
            };

            await UpsertData(durations);
        }

        private async Task SeedHierarchies()
        {
            var hierarchies = new List<Hierarchy>
            {
                // Electricity
                new Hierarchy { Id = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0"), Name = "Electricity", Description = "Anything mainly related to electricity" },
                new Hierarchy { Id = new Guid("ad240fb2-ca95-4ec0-9bc2-58c73bc4ac31"), Name = "Consumption", Description = "Electricity consumption", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("276af4b1-72ea-4654-8ce3-37e73b2657c0"), Name = "Production", Description = "Electricity production", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("a9a780d7-8f2c-4d86-b863-1327bd264575"), Name = "Consumption and Production", Description = "Electricity consumption and production", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"), Name = "Reserves", Description = "Electricity reserves", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("a6b40fdf-9831-49ec-87b0-cfed8f447577"), Name = "Ancillary Services", Description = "Electricity ancillary services", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("5f2d33fc-decc-4061-af3c-5d58e65bbc9c"), Name = "CO2 Emissions", Description = "Electricity CO2 Emissions", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("d27b19d3-e589-4f98-809a-351a062635e8"), Name = "Regulating Power", Description = "Electricity regulating power", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("fb668c70-219c-4edf-b31a-7ad36ee43d88"), Name = "Solar Power", Description = "Electricity solar power", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("ded03663-ec2e-445f-b897-b5b78c00f2d4"), Name = "Wind Power", Description = "Electricity wind power", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("c5dfc671-dcb3-4f10-bf11-758c1d3e97be"), Name = "Capacities", Description = "Electricity capacities", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("fc20e468-5ab4-48db-8a0b-09e86935c3aa"), Name = "Forecasts", Description = "Electricity forecasts", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                
                // Gas
                new Hierarchy { Id = new Guid("586c321c-3f01-4cd2-8228-000670114e32"), Name = "Gas", Description = "Anything mainly related to gas" },
                new Hierarchy { Id = new Guid("5b22fc05-6837-40f4-a8a2-184e9aa25aff"), Name = "Consumption", Description = "Gas consumption", ParentHierarchyId = new Guid("586c321c-3f01-4cd2-8228-000670114e32") },
                new Hierarchy { Id = new Guid("49ca837b-79ba-4a4f-bbc5-e01a8f23befd"), Name = "Production", Description = "Gas Production", ParentHierarchyId = new Guid("586c321c-3f01-4cd2-8228-000670114e32") },
                new Hierarchy { Id = new Guid("7330f2ef-f86f-4b7d-8592-e7c56ef52d15"), Name = "Consumption and Production", Description = "Gas consumption and production", ParentHierarchyId = new Guid("586c321c-3f01-4cd2-8228-000670114e32") },
                new Hierarchy { Id = new Guid("334740dd-7158-475d-84e9-c8e83403b515"), Name = "Storage", Description = "Gas storage", ParentHierarchyId = new Guid("586c321c-3f01-4cd2-8228-000670114e32") },
                new Hierarchy { Id = new Guid("1653a169-9548-46f6-aa47-ea9a90088b0f"), Name = "CO2 Emissions", Description = "Gas CO2 Emissions", ParentHierarchyId = new Guid("586c321c-3f01-4cd2-8228-000670114e32") },
                
                // Weather
                new Hierarchy { Id = new Guid("596203b3-743a-4482-8b20-f4bc6eb844b6"), Name = "Weather", Description = "Anything mainly related to weather" },
                new Hierarchy { Id = new Guid("a8adde4d-4dc9-4028-8993-0ef9bc153d7d"), Name = "Forecasts", Description = "Weather forecasts", ParentHierarchyId = new Guid("596203b3-743a-4482-8b20-f4bc6eb844b6") },
                new Hierarchy { Id = new Guid("077a87f9-1842-40b4-b132-1c966b41d761"), Name = "Historical Data", Description = "Weather historical data", ParentHierarchyId = new Guid("596203b3-743a-4482-8b20-f4bc6eb844b6") }
            };

            await UpsertData(hierarchies);
        }

        private async Task SeedDataSources()
        {
            var dataSources = new List<DataSource>
            {
                new DataSource { Id = new Guid("14c7e740-6104-4036-af76-73891fdf0033"), OriginEnvironment = "prod", Name = "DataPlatform", Description = "Use for any dataset that originates from within the data platform itself", ContactInfo = "dataplatform@energinet.dk", SourceType = SourceType.DataPlatform },
                new DataSource { Id = new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab"), OriginEnvironment = "prod", Name = "DPS", Description = "Energinet system DPS", ContactInfo = "dps@energinet.dk", SourceType = SourceType.Internal },
                new DataSource { Id = new Guid("43a95e77-79e6-4da1-a4ea-2d64f3587b32"), OriginEnvironment = "prod", Name = "Propel", Description = "Energinet system Propel", ContactInfo = "propel@energinet.dk", SourceType = SourceType.Internal },
                new DataSource { Id = new Guid("44bbd193-dd57-4221-89ac-80e3509a0c73"), OriginEnvironment = "prod", Name = "Neptun", Description = "Energinet system Neptun", ContactInfo = "neptun@energinet.dk", SourceType = SourceType.Internal },
                new DataSource { Id = new Guid("bec46f78-9ef7-49f1-b98b-748f85aa823b"), OriginEnvironment = "prod", Name = "DMI", Description = "Danish Meteorological Institute", ContactInfo = "dmi.dk/kontakt", SourceType = SourceType.External },
                new DataSource { Id = new Guid("3880d4cd-64d9-4205-a688-83919a0a958b"), OriginEnvironment = "prod", Name = "GTMS", Description = "Gas Transmission Rate Measurement System data", ContactInfo = "mjp@energinet.dk", SourceType = SourceType.Internal }
            };

            await UpsertData(dataSources);
        }

        private async Task SeedIdentityProviders()
        {
            var identityProviders = new List<IdentityProvider>
            {
                new IdentityProvider { Id = new Guid("75030760-f7f8-40d8-a1ab-718bcb7327b7"), Name = "Azure AD", TenantId = "f7619355-6c67-4100-9a78-1847f30742e2"}
            };

            await UpsertData(identityProviders);
        }



        private async Task UpsertData<T>(List<T> data) where T : class, IGuidId
        {
            // Get the dbSet of the current type, which we can use to query the database
            var dbSet = _context.Set<T>();

            var ids = data.Select(d => d.Id).ToList();

            // Fetch what's currently in the database
            var dbData = await dbSet
                .Where(entity => ids.Contains(entity.Id))
                .ToDictionaryAsync(entity => entity.Id);

            foreach(var dataItem in data)
            {
                if (dbData.TryGetValue(dataItem.Id, out var dbItem))
                {
                    // Update the item if it's already present in the database
                    _context.Entry(dbItem).CurrentValues.SetValues(dataItem);
                }
                else 
                {
                    // Insert the item if it's not currently present in the database
                    Log.Information($"Adding seeded item of type {typeof(T).Name} with ID {dataItem.Id}");
                    dbSet.Add(dataItem);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}