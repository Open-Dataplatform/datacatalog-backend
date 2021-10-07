using System;
using DataCatalog.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Pipeline;
using Rebus.Messages;

namespace DataCatalog.Common.Rebus
{
    public class RebusCorrelationIdProvider : ICorrelationIdProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public RebusCorrelationIdProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string GetCorrelationId()
        {
            try
            {
                // try to resolve the IMessageContext
                var messageContext = _serviceProvider.GetService<IMessageContext>();
                string correlationIdFromMessage = null;
                messageContext?.Headers?.TryGetValue(Headers.CorrelationId, out correlationIdFromMessage);
                if (correlationIdFromMessage != null)
                {
                    return correlationIdFromMessage;
                }
            }
            catch (InvalidOperationException exception)
            {
                // if we are not in a message handler, the IMessageContext service is not available
                if (exception.Source != "Rebus.ServiceProvider") throw;
            }

            return null;
        }
    }
}
