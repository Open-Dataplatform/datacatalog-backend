using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using DataCatalog.DatasetResourceManagement.Commands.Group;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.ActiveDirectory;
using DataCatalog.DatasetResourceManagement.Responses.Group;
using DataCatalog.DatasetResourceManagement.Services.ActiveDirectory;
using DataCatalog.DatasetResourceManagement.UnitTests.AutoMoqData;
using Moq;
using Shouldly;
using Xunit;

namespace DataCatalog.DatasetResourceManagement.UnitTests.Services.ActiveDirectory
{
    // ReSharper disable once InconsistentNaming
    public class AzureActiveDirectoryGroupProvider_Should
    {
        [Theory]
        [MoqAutoData]
        public async Task Create_Group_When_Not_Exists(
            [Frozen] Mock<IActiveDirectoryGroupService> aadGroupServiceMock,
            CreateGroupResponse createGroupResponse,
            string displayName,
            string description,
            string [] members,
            AzureActiveDirectoryGroupProvider sut)
        {
            // Arrange
            aadGroupServiceMock.Setup(x => x.GetGroupAsync($"SEC-A-ENDK-{displayName}"))
                .Returns(Task.FromResult<GetGroupResponse>(null));

            aadGroupServiceMock.Setup(
                x => x.CreateGroupAsync(It.Is<CreateGroup>(a =>
                    Equals(a.DisplayName, displayName) &&
                    Equals(a.Description, description) &&
                    Equals(a.MailNickname, "dataplatform") &&
                    Equals(a.Members, members)))).ReturnsAsync(createGroupResponse);

            // Act
            var groupId = await sut.ProvideGroupAsync(displayName, description, members);

            // Assert
            aadGroupServiceMock.Verify(
                x => x.CreateGroupAsync(It.Is<CreateGroup>(a =>
                    Equals(a.DisplayName, displayName) && 
                    Equals(a.Description, description) &&
                    Equals(a.MailNickname, "dataplatform") &&
                    Equals(a.Members, members))), Times.Once);
            
            groupId.ShouldBe(createGroupResponse.Id);
        }

        [Theory]
        [MoqAutoData]
        public async Task Return_Existing_Group_If_Exists(
            [Frozen] Mock<IActiveDirectoryGroupService> aadGroupServiceMock,
            GetGroupResponse getGroupResponse,
            string displayName,
            string description,
            string[] members,
            AzureActiveDirectoryGroupProvider sut)
        {
            // Arrange
            aadGroupServiceMock.Setup(x => x.GetGroupAsync($"SEC-A-ENDK-{displayName}"))
                .Returns(Task.FromResult(getGroupResponse));
            
            // Act
            var groupId = await sut.ProvideGroupAsync(displayName, description, members);

            // Assert
            aadGroupServiceMock.Verify(x => x.GetGroupAsync($"SEC-A-ENDK-{displayName}"), Times.Once);
            aadGroupServiceMock.Verify(x => x.CreateGroupAsync(It.IsAny<CreateGroup>()), Times.Never);

            groupId.ShouldBe(getGroupResponse.Id);
        }

        [Theory]
        [MoqAutoData]
        public async Task Throw_Null_Reference_Exception_When_Create_Group_Fails(
            [Frozen] Mock<IActiveDirectoryGroupService> aadGroupServiceMock,
            string displayName,
            string description,
            string[] members,
            AzureActiveDirectoryGroupProvider sut)
        {
            // Arrange
            aadGroupServiceMock.Setup(x => x.GetGroupAsync($"SEC-A-ENDK-{displayName}"))
                .Returns(Task.FromResult<GetGroupResponse>(null));

            aadGroupServiceMock.Setup(
                x => x.CreateGroupAsync(It.Is<CreateGroup>(a =>
                    Equals(a.DisplayName, displayName) &&
                    Equals(a.Description, description) &&
                    Equals(a.MailNickname, "dataplatform") &&
                    Equals(a.Members, members)))).Returns(Task.FromResult<CreateGroupResponse>(null));

            // Act
            await Assert.ThrowsAsync<NullReferenceException>(() => sut.ProvideGroupAsync(displayName, description, members));

            // Assert
            aadGroupServiceMock.Verify(
                x => x.CreateGroupAsync(It.Is<CreateGroup>(a =>
                    Equals(a.DisplayName, displayName) &&
                    Equals(a.Description, description) &&
                    Equals(a.MailNickname, "dataplatform") &&
                    Equals(a.Members, members))), Times.Once);
        }
    }
}
