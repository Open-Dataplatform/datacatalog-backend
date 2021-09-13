using DataCatalog.Api.Data.Domain;
using DataCatalog.Common.Enums;

namespace DataCatalog.Api.Extensions
{
    public static class DatasetExtensions
    {
        public static bool ShouldHaveAllUsersGroup(this Dataset dataset) 
        {
            return dataset.Confidentiality == Confidentiality.Public && dataset.Status == DatasetStatus.Published;
        }
    }
}