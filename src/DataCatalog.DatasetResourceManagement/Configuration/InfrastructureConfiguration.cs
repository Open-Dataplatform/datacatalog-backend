using System;

namespace Energinet.DataPlatform.DataSetResourceManagement.Infrastructure.Configuration
{
    public class InfrastructureConfiguration
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }
        public Uri AadProvisionerBaseUrl { get; set; }
    }
}
