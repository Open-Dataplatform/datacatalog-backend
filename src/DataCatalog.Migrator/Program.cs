using System;
using System.Linq;

using DataCatalog.Api.Extensions;
using DataCatalog.Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using src.DataCatalog.Migrations;

namespace DataCatalog.Migrations
{
    class Program
    {
        static void Main(string[] args)
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
                UpdateDatabase(configuration);
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

        static void UpdateDatabase(IConfiguration configuration)
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

            db.SaveChanges();
        }
    }
}
