using System;
using System.Threading.Tasks;
using DataCatalog.Common.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DataCatalog.Common.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (RequiresCorrelationId(context))
            {
                HandleCorrelationId(context);
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }

        private void HandleCorrelationId(HttpContext context)
        {
            const string headerKey = CorrelationId.CorrelationIdHeaderKey;
            string correlationId = null;

            if (context.Request.Headers.ContainsKey(headerKey))
            {
                correlationId = context.Request.Headers[headerKey];
            }
            else if (context.Response.Headers.ContainsKey(headerKey))
            {
                correlationId = context.Response.Headers[headerKey];
            }

            var newCorrelationIdCreated = false;
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                newCorrelationIdCreated = true;
            }

            // this should be done before first log print in order to get correct correlationId on it in case of a new one is created
            SaveCorrelationIdOnContext(context, correlationId);
            if (newCorrelationIdCreated)
            {
                _logger.LogDebug("A request was performed without a CorrelationId, one is automatically generated: {CorrelationId}", correlationId);
            }
        }

        private static bool RequiresCorrelationId(HttpContext context)
        {
            return context.Request.Path.ToString().StartsWith("/api/");
        }

        private static void SaveCorrelationIdOnContext(HttpContext context, string correlationId)
        {
            // Add to both request and response to ensure it is carried on/returned to other services
            context.Request.Headers[CorrelationId.CorrelationIdHeaderKey] = correlationId;
            context.Response.Headers[CorrelationId.CorrelationIdHeaderKey] = correlationId;
        }
    }
}
