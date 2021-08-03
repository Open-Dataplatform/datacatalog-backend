using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using DataCatalog.Api.Messages;
using DataCatalog.DatasetResourceManagement.MessageHandlers;
using DataCatalog.Common.Interfaces;
using DataCatalog.Common.Utils;
using DataCatalog.DatasetResourceManagement.Commands.AccessControlList;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.ActiveDirectory;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.Storage;
using DataCatalog.DatasetResourceManagement.Messages;
using DataCatalog.DatasetResourceManagement.UnitTests.AutoMoqData;
using Moq;
using Rebus.Bus;
using Xunit;

namespace DataCatalog.DatasetResourceManagement.UnitTests
{
    // ReSharper disable once InconsistentNaming
    public class DatasetCreatedHandler_Should
    {
        private const string ContainerName = "datasets";
        
        [Theory]
        [AutoMoqFunction]
        public async Task Send_DatasetProvisioned_Event_On_Successful_Provisioning(
            [Frozen] Mock<IBus> busMock,
            DatasetCreatedMessage datasetCreated,
            DatasetCreatedHandler sut)
        {
            // Act
            await sut.Handle(datasetCreated);

            // Assert
            busMock.Verify(
                x => x.Publish(It.Is<DatasetProvisionedMessage>(a =>
                    Equals(a.DatasetId, datasetCreated.DatasetId) &&
                    Equals(a.Status, "Succeeded")), null), Times.Once);
        }

        [Theory]
        [AutoMoqFunction]
        public async Task Correctly_Call_Root_Group_Provider(
            [Frozen] Mock<IActiveDirectoryRootGroupProvider> rootGroupProviderMock,
            DatasetCreatedMessage datasetCreated,
            DatasetCreatedHandler sut)
        {
            // Arrange
            
            // Act
            await sut.Handle(datasetCreated);

            // Assert
            rootGroupProviderMock.Verify(
                x => x.ProvideGroupAsync(
                    $"DataPlatform-{ContainerName}-Zone-{EnvironmentUtil.GetCurrentEnvironment()}-Reader", 
                    $"Root reader group for the {ContainerName} zone",
                    ContainerName),
                Times.Once);
        }

        [Theory]
        [AutoMoqFunction]
        public async Task Correctly_Create_Directory_In_Container(
            [Frozen] Mock<IStorageService> storageServiceMock,
            DatasetCreatedMessage datasetCreated,
            DatasetCreatedHandler sut)
        {
            // Arrange
            var path = datasetCreated.DatasetId.ToString();

            // Act
            await sut.Handle(datasetCreated);

            // Assert
            storageServiceMock.Verify(x => x.CreateDirectoryIfNeeded(ContainerName, path), Times.Once);
        }

        [Theory]
        [AutoMoqFunction]
        public async Task Correctly_Acquire_Lease_On_Container_Path(
            [Frozen] Mock<IStorageService> storageServiceMock,
            DatasetCreatedMessage datasetCreated,
            DatasetCreatedHandler sut)
        {
            // Arrange
            var path = datasetCreated.DatasetId.ToString();

            // Act
            await sut.Handle(datasetCreated);

            // Assert
            storageServiceMock.Verify(x => x.AcquireLeaseAsync(ContainerName, path), Times.Once);
        }

        [Theory]
        [AutoMoqFunction]
        public async Task Correctly_Call_Group_Provider_For_Read_And_Write_Groups(
            [Frozen] Mock<IActiveDirectoryGroupProvider> groupProviderMock,
            DatasetCreatedMessage datasetCreated,
            DatasetCreatedHandler sut)
        {
            // Act
            await sut.Handle(datasetCreated);

            // Assert
            groupProviderMock.Verify(
                x => x.ProvideGroupAsync(
                    $"DataPlatform-DataSet_{datasetCreated.DatasetId}-{EnvironmentUtil.GetCurrentEnvironment()}-Reader",
                    $"Reader group with id {datasetCreated.DatasetId} and name at creation {datasetCreated.DatasetName}",
                    It.Is<string[]>(a => Equals(a.Length, 1) && a.Contains(datasetCreated.Owner))),
                Times.Once);

            groupProviderMock.Verify(
                x => x.ProvideGroupAsync(
                    $"DataPlatform-DataSet_{datasetCreated.DatasetId}-{EnvironmentUtil.GetCurrentEnvironment()}-Writer",
                    $"Writer group with id {datasetCreated.DatasetId} and name at creation {datasetCreated.DatasetName}",null),
                Times.Once);
        }

        [Theory]
        [AutoMoqFunction]
        public async Task Correctly_Add_Read_And_Write_Groups_To_Directory_Acl(
            string readerGroupId,
            string writerGroupId,
            ILease lease,
            [Frozen] Mock<IAccessControlListService> accessControlListServiceMock,
            [Frozen] Mock<IStorageService> storageServiceMock,
            [Frozen] Mock<IActiveDirectoryGroupProvider> groupProviderMock,
            DatasetCreatedMessage datasetCreated,
            DatasetCreatedHandler sut)
        {
            // Arrange
            var path = datasetCreated.DatasetId.ToString();

            storageServiceMock.Setup(x => x.AcquireLeaseAsync(ContainerName, path)).ReturnsAsync(lease);

            groupProviderMock.Setup(
                x => x.ProvideGroupAsync(
                    $"DataPlatform-DataSet_{datasetCreated.DatasetId}-{EnvironmentUtil.GetCurrentEnvironment()}-Reader",
                    $"Reader group with id {datasetCreated.DatasetId} and name at creation {datasetCreated.DatasetName}",
                    It.Is<string[]>(a => Equals(a.Length, 1) && a.Contains(datasetCreated.Owner)))).ReturnsAsync(readerGroupId);

            groupProviderMock.Setup(
                x => x.ProvideGroupAsync(
                    $"DataPlatform-DataSet_{datasetCreated.DatasetId}-{EnvironmentUtil.GetCurrentEnvironment()}-Writer",
                    $"Writer group with id {datasetCreated.DatasetId} and name at creation {datasetCreated.DatasetName}",
                    null)).ReturnsAsync(
                writerGroupId);

            // Act
            await sut.Handle(datasetCreated);

            // Assert
            accessControlListServiceMock.Verify(
                x => x.SetGroupsInAccessControlListAsync(
                    It.Is<CreateGroupsInAccessControlList>(a => 
                        Equals(a.StorageContainer, ContainerName) && 
                        Equals(a.Path, path) &&
                        Equals(a.GroupEntries.Count(), 4) &&
                        a.GroupEntries.Any(entry => Equals(entry.Id, readerGroupId) && Equals(entry.IsDefault, false) && Equals(entry.Permissions, "r-x")) &&
                        a.GroupEntries.Any(entry => Equals(entry.Id, readerGroupId) && Equals(entry.IsDefault, true) && Equals(entry.Permissions, "r-x"))  &&
                        a.GroupEntries.Any(entry => Equals(entry.Id, writerGroupId) && Equals(entry.IsDefault, false) && Equals(entry.Permissions, "rwx")) &&
                        a.GroupEntries.Any(entry => Equals(entry.Id, writerGroupId) && Equals(entry.IsDefault, true) && Equals(entry.Permissions, "rwx"))),
                    lease.LeaseId), Times.Once);
        }

        [Theory]
        [AutoMoqFunction]
        public async Task Correctly_Add_Read_And_Write_Groups_And_DatasetId_To_Directory_Metadata(
            string readerGroupId,
            string writerGroupId,
            ILease lease,
            [Frozen] Mock<IStorageService> storageServiceMock,
            [Frozen] Mock<IActiveDirectoryGroupProvider> groupProviderMock,
            DatasetCreatedMessage datasetCreated,
            DatasetCreatedHandler sut)
        {
            // Arrange
            var path = datasetCreated.DatasetId.ToString();

            storageServiceMock.Setup(x => x.AcquireLeaseAsync(ContainerName, path)).ReturnsAsync(lease);

            groupProviderMock.Setup(
                x => x.ProvideGroupAsync(
                    $"DataPlatform-DataSet_{datasetCreated.DatasetId}-{EnvironmentUtil.GetCurrentEnvironment()}-Reader",
                    $"Reader group with id {datasetCreated.DatasetId} and name at creation {datasetCreated.DatasetName}",
                    It.Is<string[]>(a => Equals(a.Length, 1) && a.Contains(datasetCreated.Owner)))).ReturnsAsync(readerGroupId);

            groupProviderMock.Setup(
                x => x.ProvideGroupAsync(
                    $"DataPlatform-DataSet_{datasetCreated.DatasetId}-{EnvironmentUtil.GetCurrentEnvironment()}-Writer",
                    $"Writer group with id {datasetCreated.DatasetId} and name at creation {datasetCreated.DatasetName}",
                    null)).ReturnsAsync(writerGroupId);

            // Act
            await sut.Handle(datasetCreated);

            // Assert
            storageServiceMock.Verify(x =>
                    x.SetDirectoryMetadata(ContainerName, path,
                        It.Is<IDictionary<string, string>>(a =>
                            Equals(a["ReaderGroupId"], readerGroupId) && 
                            Equals(a["WriterGroupId"], writerGroupId) &&
                            Equals(a["RootDirectoryDatasetId"], path)),
                        lease.LeaseId),
                Times.Once);
        }

        [Theory]
        [AutoMoqFunction]
        public async Task Correctly_Remove_Root_Group_As_Default_Acl_Entry_On_Directory(
            string rootGroupId,
            ILease lease,
            [Frozen] Mock<IStorageService> storageServiceMock,
            [Frozen] Mock<IAccessControlListService> accessControlListServiceMock,
            [Frozen] Mock<IActiveDirectoryRootGroupProvider> rootGroupProviderMock,
            DatasetCreatedMessage datasetCreated,
            DatasetCreatedHandler sut)
        {
            // Arrange
            var path = datasetCreated.DatasetId.ToString();

            storageServiceMock.Setup(x => x.AcquireLeaseAsync(ContainerName, path)).ReturnsAsync(lease);

            rootGroupProviderMock.Setup(
                x => x.ProvideGroupAsync(
                    $"DataPlatform-{ContainerName}-Zone-{EnvironmentUtil.GetCurrentEnvironment()}-Reader",
                    $"Root reader group for the {ContainerName} zone",
                    ContainerName)).ReturnsAsync(rootGroupId);

            // Act
            await sut.Handle(datasetCreated);

            // Assert
            accessControlListServiceMock.Verify(x =>
                x.RemoveGroupFromAccessControlListAsync(ContainerName, path, rootGroupId, lease.LeaseId), Times.Once);
        }

        [Theory]
        [AutoMoqFunction]
        public async Task Correctly_Add_Read_And_Write_Groups_As_Members_To_Root_Group(
            string rootGroupId,
            string readerGroupId,
            string writerGroupId,
            [Frozen] Mock<IActiveDirectoryGroupProvider> groupProviderMock,
            [Frozen] Mock<IActiveDirectoryRootGroupProvider> rootGroupProviderMock,
            [Frozen] Mock<IActiveDirectoryGroupService> adGroupService,
            DatasetCreatedMessage datasetCreated,
            DatasetCreatedHandler sut)
        {
            // Arrange
            groupProviderMock.Setup(
                x => x.ProvideGroupAsync(
                    $"DataPlatform-DataSet_{datasetCreated.DatasetId}-{EnvironmentUtil.GetCurrentEnvironment()}-Reader",
                    $"Reader group with id {datasetCreated.DatasetId} and name at creation {datasetCreated.DatasetName}",
                    It.Is<string[]>(a => Equals(a.Length, 1) && a.Contains(datasetCreated.Owner)))).ReturnsAsync(readerGroupId);

            groupProviderMock.Setup(
                x => x.ProvideGroupAsync(
                    $"DataPlatform-DataSet_{datasetCreated.DatasetId}-{EnvironmentUtil.GetCurrentEnvironment()}-Writer",
                    $"Writer group with id {datasetCreated.DatasetId} and name at creation {datasetCreated.DatasetName}",
                    null)).ReturnsAsync(writerGroupId);

            rootGroupProviderMock.Setup(
                x => x.ProvideGroupAsync(
                    $"DataPlatform-{ContainerName}-Zone-{EnvironmentUtil.GetCurrentEnvironment()}-Reader",
                    $"Root reader group for the {ContainerName} zone",
                    ContainerName)).ReturnsAsync(rootGroupId);

            // Act
            await sut.Handle(datasetCreated);

            // Assert
            adGroupService.Verify(x => x.AddGroupMember(rootGroupId, readerGroupId), Times.Once);
            adGroupService.Verify(x => x.AddGroupMember(rootGroupId, writerGroupId), Times.Once);
        }

        [Theory]
        [AutoMoqFunction]
        public async Task Correctly_Add_All_Users_Group_As_Member_To_Read_Group_When_Dataset_Is_Public(
            string readerGroupId,
            [Frozen] Mock<IActiveDirectoryGroupProvider> groupProviderMock,
            [Frozen] Mock<IActiveDirectoryGroupService> adGroupService,
            [Frozen] Mock<IAllUsersGroupProvider> allUsersGroupProviderMock,
            string allUsersGroupId,
            DatasetCreatedMessage datasetCreated,
            DatasetCreatedHandler sut)
        {
            // Arrange
            datasetCreated.Public = true;

            allUsersGroupProviderMock.Setup(x => x.GetAllUsersGroup()).Returns(allUsersGroupId);
            
            groupProviderMock.Setup(
                x => x.ProvideGroupAsync(
                    $"DataPlatform-DataSet_{datasetCreated.DatasetId}-{EnvironmentUtil.GetCurrentEnvironment()}-Reader",
                    $"Reader group with id {datasetCreated.DatasetId} and name at creation {datasetCreated.DatasetName}",
                    It.Is<string[]>(a => Equals(a.Length, 1) && a.Contains(datasetCreated.Owner)))).ReturnsAsync(readerGroupId);

            // Act
            await sut.Handle(datasetCreated);

            // Assert
            adGroupService.Verify(x => x.AddGroupMember(readerGroupId, allUsersGroupId), Times.Once);
        }

        [Theory]
        [AutoMoqFunction]
        public async Task Not_Add_All_Users_Group_As_Member_To_Read_Group_When_Dataset_Is_Not_Public(
            string readerGroupId,
            [Frozen] Mock<IActiveDirectoryGroupProvider> groupProviderMock,
            [Frozen] Mock<IActiveDirectoryGroupService> adGroupService,
            [Frozen] Mock<IAllUsersGroupProvider> allUsersGroupProviderMock,
            string allUsersGroupId,
            DatasetCreatedMessage datasetCreated,
            DatasetCreatedHandler sut)
        {
            // Arrange
            datasetCreated.Public = false;

            allUsersGroupProviderMock.Setup(x => x.GetAllUsersGroup()).Returns(allUsersGroupId);

            groupProviderMock.Setup(
                x => x.ProvideGroupAsync(
                    $"DataPlatform-DataSet_{datasetCreated.DatasetId}-{EnvironmentUtil.GetCurrentEnvironment()}-Reader",
                    $"Reader group with id {datasetCreated.DatasetId} and name at creation {datasetCreated.DatasetName}",
                    It.Is<string[]>(a => Equals(a.Length, 1) && a.Contains(datasetCreated.Owner)))).ReturnsAsync(readerGroupId);

            // Act
            await sut.Handle(datasetCreated);

            // Assert
            adGroupService.Verify(x => x.AddGroupMember(readerGroupId, allUsersGroupId), Times.Never);
        }
    }
}
