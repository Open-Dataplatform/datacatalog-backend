using System;
using System.Linq;
using System.Threading.Tasks;
using DataCatalog.Common.Extensions;
using DataCatalog.Common.Utils;
using DataCatalog.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace DataCatalog.Migrator
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var environmentName = EnvironmentUtil.GetCurrentEnvironment();

            var configuration = new ConfigurationBuilder()
                .BuildPlatformConfiguration(environmentName, args)
                .Build();
                
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironment()
                .CreateLogger();

            try 
            {
                await UpdateDatabase(configuration);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        static async Task UpdateDatabase(IConfiguration configuration)
        {
            using var db = new DataCatalogContextFactory().CreateDbContext(configuration);

            // Apply migrations
            db.Database.Migrate();

            //One-time update of OriginEnvironment
            var originEnvironment = EnvironmentUtil.GetCurrentEnvironment().ToLower();
            var datasets = db.Datasets.Where(a => a.OriginEnvironment == null).ToArray();
            foreach (var a in datasets)
            {
                a.OriginEnvironment = originEnvironment;
            }

            var dataContracts = db.DataContracts.Where(a => a.OriginEnvironment == null).ToArray();
            foreach (var a in dataContracts)
            {
                a.OriginEnvironment = originEnvironment;
            }

            var categories = db.Categories.Where(a => a.OriginEnvironment == null).ToArray();
            foreach (var a in categories)
            { 
                a.OriginEnvironment = originEnvironment;
            } 
            
            var dataSources = db.DataSources.Where(a => a.OriginEnvironment == null).ToArray();
            foreach (var a in dataSources)
            {
                a.OriginEnvironment = originEnvironment;
            }

            await db.SaveChangesAsync();
            await new SeedLogic(db).SeedData();
        }
    }
}
