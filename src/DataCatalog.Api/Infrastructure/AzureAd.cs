namespace DataCatalog.Api.Infrastructure
{
    public class AzureAd
    {
        public string TenantId { get; set; }
        public string Authority { get; set; }
        public string Audience { get; set; }
        public AllRoles Roles { get; set; }
    }
}
