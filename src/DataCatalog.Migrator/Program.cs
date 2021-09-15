using System;
using System.Collections.Generic;
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
                Log.Information("Configuring the DataCatalog database using the environment {Environment}", environmentName);

                await UpdateDatabase(configuration);

                Log.Information("Job completed successfully");
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

        private static async Task UpdateDatabase(IConfiguration configuration)
        {
            using var db = new DataCatalogContextFactory().CreateDbContext(configuration);
            
            foreach (var migrationName in GetUnappliedMigrations(db))
            {
                Log.Information($"Applying migration: {migrationName}");
            }

            // Apply migrations if there are any pending
            db.Database.Migrate();

            //One-time update of OriginEnvironment
            var originEnvironment = EnvironmentUtil.GetCurrentEnvironment().ToLower();
            var datasets = db.Datasets.Where(a => a.OriginEnvironment == null).ToArray();
            foreach (var a in datasets)
            {
                Log.Information($"Updating origin environment of dataset {a.Name}");
                a.OriginEnvironment = originEnvironment;
            }

            var categories = db.Categories.Where(a => a.OriginEnvironment == null).ToArray();
            foreach (var a in categories)
            { 
                Log.Information($"Updating origin environment of category with name {a.Name}");
                a.OriginEnvironment = originEnvironment;
            } 
            
            var dataSources = db.DataSources.Where(a => a.OriginEnvironment == null).ToArray();
            foreach (var a in dataSources)
            {
                Log.Information($"Updating origin environment of data source with name {a.Name}");
                a.OriginEnvironment = originEnvironment;
            }

            await db.SaveChangesAsync();
            await new SeedLogic(db).SeedData();
        }

        private static IEnumerable<string> GetUnappliedMigrations(DataCatalogContext db)
        {
            var allMigrations = db.Database.GetMigrations().ToHashSet();
            var appliedMigrations = db.Database.GetAppliedMigrations().ToHashSet();

            return allMigrations.Except(appliedMigrations);
        }
    }

}
