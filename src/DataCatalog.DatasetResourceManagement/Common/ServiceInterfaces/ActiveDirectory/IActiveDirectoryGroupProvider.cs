using System.Threading.Tasks;

namespace DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.ActiveDirectory
{
    public interface IActiveDirectoryGroupProvider
    {
        Task<string> ProvideGroupAsync(string displayName, string description, string[] members = null);
    }
}