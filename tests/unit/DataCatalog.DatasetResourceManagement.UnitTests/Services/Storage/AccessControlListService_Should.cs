using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using DataCatalog.Common.UnitTests.Extensions;
using DataCatalog.DatasetResourceManagement.Commands.AccessControlList;
using DataCatalog.DatasetResourceManagement.Services.Storage;
using DataCatalog.DatasetResourceManagement.UnitTests.AutoMoqData;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace DataCatalog.DatasetResourceManagement.UnitTests.Services.Storage
{
    // ReSharper disable once InconsistentNaming
    public class AccessControlListService_Should
    {
        [Theory]
        [AutoMoqInfrastructure]
        public async Task Correctly_Remove_Group_In_Acl(
            Mock<ILogger<AccessControlListService>> loggerMock,
            Mock<DataLakeDirectoryClient> dataLakeDirectoryClientMock,
            Mock<DataLakeFileSystemClient> dataLakeFileSystemClientMock,
            Mock<DataLakeServiceClient> dataLakeServiceClientMock,
            PathAccessControl pathAccessControl,
            string storageContainer,
            string path,
            string leaseId)
        {
            // Arrange
            var entityToRemove = pathAccessControl.AccessControlList.First().EntityId;
            var responseMock = new Mock<Response<PathAccessControl>>();
            responseMock.SetupGet(x => x.Value).Returns(pathAccessControl);
            dataLakeFileSystemClientMock
                .Setup(x => x.GetDirectoryClient(path))
                .Returns(dataLakeDirectoryClientMock.Object);

            dataLakeServiceClientMock
                .Setup(x => x.GetFileSystemClient(storageContainer.ToLower()))
                .Returns(dataLakeFileSystemClientMock.Object);

            dataLakeDirectoryClientMock.Setup(x => x.GetAccessControlAsync(null, null, default))
                .ReturnsAsync(responseMock.Object);
            
            var sut = new AccessControlListService(loggerMock.Object, dataLakeServiceClientMock.Object);

            // Act
            await sut.RemoveGroupFromAccessControlListAsync(storageContainer, path, entityToRemove, leaseId);

            // Assert
            dataLakeDirectoryClientMock.Verify(
                x => x.SetAccessControlListAsync(It.Is<IList<PathAccessControlItem>>(a => !a.Any(i => Equals(i.EntityId, entityToRemove))), null, null,
                    It.Is<DataLakeRequestConditions>(a => Equals(a.LeaseId, leaseId)), default),
                Times.Once);
        }

        [Theory]
        [AutoMoqInfrastructure]
        public async Task Correctly_Set_Group_In_Acl_With_Lease(
            Mock<ILogger<AccessControlListService>> loggerMock,
            Mock<DataLakeDirectoryClient> dataLakeDirectoryClientMock,
            Mock<DataLakeFileSystemClient> dataLakeFileSystemClientMock,
            Mock<DataLakeServiceClient> dataLakeServiceClientMock,
            CreateGroupsInAccessControlList createGroupsInAccessControlList,
            string leaseId)
        {
            // Arrange
            dataLakeFileSystemClientMock
                .Setup(x => x.GetDirectoryClient(createGroupsInAccessControlList.Path))
                .Returns(dataLakeDirectoryClientMock.Object);

            dataLakeServiceClientMock
                .Setup(x => x.GetFileSystemClient(createGroupsInAccessControlList.StorageContainer.ToLower()))
                .Returns(dataLakeFileSystemClientMock.Object);

            dataLakeDirectoryClientMock.Setup(x =>
                    x.SetAccessControlListAsync(It.IsAny<IList<PathAccessControlItem>>(), null, null,
                        It.IsAny<DataLakeRequestConditions>(), default))
                .Returns(Task.FromResult<Response<PathInfo>>(null));

            var sut = new AccessControlListService(loggerMock.Object, dataLakeServiceClientMock.Object);

            // Act
            await sut.SetGroupsInAccessControlListAsync(createGroupsInAccessControlList, leaseId);

            // Assert
            dataLakeDirectoryClientMock.Verify(
                x => x.SetAccessControlListAsync(
                    It.Is<IList<PathAccessControlItem>>(a => a
                        .Where(entry => entry.AccessControlType != AccessControlType.Mask).All(entry =>
                            createGroupsInAccessControlList.GroupEntries
                                .Any(groupEntry => Equals(groupEntry.Id, entry.EntityId) &&
                                                   Equals(groupEntry.IsDefault, entry.DefaultScope) &&
                                                   Equals(groupEntry.Permissions,
                                                       entry.Permissions.ToSymbolicRolePermissions()) &&
                                                   Equals(AccessControlType.Group, entry.AccessControlType)))), null,
                    null,
                    It.Is<DataLakeRequestConditions>(a => Equals(a.LeaseId, leaseId)), default),
                Times.Once);

            // Verify default group
            dataLakeDirectoryClientMock.Verify(
                x => x.SetAccessControlListAsync(
                    It.Is<IList<PathAccessControlItem>>(a => a.SingleOrDefault(entry =>
                        entry.AccessControlType == AccessControlType.Mask &&
                        entry.Permissions.ToSymbolicRolePermissions().Equals("rwx") && 
                        entry.DefaultScope) != null), null,
                    null,
                    It.Is<DataLakeRequestConditions>(a => Equals(a.LeaseId, leaseId)), default),
                Times.Once);

            dataLakeDirectoryClientMock.Verify(
                x => x.SetAccessControlListAsync(
                    It.Is<IList<PathAccessControlItem>>(a => a.SingleOrDefault(entry =>
                        entry.AccessControlType == AccessControlType.Mask &&
                        entry.Permissions.ToSymbolicRolePermissions().Equals("rwx") &&
                        !entry.DefaultScope) != null), null,
                    null,
                    It.Is<DataLakeRequestConditions>(a => Equals(a.LeaseId, leaseId)), default),
                Times.Once);
        }

        [Theory]
        [AutoMoqInfrastructure]
        public async Task Correctly_Set_Group_In_Acl_Without_Lease(
            Mock<ILogger<AccessControlListService>> loggerMock,
            Mock<DataLakeDirectoryClient> dataLakeDirectoryClientMock,
            Mock<DataLakeFileSystemClient> dataLakeFileSystemClientMock,
            Mock<DataLakeServiceClient> dataLakeServiceClientMock,
            CreateGroupsInAccessControlList createGroupsInAccessControlList)
        {
            // Arrange
            dataLakeFileSystemClientMock
                .Setup(x => x.GetDirectoryClient(createGroupsInAccessControlList.Path))
                .Returns(dataLakeDirectoryClientMock.Object);

            dataLakeServiceClientMock
                .Setup(x => x.GetFileSystemClient(createGroupsInAccessControlList.StorageContainer.ToLower()))
                .Returns(dataLakeFileSystemClientMock.Object);

            dataLakeDirectoryClientMock.Setup(x =>
                    x.SetAccessControlListAsync(It.IsAny<IList<PathAccessControlItem>>(), null, null, null, default))
                .Returns(Task.FromResult<Response<PathInfo>>(null));

            var sut = new AccessControlListService(loggerMock.Object, dataLakeServiceClientMock.Object);

            // Act
            await sut.SetGroupsInAccessControlListAsync(createGroupsInAccessControlList);

            // Assert
            dataLakeDirectoryClientMock.Verify(
                x => x.SetAccessControlListAsync(
                    It.Is<IList<PathAccessControlItem>>(a => a
                        .Where(entry => entry.AccessControlType != AccessControlType.Mask).All(entry =>
                            createGroupsInAccessControlList.GroupEntries
                                .Any(groupEntry => Equals(groupEntry.Id, entry.EntityId) &&
                                                   Equals(groupEntry.IsDefault, entry.DefaultScope) &&
                                                   Equals(groupEntry.Permissions,
                                                       entry.Permissions.ToSymbolicRolePermissions()) &&
                                                   Equals(AccessControlType.Group, entry.AccessControlType)))), null,
                    null, null, default),
                Times.Once);

            // Verify default group
            dataLakeDirectoryClientMock.Verify(
                x => x.SetAccessControlListAsync(
                    It.Is<IList<PathAccessControlItem>>(a => a.SingleOrDefault(entry =>
                        entry.AccessControlType == AccessControlType.Mask &&
                        entry.Permissions.ToSymbolicRolePermissions().Equals("rwx") &&
                        entry.DefaultScope) != null), null,
                    null, null, default),
                Times.Once);

            dataLakeDirectoryClientMock.Verify(
                x => x.SetAccessControlListAsync(
                    It.Is<IList<PathAccessControlItem>>(a => a.SingleOrDefault(entry =>
                        entry.AccessControlType == AccessControlType.Mask &&
                        entry.Permissions.ToSymbolicRolePermissions().Equals("rwx") &&
                        !entry.DefaultScope) != null), null,
                    null, null, default),
                Times.Once);
        }

        [Theory]
        [AutoMoqInfrastructure]
        public async Task Return_True_If_Group_Is_In_Acl(
            Mock<ILogger<AccessControlListService>> loggerMock,
            Mock<DataLakeDirectoryClient> dataLakeDirectoryClientMock,
            Mock<DataLakeFileSystemClient> dataLakeFileSystemClientMock,
            Mock<DataLakeServiceClient> dataLakeServiceClientMock,
            PathAccessControl pathAccessControl,
            string container,
            string path,
            string groupId)
        {
            // Arrange
            pathAccessControl.SetInternalProperty(x => x.AccessControlList,
                pathAccessControl.AccessControlList.Concat(new List<PathAccessControlItem>
                {
                    new PathAccessControlItem
                    {
                        AccessControlType = AccessControlType.Group, DefaultScope = false, EntityId = groupId,
                        Permissions = RolePermissions.Execute
                    }
                }));
            var responseMock = new Mock<Response<PathAccessControl>>();
            responseMock.SetupGet(x => x.Value).Returns(pathAccessControl);

            dataLakeFileSystemClientMock
                .Setup(x => x.GetDirectoryClient(path))
                .Returns(dataLakeDirectoryClientMock.Object);

            dataLakeServiceClientMock
                .Setup(x => x.GetFileSystemClient(container.ToLower()))
                .Returns(dataLakeFileSystemClientMock.Object);

            dataLakeDirectoryClientMock.Setup(x => x.GetAccessControlAsync(null, null, default))
                .ReturnsAsync(responseMock.Object);

            var sut = new AccessControlListService(loggerMock.Object, dataLakeServiceClientMock.Object);

            // Act
            var isGroupInAcl = await sut.IsGroupInAccessControlListAsync(groupId, container, path);

            // Assert
            isGroupInAcl.ShouldBe(true);
        }

        [Theory]
        [AutoMoqInfrastructure]
        public async Task Return_False_If_Group_Is_Not_In_Acl(
            Mock<ILogger<AccessControlListService>> loggerMock,
            Mock<DataLakeDirectoryClient> dataLakeDirectoryClientMock,
            Mock<DataLakeFileSystemClient> dataLakeFileSystemClientMock,
            Mock<DataLakeServiceClient> dataLakeServiceClientMock,
            PathAccessControl pathAccessControl,
            string container,
            string path,
            string groupId)
        {
            // Arrange
            var responseMock = new Mock<Response<PathAccessControl>>();
            responseMock.SetupGet(x => x.Value).Returns(pathAccessControl);
            
            dataLakeFileSystemClientMock
                .Setup(x => x.GetDirectoryClient(path))
                .Returns(dataLakeDirectoryClientMock.Object);

            dataLakeServiceClientMock
                .Setup(x => x.GetFileSystemClient(container.ToLower()))
                .Returns(dataLakeFileSystemClientMock.Object);

            dataLakeDirectoryClientMock.Setup(x => x.GetAccessControlAsync(null, null, default))
                .ReturnsAsync(responseMock.Object);

            var sut = new AccessControlListService(loggerMock.Object, dataLakeServiceClientMock.Object);

            // Act
            var isGroupInAcl = await sut.IsGroupInAccessControlListAsync(groupId, container, path);

            // Assert
            isGroupInAcl.ShouldBe(false);
        }
    }
}
