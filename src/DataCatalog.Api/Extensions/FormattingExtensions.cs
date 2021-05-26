using System.Linq;

namespace DataCatalog.Api.Extensions
{
    public static class FormattingExtensions
    {
        public static string FormatName(this string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            name = name.TrimStart().TrimEnd();
            name = name[0] + (name.Length > 1 ? name.Substring(1) : "");
            return name;
        }

        public static string FirstCharToUpper(this string input) 
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            return input.First().ToString().ToUpper() + input.Substring(1);
        }
        
    }
}