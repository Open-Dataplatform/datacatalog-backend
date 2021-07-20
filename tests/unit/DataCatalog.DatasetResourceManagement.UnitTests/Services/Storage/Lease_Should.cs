using System.Threading.Tasks;
using Azure.Storage.Files.DataLake;
using DataCatalog.DatasetResourceManagement.Services.Storage;
using DataCatalog.DatasetResourceManagement.UnitTests.AutoMoqData;
using Moq;
using Shouldly;
using Xunit;

namespace DataCatalog.DatasetResourceManagement.UnitTests.Services.Storage
{
    // ReSharper disable once InconsistentNaming
    public class Lease_Should
    {
        [Theory]
        [MoqAutoData]
        public async Task Correctly_Break_And_Release_Lease_on_DisposeAsync(
            Mock<DataLakeLeaseClient> dataLakeLeaseClientMock)
        {
            // Arrange
            var sut = new Lease(dataLakeLeaseClientMock.Object);

            // Act
            await sut.DisposeAsync();
            
            // Assert
            dataLakeLeaseClientMock.Verify(x => x.BreakAsync(null, null, default), Times.Once);
            dataLakeLeaseClientMock.Verify(x => x.ReleaseAsync(null, default), Times.Once);
        }

        [Theory]
        [MoqAutoData]
        public void Correctly_Set_LeaseId(
            Mock<DataLakeLeaseClient> dataLakeLeaseClientMock)
        {
            // Arrange
            
            // Act
            var sut = new Lease(dataLakeLeaseClientMock.Object);
            
            // Assert
            sut.LeaseId.ShouldBe(dataLakeLeaseClientMock.Object.LeaseId);
        }
    }
}