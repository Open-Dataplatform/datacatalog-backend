using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using DataCatalog.Common.Extensions;
using DataCatalog.Common.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DataCatalog.Api.MessageBus
{
    public class MessageBusReceiver<TMessage, TService> : IHostedService where TService : IHandleMessageBusMessage
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusProcessor _serviceBusProcessor;

        public MessageBusReceiver(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            var conn = configuration.GetConnectionString("ServiceBus");
            conn.ValidateConfiguration("ConnectionStrings:ServiceBus");

            var messageHandlerOptions = new ServiceBusProcessorOptions()
            {
                MaxConcurrentCalls = 1,
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                AutoCompleteMessages = false,
            };

            _serviceBusClient = new ServiceBusClient(conn);
            _serviceBusProcessor = _serviceBusClient.CreateProcessor(typeof(TMessage).Name, "DataCatalog-API", messageHandlerOptions);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Subscribe to service bus messages

            _serviceBusProcessor.ProcessMessageAsync += ReceiveAsync;
            _serviceBusProcessor.ProcessErrorAsync += ExceptionReceivedHandler;

            await _serviceBusProcessor.StartProcessingAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _serviceBusProcessor.DisposeAsync();
            await _serviceBusClient.DisposeAsync();
        }

        private async Task ReceiveAsync(ProcessMessageEventArgs args)
        {
            // Process the message.
            using var scope = _serviceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetService<TService>();
            await service.HandleMessage(args.Message.Body.ToString());

            // Complete the message so that it is not received again.
            await args.CompleteMessageAsync(args.Message);
        }

        private static Task ExceptionReceivedHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Message handler encountered an exception {args.Exception}.");
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Entity Path: {args.EntityPath}");
            return Task.CompletedTask;
        }
    }
}
