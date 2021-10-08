using System;
using System.Collections.Generic;
using System.Reflection;
using DataCatalog.Common.Extensions;
using DataCatalog.Common.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Messages;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Serilog;

namespace DataCatalog.Common.Rebus.Extensions
{
    public static class ServiceCollectionRebusExtensions
    {
        /// <summary>
        /// Ensures setup of rebus in the service provider and subscribe to message of the given types
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="messageTypes">The message types to subscribe to</param>
        /// <returns>The service provider</returns>
        public static IServiceProvider UseRebusSubscriptions(this IServiceProvider serviceProvider, IEnumerable<Type> messageTypes)
        {
            serviceProvider.UseRebus(bus =>
            {
                foreach (var messageType in messageTypes)
                {
                    bus.Subscribe(messageType);
                }
            });

            return serviceProvider;
        }

        /// <summary>
        /// Adds Rebus to the DI. It will automatically include all handlers within the assembly that the <code>TMessageHandlerType</code>
        /// is included in.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration</param>
        /// <param name="connectionString">The connection string to the Mysql database</param>
        /// <param name="messageTypesToMap">All message types which you would like to subscribe to</param>
        /// <typeparam name="TMessageHandlerType">A type from the assembly where all message handlers should be included in</typeparam>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddRebusWithSubscription<TMessageHandlerType>(this IServiceCollection services,
            IConfiguration configuration, string connectionString, IEnumerable<Type> messageTypesToMap)
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
            
            services.Configure<RebusOptions>(configuration.GetSection("Rebus"));

            var callerModuleName = RemoveDotsAndDll(Assembly.GetCallingAssembly().ManifestModule.Name);
            
            const string subscriptionsTableName = "datacatalog_messaging_subscriptions";

            var inputQueueName = callerModuleName + "_Queue";
            var errorQueueAddress = callerModuleName + "_Error_Queue";

            services.AutoRegisterHandlersFromAssemblyOf<TMessageHandlerType>();
            
            var transportOptions = new SqlServerTransportOptions(connectionString);
            services.AddRebus(configure => configure
                .Events(e =>
                {
                    e.BeforeMessageSent += (_, headers, message, _) =>
                    {
                        var m = message as MessageBase;
                        headers[Headers.CorrelationId] = m?.CorrelationId;
                    };
                })
                .Logging(l => l.Serilog())
                .Transport(t => t.UseSqlServer(transportOptions, inputQueueName))
                .Options(o =>
                {
                    o.SetNumberOfWorkers(numberOfWorkers);
                    o.SetMaxParallelism(maxParallelism);
                    o.SimpleRetryStrategy(secondLevelRetriesEnabled: true, maxDeliveryAttempts: maxAttempts, errorQueueAddress: errorQueueAddress);
                })
                .Subscriptions(s => s.StoreInSqlServer(connectionString, subscriptionsTableName))
                .Routing(r =>
                {
                    foreach (var type in messageTypesToMap)
                    {
                        r.TypeBased().Map(type, inputQueueName);
                    }
                })
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