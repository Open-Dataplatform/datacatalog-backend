using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataCatalog.Api.Services.Storage
{
    public interface IStorageService
    {
        Task<IDictionary<string, string>> GetDirectoryMetadataAsync(string path);
    }
}