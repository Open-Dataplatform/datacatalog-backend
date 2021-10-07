using System;
using DataCatalog.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DataCatalog.Common.Implementations
{
    public class CorrelationIdResolver : ICorrelationIdResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public CorrelationIdResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string GetCorrelationId()
        {
            var services = _serviceProvider.GetServices<ICorrelationIdProvider>();

            foreach (var correlationIdProvider in services)
            {
                var correlationId = correlationIdProvider.GetCorrelationId();
                if (!string.IsNullOrEmpty(correlationId))
                {
                    return correlationId;
                }
            }

            return null;
        }
    }
}
