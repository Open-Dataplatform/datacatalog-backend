using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Api.Services.AD;
using DataCatalog.Api.Services.Storage;
using DataCatalog.Common.Utils;

namespace DataCatalog.Api.Services.Local
{
    public class LocalStorageService : IStorageService
    {
        public LocalStorageService()
        {
            if (!EnvironmentUtil.IsDevelopment())
            {
                throw new InvalidOperationException("This class cannot be used unless the environment is local");
            }
        }
        
        public Task<IDictionary<string, string>> GetDirectoryMetadataWithRetry(string path)
        {
            IDictionary<string, string> directory = new Dictionary<string, string>();
            directory.Add(GroupConstants.ReaderGroup, path);
            directory.Add(GroupConstants.WriterGroup, path);
            return Task.FromResult(directory);
        }
    }
}