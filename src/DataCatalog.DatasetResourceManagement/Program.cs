using DataCatalog.Common.Extensions;

namespace DataCatalog.DatasetResourceManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HostBuilderExtensions.CreateHostBuilderWithStartup<Startup>("Dataset Resource Management Service", args);
        }
    }
}