using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Api.Repositories;
using DataCatalog.Api.Services.AD;
using DataCatalog.Api.Services.Storage;
using DataCatalog.Common.Enums;
using DataCatalog.Common.Utils;
using Polly;

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

        public Task<IDictionary<string, string>> GetDirectoryMetadataWithRetry(Guid datasetId)
        {
            IDictionary<string, string> directory = new Dictionary<string, string>();
            directory.Add(GroupConstants.ReaderGroup, datasetId.ToString());
            directory.Add(GroupConstants.WriterGroup, datasetId.ToString());
            return Task.FromResult(directory);
        }
       
    }
}