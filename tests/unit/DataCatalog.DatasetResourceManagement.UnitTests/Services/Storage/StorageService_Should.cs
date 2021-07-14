using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using DataCatalog.DatasetResourceManagement.Services.Storage;
using DataCatalog.DatasetResourceManagement.UnitTests.AutoMoqData;
using DataCatalog.DatasetResourceManagement.UnitTests.Logger;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace DataCatalog.DatasetResourceManagement.UnitTests.Services.Storage
{
    // ReSharper disable once InconsistentNaming
    public class StorageService_Should
    {
        [Theory]
        [MoqAutoData]
        public async Task Correctly_Create_Directory_On_CreateDirectoryIfNeeded(
            Mock<DataLakeDirectoryClient> dataLakeDirectoryClientMock,
            Mock<DataLakeFileSystemClient> dataLakeFileSystemClientMock,
            Mock<ILeaseClientProvider> leaseClientProviderMock,
            Mock<ILogger<StorageService>> loggerMock,
            Mock<DataLakeServiceClient> dataLakeServiceClientMock,
            string storageContainer,
            string path)
        {
            // Arrange
            dataLakeFileSystemClientMock
                .Setup(x => x.GetDirectoryClient(path))
                .Returns(dataLakeDirectoryClientMock.Object);
            
            dataLakeServiceClientMock
                .Setup(x => x.GetFileSystemClient(storageContainer.ToLower()))
                .Returns(dataLakeFileSystemClientMock.Object);

            var sut = new StorageService(loggerMock.Object, dataLakeServiceClientMock.Object, leaseClientProviderMock.Object);

            // Act
            await sut.CreateDirectoryIfNeeded(storageContainer, path);

            // Assert
            loggerMock.VerifyLogWasCalled(LogLevel.Information, $"Creating: {storageContainer}/{path} if needed");
            dataLakeDirectoryClientMock.Verify(x => x.CreateIfNotExistsAsync(default, default, default, default, default), Times.Once());
            dataLakeFileSystemClientMock.Verify(x => x.GetDirectoryClient(path), Times.Once);
            dataLakeServiceClientMock.Verify(x => x.GetFileSystemClient(storageContainer.ToLower()), Times.Once);
        }

        [Theory]
        [MoqAutoData]
        public async Task Correctly_Acquire_Lease_On_Directory(
            Mock<DataLakeDirectoryClient> dataLakeDirectoryClientMock,
            Mock<DataLakeFileSystemClient> dataLakeFileSystemClientMock,
            Mock<ILeaseClientProvider> leaseClientProviderMock,
            Mock<ILogger<StorageService>> loggerMock,
            Mock<DataLakeServiceClient> dataLakeServiceClientMock,
            Mock<DataLakeLeaseClient> dataLakeLeaseClientMock,
            string storageContainer,
            string path)
        {
            // Arrange
            dataLakeFileSystemClientMock
                .Setup(x => x.GetDirectoryClient(path))
                .Returns(dataLakeDirectoryClientMock.Object);

            dataLakeServiceClientMock
                .Setup(x => x.GetFileSystemClient(storageContainer.ToLower()))
                .Returns(dataLakeFileSystemClientMock.Object);
            leaseClientProviderMock.Setup(x => x.ProvideDataLakeLeaseClient(dataLakeDirectoryClientMock.Object))
                .Returns(dataLakeLeaseClientMock.Object);

            var sut = new StorageService(loggerMock.Object, dataLakeServiceClientMock.Object, leaseClientProviderMock.Object);

            // Act
            var lease = await sut.AcquireLeaseAsync(storageContainer,path);

            // Assert
            leaseClientProviderMock.Verify(x => x.ProvideDataLakeLeaseClient(dataLakeDirectoryClientMock.Object), Times.Once);
            leaseClientProviderMock.Verify(x => x.ProvideDataLakeLeaseClient(dataLakeFileSystemClientMock.Object), Times.Never);
            lease.LeaseId.ShouldBe(dataLakeLeaseClientMock.Object.LeaseId);
            dataLakeFileSystemClientMock.Verify(x => x.GetDirectoryClient(path), Times.Once);
            dataLakeServiceClientMock.Verify(x => x.GetFileSystemClient(storageContainer.ToLower()), Times.Once);
        }

        [Theory]
        [MoqAutoData]
        public async Task Correctly_Acquire_Lease_On_Container(
            Mock<DataLakeDirectoryClient> dataLakeDirectoryClientMock,
            Mock<DataLakeFileSystemClient> dataLakeFileSystemClientMock,
            Mock<ILeaseClientProvider> leaseClientProviderMock,
            Mock<ILogger<StorageService>> loggerMock,
            Mock<DataLakeServiceClient> dataLakeServiceClientMock,
            Mock<DataLakeLeaseClient> dataLakeLeaseClientMock,
            string storageContainer,
            string path)
        {
            // Arrange
            path = null;
            dataLakeFileSystemClientMock
                .Setup(x => x.GetDirectoryClient(path))
                .Returns(dataLakeDirectoryClientMock.Object);

            dataLakeServiceClientMock
                .Setup(x => x.GetFileSystemClient(storageContainer.ToLower()))
                .Returns(dataLakeFileSystemClientMock.Object);
            leaseClientProviderMock.Setup(x => x.ProvideDataLakeLeaseClient(dataLakeFileSystemClientMock.Object))
                .Returns(dataLakeLeaseClientMock.Object);

            var sut = new StorageService(loggerMock.Object, dataLakeServiceClientMock.Object, leaseClientProviderMock.Object);

            // Act
            var lease = await sut.AcquireLeaseAsync(storageContainer, path);

            // Assert
            leaseClientProviderMock.Verify(x => x.ProvideDataLakeLeaseClient(dataLakeDirectoryClientMock.Object), Times.Never);
            leaseClientProviderMock.Verify(x => x.ProvideDataLakeLeaseClient(dataLakeFileSystemClientMock.Object), Times.Once);
            lease.LeaseId.ShouldBe(dataLakeLeaseClientMock.Object.LeaseId);
            dataLakeFileSystemClientMock.Verify(x => x.GetDirectoryClient(path), Times.Never);
            dataLakeServiceClientMock.Verify(x => x.GetFileSystemClient(storageContainer.ToLower()), Times.Once);
        }

        [Theory]
        [MoqAutoData]
        public async Task Correctly_Set_Directory_Metadata(
            Mock<DataLakeDirectoryClient> dataLakeDirectoryClientMock,
            Mock<DataLakeFileSystemClient> dataLakeFileSystemClientMock,
            Mock<ILeaseClientProvider> leaseClientProviderMock,
            Mock<ILogger<StorageService>> loggerMock,
            Mock<DataLakeServiceClient> dataLakeServiceClientMock,
            string storageContainer,
            string path,
            IDictionary<string, string> metadata,
            string leaseId)
        {
            // Arrange
            dataLakeFileSystemClientMock
                .Setup(x => x.GetDirectoryClient(path))
                .Returns(dataLakeDirectoryClientMock.Object);

            dataLakeServiceClientMock
                .Setup(x => x.GetFileSystemClient(storageContainer.ToLower()))
                .Returns(dataLakeFileSystemClientMock.Object);

            var sut = new StorageService(loggerMock.Object, dataLakeServiceClientMock.Object, leaseClientProviderMock.Object);

            // Act
            await sut.SetDirectoryMetadata(storageContainer, path, metadata, leaseId);

            // Assert
            dataLakeFileSystemClientMock.Verify(x => x.GetDirectoryClient(path), Times.Once);
            dataLakeServiceClientMock.Verify(x => x.GetFileSystemClient(storageContainer.ToLower()), Times.Once);
            dataLakeDirectoryClientMock.Verify(x => x.SetMetadataAsync(metadata,
                    It.Is<DataLakeRequestConditions>(a => Equals(a.LeaseId, leaseId)), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
