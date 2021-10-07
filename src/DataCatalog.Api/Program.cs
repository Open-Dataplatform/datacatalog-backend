using DataCatalog.Common.Extensions;
using DataCatalog.Common.Utils;
using Rebus.Config;

namespace DataCatalog.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HostBuilderExtensions.CreateHostBuilderWithStartup<Startup>("DataCatalog Api", args, 
                config => config.Enrich.WithRebusCorrelationId(CorrelationId.CorrelationIdHeaderKey));
        }
    }
}
