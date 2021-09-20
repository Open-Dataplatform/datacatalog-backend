using System;

namespace DataCatalog.Api.Data.Domain
{
    public class AdSearchResultType
    {
        public static string Group => "Group";
        public static string User => "User";
        public static string ServicePrincipal => "ServicePrincipal";
    }
    public class AdSearchResult
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Mail { get; set; }
        public string Type { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is AdSearchResult other)
            {
                return Id == other.Id && DisplayName == other.DisplayName && Mail == other.Mail && Type == other.Type;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, DisplayName, Mail, Type);
        }
    }
}
