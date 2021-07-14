using DataCatalog.Common.Extensions;

namespace DataCatalog.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HostBuilderExtensions.CreateHostBuilderWithStartup<Startup>("DataCatalog Api", args);
        }
    }
}
