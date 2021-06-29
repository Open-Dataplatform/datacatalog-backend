using Azure.Storage.Files.DataLake;

namespace DataCatalog.DatasetResourceManagement.Services.Storage
{
    public class DataLakeLeaseClientProvider : ILeaseClientProvider
    {
        public DataLakeLeaseClient ProvideDataLakeLeaseClient(DataLakeFileSystemClient client)
        {
            return client.GetDataLakeLeaseClient();
        }

        public DataLakeLeaseClient ProvideDataLakeLeaseClient(DataLakeDirectoryClient client)
        {
            return client.GetDataLakeLeaseClient();
        }
    }
}