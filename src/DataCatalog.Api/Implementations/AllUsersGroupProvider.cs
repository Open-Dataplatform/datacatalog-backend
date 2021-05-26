using DataCatalog.Common.Interfaces;

namespace DataCatalog.Api.Implementations
{
    public class AllUsersGroupProvider : IAllUsersGroupProvider
    {
        private readonly string _allUsersGroup;

        public AllUsersGroupProvider(string allUsersGroup)
        {
            _allUsersGroup = allUsersGroup;
        }

        public string GetAllUsersGroup()
        {
            return _allUsersGroup;
        }
    }
}
