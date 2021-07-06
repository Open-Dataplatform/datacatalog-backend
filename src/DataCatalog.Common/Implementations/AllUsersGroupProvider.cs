using DataCatalog.Common.Interfaces;

namespace DataCatalog.Common.Implementations
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
