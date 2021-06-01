using DataCatalog.Data;
using DataCatalog.Data.Extensions;
using DataCatalog.Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using DataCatalog.Common.Extensions;

namespace src.DataCatalog.Migrations
{
    public class DataCatalogContextFactory : IDesignTimeDbContextFactory<DataCatalogContext>
    {
        public DataCatalogContext CreateDbContext(string[] args)
        {
            var environmentName = EnvironmentUtil.GetCurrentEnvironment();

            var configuration = new ConfigurationBuilder()
                .BuildPlatformConfiguration(environmentName, args)
                .Build();

            return CreateDbContext(configuration);
        }

        public DataCatalogContext CreateDbContext(IConfiguration configuration)
        {
            // Db Context
            var conn = configuration.GetConnectionString("DataCatalog");
            conn.ValidateConfiguration("ConnectionStrings:DataCatalog");

            var optionsBuilder = new DbContextOptionsBuilder<DataCatalogContext>();
            optionsBuilder.UseSqlServer(conn);

            return new DataCatalogContext(optionsBuilder.Options);
        }
    }
}