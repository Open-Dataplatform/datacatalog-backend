using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using DataCatalog.DatasetResourceManagement.Commands.AccessControlList;
using DataCatalog.DatasetResourceManagement.Commands.Group;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.ActiveDirectory;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.Storage;
using DataCatalog.DatasetResourceManagement.Responses.Group;
using DataCatalog.DatasetResourceManagement.Services.ActiveDirectory;
using DataCatalog.DatasetResourceManagement.UnitTests.AutoMoqData;
using Moq;
using Shouldly;
using Xunit;

namespace DataCatalog.DatasetResourceManagement.UnitTests.Services.ActiveDirectory
{
    // ReSharper disable once InconsistentNaming
    public class AadRootGroupProvider_Should
    {
        [Theory]
        [AutoMoqInfrastructure]
        public async Task Correctly_Create_Group_If_None_Exists(
            [Frozen] Mock<IActiveDirectoryGroupService> adGroupServiceMock,
            CreateGroupResponse createGroupResponse,
            string displayName,
            string description,
            string leaseContainer,
            AzureActiveDirectoryRootGroupProvider sut)
        {
            // Arrange
            adGroupServiceMock.Setup(x => x.GetGroupAsync($"SEC-A-ENDK-{displayName}"))
                .Returns(Task.FromResult<GetGroupResponse>(null));

            adGroupServiceMock.Setup(
                x => x.CreateGroupAsync(It.Is<CreateGroup>(a =>
                    Equals(a.DisplayName, displayName) &&
                    Equals(a.Description, description) &&
                    Equals(a.MailNickname, "dataplatform")))).ReturnsAsync(createGroupResponse);

            // Act
            var groupId = await sut.ProvideGroupAsync(displayName, description, leaseContainer);

            // Assert
            adGroupServiceMock.Verify(
                x => x.CreateGroupAsync(It.Is<CreateGroup>(a =>
                    Equals(a.DisplayName, displayName) &&
                    Equals(a.Description, description) &&
                    Equals(a.MailNickname, "dataplatform"))), Times.Once);

            groupId.ShouldBe(createGroupResponse.Id);
        }

        [Theory]
        [AutoMoqInfrastructure]
        public async Task Return_Existing_Group_If_Exists(
            [Frozen] Mock<IActiveDirectoryGroupService> adGroupServiceMock,
            GetGroupResponse getGroupResponse,
            string displayName,
            string description,
            string leaseContainer,
            AzureActiveDirectoryRootGroupProvider sut)
        {
            // Arrange
            adGroupServiceMock.Setup(x => x.GetGroupAsync($"SEC-A-ENDK-{displayName}"))
                .Returns(Task.FromResult(getGroupResponse));

            // Act
            var groupId = await sut.ProvideGroupAsync(displayName, description, leaseContainer);

            // Assert
            adGroupServiceMock.Verify(x => x.GetGroupAsync($"SEC-A-ENDK-{displayName}"), Times.Once);
            adGroupServiceMock.Verify(x => x.CreateGroupAsync(It.IsAny<CreateGroup>()), Times.Never);

            groupId.ShouldBe(getGroupResponse.Id);
        }

        [Theory]
        [AutoMoqInfrastructure]
        public async Task Correctly_Acquire_And_Release_Lease_When_Creating_Group(
            [Frozen] Mock<IActiveDirectoryGroupService> adGroupServiceMock,
            [Frozen] Mock<IStorageService> storageServiceMock,
            Mock<ILease> leaseMock,
            string displayName,
            string description,
            string leaseContainer,
            AzureActiveDirectoryRootGroupProvider sut)
        {
            // Arrange
            adGroupServiceMock.Setup(x => x.GetGroupAsync($"SEC-A-ENDK-{displayName}"))
                .Returns(Task.FromResult<GetGroupResponse>(null));
            storageServiceMock.Setup(x => x.AcquireLeaseAsync(leaseContainer, null)).ReturnsAsync(leaseMock.Object);

            // Act
            await sut.ProvideGroupAsync(displayName, description, leaseContainer);

            // Assert
            storageServiceMock.Verify(x => x.AcquireLeaseAsync(leaseContainer, null), Times.Once);
            leaseMock.Verify(x => x.DisposeAsync(), Times.Once);
        }

        [Theory]
        [AutoMoqInfrastructure]
        public async Task Correctly_Update_Acls_When_Group_Is_Created(
            [Frozen] Mock<IActiveDirectoryGroupService> adGroupServiceMock,
            [Frozen] Mock<IAccessControlListService> accessControlListServiceMock,
            CreateGroupResponse createGroupResponse,
            string displayName,
            string description,
            string leaseContainer,
            AzureActiveDirectoryRootGroupProvider sut)
        {
            // Arrange
            adGroupServiceMock.Setup(x => x.GetGroupAsync($"SEC-A-ENDK-{displayName}"))
                .Returns(Task.FromResult<GetGroupResponse>(null));

            adGroupServiceMock.Setup(
                x => x.CreateGroupAsync(It.Is<CreateGroup>(a =>
                    Equals(a.DisplayName, displayName) &&
                    Equals(a.Description, description) &&
                    Equals(a.MailNickname, "dataplatform")))).ReturnsAsync(createGroupResponse);

            // Act
            await sut.ProvideGroupAsync(displayName, description, leaseContainer);

            // Assert
            adGroupServiceMock.Verify(x => x.CreateGroupAsync(It.IsAny<CreateGroup>()), Times.Once);
            accessControlListServiceMock.Verify(
                x => x.IsGroupInAccessControlListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            accessControlListServiceMock.Verify(x => x.SetGroupsInAccessControlListAsync(
                    It.Is<CreateGroupsInAccessControlList>(a => 
                        Equals(a.Path, "/") &&
                        Equals(a.StorageContainer, leaseContainer) &&
                        Equals(a.GroupEntries.Count(), 2) &&
                        a.GroupEntries.SingleOrDefault(entry => entry.IsDefault && Equals(entry.Id, createGroupResponse.Id) && Equals(entry.Permissions, "r-x")) != null &&
                        a.GroupEntries.SingleOrDefault(entry => !entry.IsDefault && Equals(entry.Id, createGroupResponse.Id) && Equals(entry.Permissions, "r-x")) != null &&
                        Equals(a.StorageContainer, leaseContainer)), null),
                Times.Once);
        }

        [Theory]
        [AutoMoqInfrastructure]
        public async Task Correctly_Update_Acls_When_Group_Is_Not_Created_And_Group_Is_Not_Already_In_Acl(
            [Frozen] Mock<IActiveDirectoryGroupService> adGroupServiceMock,
            [Frozen] Mock<IAccessControlListService> accessControlListServiceMock,
            GetGroupResponse getGroupResponse,
            string displayName,
            string description,
            string leaseContainer,
            AzureActiveDirectoryRootGroupProvider sut)
        {
            // Arrange
            adGroupServiceMock.Setup(x => x.GetGroupAsync($"SEC-A-ENDK-{displayName}"))
                .ReturnsAsync(getGroupResponse);
            accessControlListServiceMock.Setup(x => x.IsGroupInAccessControlListAsync(getGroupResponse.Id, leaseContainer, "/"))
                .ReturnsAsync(false);

            // Act
            await sut.ProvideGroupAsync(displayName, description, leaseContainer);

            // Assert
            adGroupServiceMock.Verify(x => x.CreateGroupAsync(It.IsAny<CreateGroup>()), Times.Never);
            accessControlListServiceMock.Verify(x => x.IsGroupInAccessControlListAsync(getGroupResponse.Id, leaseContainer, "/"), Times.Once);

            accessControlListServiceMock.Verify(x => x.SetGroupsInAccessControlListAsync(
                    It.Is<CreateGroupsInAccessControlList>(a =>
                        Equals(a.Path, "/") &&
                        Equals(a.StorageContainer, leaseContainer) &&
                        Equals(a.GroupEntries.Count(), 2) &&
                        a.GroupEntries.SingleOrDefault(entry => entry.IsDefault && Equals(entry.Id, getGroupResponse.Id) && Equals(entry.Permissions, "r-x")) != null &&
                        a.GroupEntries.SingleOrDefault(entry => !entry.IsDefault && Equals(entry.Id, getGroupResponse.Id) && Equals(entry.Permissions, "r-x")) != null &&
                        Equals(a.StorageContainer, leaseContainer)), null),
                Times.Once);
        }

        [Theory]
        [AutoMoqInfrastructure]
        public async Task Do_Not_Update_Acls_When_Group_Is_Not_Created_And_Group_Is_Already_In_Acl(
            [Frozen] Mock<IActiveDirectoryGroupService> adGroupServiceMock,
            [Frozen] Mock<IAccessControlListService> accessControlListServiceMock,
            GetGroupResponse getGroupResponse,
            string displayName,
            string description,
            string leaseContainer,
            AzureActiveDirectoryRootGroupProvider sut)
        {
            // Arrange
            adGroupServiceMock.Setup(x => x.GetGroupAsync($"SEC-A-ENDK-{displayName}"))
                .ReturnsAsync(getGroupResponse);
            accessControlListServiceMock.Setup(x => x.IsGroupInAccessControlListAsync(getGroupResponse.Id, leaseContainer, "/"))
                .ReturnsAsync(true);

            // Act
            await sut.ProvideGroupAsync(displayName, description, leaseContainer);

            // Assert
            adGroupServiceMock.Verify(x => x.CreateGroupAsync(It.IsAny<CreateGroup>()), Times.Never);
            accessControlListServiceMock.Verify(x => x.IsGroupInAccessControlListAsync(getGroupResponse.Id, leaseContainer, "/"), Times.Once);

            accessControlListServiceMock.Verify(x => x.SetGroupsInAccessControlListAsync(
                    It.IsAny<CreateGroupsInAccessControlList>(), null), Times.Never);
        }

        [Theory]
        [AutoMoqInfrastructure]
        public async Task Throw_Null_Reference_Exception_When_Create_Group_Fails(
            [Frozen] Mock<IActiveDirectoryGroupService> aadGroupServiceMock,
            string displayName,
            string description,
            string leaseContainer,
            AzureActiveDirectoryRootGroupProvider sut)
        {
            // Arrange
            aadGroupServiceMock.Setup(x => x.GetGroupAsync($"SEC-A-ENDK-{displayName}"))
                .Returns(Task.FromResult<GetGroupResponse>(null));

            aadGroupServiceMock.Setup(
                x => x.CreateGroupAsync(It.Is<CreateGroup>(a =>
                    Equals(a.DisplayName, displayName) &&
                    Equals(a.Description, description) &&
                    Equals(a.MailNickname, "dataplatform"))))
                .Returns(Task.FromResult<CreateGroupResponse>(null));

            // Act
            await Assert.ThrowsAsync<NullReferenceException>(() => sut.ProvideGroupAsync(displayName, description, leaseContainer));

            // Assert
            aadGroupServiceMock.Verify(
                x => x.CreateGroupAsync(It.Is<CreateGroup>(a =>
                    Equals(a.DisplayName, displayName) &&
                    Equals(a.Description, description) &&
                    Equals(a.MailNickname, "dataplatform"))), Times.Once);
        }
    }
}
