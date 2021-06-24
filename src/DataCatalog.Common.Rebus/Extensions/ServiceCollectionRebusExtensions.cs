using System;
using System.Reflection;
using DataCatalog.Common.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Serilog;

namespace DataCatalog.Common.Rebus.Extensions
{
    public static class ServiceCollectionRebusExtensions
    {
        /// <summary>
        /// Adds Rebus to the DI. It will automatically include all handlers within the assembly that the <code>TMessageHandlerType</code>
        /// is included in.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="connectionString">The connection string to the Mysql database</param>
        /// <typeparam name="TMessageHandlerType">A type from the assembly where all handlers should be included in</typeparam>
        /// <returns></returns>
        public static IServiceCollection AddRebusWithSubscription<TMessageHandlerType>(this IServiceCollection services,
            IConfiguration configuration, string connectionString)
        {
            var numberOfWorkers = configuration.GetValidatedIntValue("Rebus:NumberOfWorkers");
            var maxParallelism = configuration.GetValidatedIntValue("Rebus:MaxParallelism");
            var attemptNumberKey = configuration.GetValue("Rebus:AttemptNumberKey", "attemptNumber");
            var maxAttempts = configuration.GetValidatedIntValue("Rebus:MaxAttempts");
            var retryInMinutes = configuration.GetValidatedIntValue("Rebus:RetryInMinutes");
            var secondsTimeout = configuration.GetValidatedIntValue("Rebus:SecondsTimeout");
            //log configuration
            Log.Information("Logging configuration - start Rebus");
            Log.Information("Rebus:NumberOfWorkers = {NumberOfWorkers}", numberOfWorkers);
            Log.Information("Rebus:MaxParallelism = {MaxParallelism}", maxParallelism);
            Log.Information("Rebus:AttemptNumberKey = {AttemptNumberKey}", attemptNumberKey);
            Log.Information("Rebus:MaxAttempts = {MaxAttempts}", maxAttempts);
            Log.Information("Rebus:RetryInMinutes = {RetryInMinutes}", retryInMinutes);
            Log.Information("Rebus:SecondsTimeout = {SecondsTimeout}", secondsTimeout);
            Log.Information("Logging configuration - end Rebus");

            var callerModuleName = RemoveDotsAndDll(Assembly.GetCallingAssembly().ManifestModule.Name);

            const string tableName = "datacatalog_messaging_queue";
            const string subscriptionsTableName = "datacatalog_messaging_subscriptions";

            var inputQueueName = callerModuleName + "_Queue";
            var errorQueueAddress = callerModuleName + "_Error_Queue";

            services.AutoRegisterHandlersFromAssemblyOf<TMessageHandlerType>();
            
            var transportOptions = new SqlServerTransportOptions(connectionString);
            services.AddRebus(configure => configure
                .Logging(l => l.Serilog())
                .Transport(t => t.UseSqlServer(transportOptions, inputQueueName))
                .Options(o =>
                {
                    o.SetNumberOfWorkers(numberOfWorkers);
                    o.SetMaxParallelism(maxParallelism);
                    o.SimpleRetryStrategy(secondLevelRetriesEnabled: true, maxDeliveryAttempts: maxAttempts, errorQueueAddress: errorQueueAddress);
                })
                .Subscriptions(s => s.StoreInSqlServer(connectionString, subscriptionsTableName))
                .Routing(r => r.TypeBased().MapAssemblyOf<TMessageHandlerType>(tableName))
            );

            return services;
        }
        
        private static string RemoveDotsAndDll(string input)
        {
            return input
                .Replace(".dll", "", StringComparison.InvariantCultureIgnoreCase)
                .Replace(".", "_", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}