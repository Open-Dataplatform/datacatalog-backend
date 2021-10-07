using DataCatalog.Common.Interfaces;
using DataCatalog.Common.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace DataCatalog.Common.Implementations
{
    public class WebApiCorrelationIdProvider : ICorrelationIdProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WebApiCorrelationIdProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCorrelationId()
        {
            StringValues correlationId = StringValues.Empty;
            _httpContextAccessor.HttpContext?.Request.Headers.TryGetValue(CorrelationId.CorrelationIdHeaderKey, out correlationId);
            return correlationId;
        }
    }
}
