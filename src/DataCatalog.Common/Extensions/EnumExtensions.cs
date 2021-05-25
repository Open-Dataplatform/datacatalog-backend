using System.Collections.Generic;
using System.Linq;

namespace DataCatalog.Api.Extensions
{
    public static class EnumExtensions
    {
        public static string EnumNameToDescription(this object value)
        {
            var name = System.Enum.GetName(value.GetType(), value);
            var result = name.Substring(0, 1);
            for (int i = 1; i < name.Length; i++)
            {
                var letter = name[i];
                if (char.IsUpper(letter)) result += ' ';
                result += letter.ToString().ToLower();
            }
            return result;
        }

        public static IEnumerable<System.Enum> GetEnums<TEnum>() =>
            System.Enum.GetValues(typeof(TEnum)).Cast<System.Enum>();
    }
}