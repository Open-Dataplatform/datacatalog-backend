namespace DataCatalog.Api.Data.Domain
{
    public class AdSearchResultType
    {
        public static string Group { get; } = "Group";
        public static string User { get; } = "User";
        public static string ServicePrincipal { get; } = "ServicePrincipal";
    }
    public class AdSearchResult
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
    }
}
