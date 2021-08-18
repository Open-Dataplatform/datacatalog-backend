using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Azure;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using DataCatalog.Api.Services.Storage;
using DataCatalog.Common.UnitTests.AutoMoqAttribute;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace DataCatalog.Api.UnitTests.Services.Storage
{
    // ReSharper disable once InconsistentNaming
    public class AzureStorageService_Should
    {
        [Theory]
        [MoqAutoData]
        public async Task Correctly_Return_Metadata_On_GetDirectoryMetadataAsync(
            Mock<DataLakeDirectoryClient> dataLakeDirectoryClientMock,
            Mock<DataLakeFileSystemClient> dataLakeFileSystemClientMock,
            Mock<ILogger<AzureStorageService>> loggerMock,
            Mock<DataLakeServiceClient> dataLakeServiceClientMock,
            string path)
        {
            // Arrange
            var responseMock = CreatePathPropertiesResponseMock();
                
            dataLakeFileSystemClientMock
                .Setup(x => x.GetDirectoryClient(path))
                .Returns(dataLakeDirectoryClientMock.Object);

            dataLakeServiceClientMock
                .Setup(x => x.GetFileSystemClient("datasets"))
                .Returns(dataLakeFileSystemClientMock.Object);

            dataLakeDirectoryClientMock.Setup(x => x.GetPropertiesAsync(null, default))
                .Returns(Task.FromResult(responseMock.Object));
            
            var sut = new AzureStorageService(dataLakeServiceClientMock.Object, loggerMock.Object);

            // Act
            var metaData = await sut.GetDirectoryMetadataWithRetry(path);

            // Assert
            dataLakeDirectoryClientMock.Verify(x => x.GetPropertiesAsync(null, default), Times.Once);
            metaData.ShouldBe(responseMock.Object.Value.Metadata);
        }

        [Theory]
        [MoqAutoData]
        public async Task Return_Null_When_RequestFailedException_On_GetDirectoryMetadataAsync(
            Mock<DataLakeDirectoryClient> dataLakeDirectoryClientMock,
            Mock<DataLakeFileSystemClient> dataLakeFileSystemClientMock,
            Mock<ILogger<AzureStorageService>> loggerMock,
            Mock<DataLakeServiceClient> dataLakeServiceClientMock,
            RequestFailedException rfe,
            string path)
        {
            // Arrange
            dataLakeFileSystemClientMock
                .Setup(x => x.GetDirectoryClient(path))
                .Returns(dataLakeDirectoryClientMock.Object);

            dataLakeServiceClientMock
                .Setup(x => x.GetFileSystemClient("datasets"))
                .Returns(dataLakeFileSystemClientMock.Object);

            dataLakeDirectoryClientMock.Setup(x => x.GetPropertiesAsync(null, default))
                .ThrowsAsync(rfe);

            var sut = new AzureStorageService(dataLakeServiceClientMock.Object, loggerMock.Object);

            // Act
            var metaData = await sut.GetDirectoryMetadataWithRetry(path);

            // Assert
            dataLakeDirectoryClientMock.Verify(x => x.GetPropertiesAsync(null, default), Times.AtLeastOnce);
            metaData.ShouldBeNull();
        }

        [Theory]
        [MoqAutoData]
        public async Task Throw_When_Unexpected_Exception_On_GetDirectoryMetadataAsync(
            Mock<DataLakeDirectoryClient> dataLakeDirectoryClientMock,
            Mock<DataLakeFileSystemClient> dataLakeFileSystemClientMock,
            Mock<ILogger<AzureStorageService>> loggerMock,
            Mock<DataLakeServiceClient> dataLakeServiceClientMock,
            Exception e,
            string path)
        {
            // Arrange
            dataLakeFileSystemClientMock
                .Setup(x => x.GetDirectoryClient(path))
                .Returns(dataLakeDirectoryClientMock.Object);

            dataLakeServiceClientMock
                .Setup(x => x.GetFileSystemClient("datasets"))
                .Returns(dataLakeFileSystemClientMock.Object);

            dataLakeDirectoryClientMock.Setup(x => x.GetPropertiesAsync(null, default))
                .ThrowsAsync(e);

            var sut = new AzureStorageService(dataLakeServiceClientMock.Object, loggerMock.Object);

            // Act
            var ex = await Assert.ThrowsAsync<Exception>(() => sut.GetDirectoryMetadataWithRetry(path));

            // Assert
            dataLakeDirectoryClientMock.Verify(x => x.GetPropertiesAsync(null, default), Times.AtLeastOnce);
            ex.ShouldBe(e);
        }

        private Mock<Response<PathProperties>> CreatePathPropertiesResponseMock()
        {
            IFixture fixture = new Fixture();

            var responseMock = new Mock<Response<PathProperties>>();
            var pathProperties = DataLakeModelFactory.PathProperties(fixture.Create<DateTimeOffset>(),
                fixture.Create<DateTimeOffset>(), fixture.Create<IDictionary<string, string>>(),
                fixture.Create<DateTimeOffset>(), fixture.Create<string>(), fixture.Create<string>(),
                fixture.Create<string>(), fixture.Create<Uri>(), fixture.Create<CopyStatus>(), fixture.Create<bool>(),
                fixture.Create<DataLakeLeaseDuration>(), fixture.Create<DataLakeLeaseState>(),
                fixture.Create<DataLakeLeaseStatus>(), fixture.Create<long>(), fixture.Create<string>(),
                fixture.Create<ETag>(), fixture.Create<byte[]>(), fixture.Create<string>(), fixture.Create<string>(),
                fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>(), fixture.Create<bool>(),
                fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>(),
                fixture.Create<DateTimeOffset>());

            responseMock.SetupGet(x => x.Value).Returns(pathProperties);

            return responseMock;
        }
    }
}
