namespace DataCatalog.Api.Data.Domain
{
    public class AccessMember
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public AccessMemberType Type { get; set; }
    }
}