using DataCatalog.Common.Data;
using DataCatalog.Common.Enums;

namespace DataCatalog.Api.Services
{
    public class PermissionUtils : IPermissionUtils
    {
        private readonly Current _current;

        public PermissionUtils(Current current)
        {
            _current = current;
        }

        public bool IsAdminOrDataSteward => _current.Roles.Contains(Role.Admin) || _current.Roles.Contains(Role.DataSteward);

        public bool FilterUnpublishedDatasets => !IsAdminOrDataSteward;

        public bool IsAdmin => _current.Roles.Contains(Role.Admin);
    }
}