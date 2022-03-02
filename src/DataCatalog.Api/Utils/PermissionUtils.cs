using DataCatalog.Api.Services;
using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;

namespace DataCatalog.Api.Utils
{
    public class PermissionUtils : IPermissionUtils
    {
        private readonly Current _current;

        public PermissionUtils(Current current)
        {
            _current = current;
        }

        public bool IsAdminOrDataSteward => _current.Roles.Contains(Role.Admin) 
                                            || _current.Roles.Contains(Role.DataSteward) 
                                            || _current.Roles.Contains(Role.MetadataProvider)
                                            || _current.Roles.Contains(Role.ServiceReader);

        public bool FilterUnpublishedDatasets => !IsAdminOrDataSteward;

        public bool IsAdmin => _current.Roles.Contains(Role.Admin);
    }
}