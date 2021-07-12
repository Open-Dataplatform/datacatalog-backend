using System.Threading.Tasks;

namespace DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.ActiveDirectory
{
    public interface IActiveDirectoryRootGroupProvider
    {
        Task<string> ProvideGroupAsync(string displayName, string description, string leaseContainer);
    }
}