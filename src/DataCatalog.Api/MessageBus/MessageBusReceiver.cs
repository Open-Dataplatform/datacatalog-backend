using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCatalog.Api.Services;
using DataCatalog.Common.Utils;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DataCatalog.Api.MessageBus
{
    public class MessageBusReceiver<TMessage, TService> : IHostedService where TService : IHandleMessageBusMessage
    {
        private ISubscriptionClient _subscriptionClient;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MessageBusReceiver(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            // Subscribe to service bus messages
            var serviceBusNamespace = $"dpdomainevents-servicebus-{EnvironmentUtil.GetCurrentEnvironment().ToLower()}";
            var serviceBusEndpoint = $"Endpoint=sb://{serviceBusNamespace}.servicebus.windows.net/;Authentication=Managed Identity;";
            _subscriptionClient = new SubscriptionClient(serviceBusEndpoint, typeof(TMessage).Name, "DataCatalog-API", ReceiveMode.PeekLock, RetryPolicy.Default);
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            _subscriptionClient.RegisterMessageHandler(ReceiveAsync, messageHandlerOptions);

            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task ReceiveAsync(Message message, CancellationToken token)
        {
            // Process the message.
            using var scope = _serviceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetService<TService>();
            await service.HandleMessage(Encoding.UTF8.GetString(message.Body));

            // Complete the message so that it is not received again.
            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
