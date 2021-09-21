using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            await SeedDurations();
            await SeedDataSources();
            await SeedIdentityProviders();
            await SeedServiceLevelAgreements();
        }

        private async Task SeedServiceLevelAgreements()
        {
            var serviceLevelAgreements = new List<ServiceLevelAgreement>
            {
                new ServiceLevelAgreement { Id = new Guid("2a0364c4-6047-4a3e-b21e-9b67ed8a71df"), Name = "No Agreement", Description = "No agreement" },
            };

            await UpsertData(serviceLevelAgreements);
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
                    Log.Information("Adding seeded item of type {Type} with ID {ID}", typeof(T).Name, dataItem.Id);
                    dbSet.Add(dataItem);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}