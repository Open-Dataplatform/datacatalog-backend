using DataCatalog.Common.Extensions;
using DataCatalog.Common.Utils;
using Rebus.Config;

namespace DataCatalog.DatasetResourceManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HostBuilderExtensions.CreateHostBuilderWithStartup<Startup>("Dataset Resource Management Service", args,
                config => config.Enrich.WithRebusCorrelationId(CorrelationId.CorrelationIdHeaderKey));
        }
    }
}