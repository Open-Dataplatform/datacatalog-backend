using Azure.Storage.Files.DataLake;

namespace DataCatalog.DatasetResourceManagement.Services.Storage
{
    public interface ILeaseClientProvider
    {
        DataLakeLeaseClient ProvideDataLakeLeaseClient(DataLakeFileSystemClient client);
        DataLakeLeaseClient ProvideDataLakeLeaseClient(DataLakeDirectoryClient client);
    }
}