using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Api.Services.AD;
using DataCatalog.Api.Services.Storage;

namespace DataCatalog.Api.Services.Local
{
    public class LocalStorageService : IStorageService
    {
        public Task<IDictionary<string, string>> GetDirectoryMetadataAsync(string path)
        {
            IDictionary<string, string> directory = new Dictionary<string, string>();
            directory.Add(GroupConstants.ReaderGroup, path);
            directory.Add(GroupConstants.WriterGroup, path);
            return Task.FromResult(directory);
        }
    }
}