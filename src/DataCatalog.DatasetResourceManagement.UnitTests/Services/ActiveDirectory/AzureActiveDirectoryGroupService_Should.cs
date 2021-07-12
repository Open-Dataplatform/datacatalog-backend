using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using DataCatalog.DatasetResourceManagement.Commands.Group;
using DataCatalog.DatasetResourceManagement.Services.ActiveDirectory;
using DataCatalog.DatasetResourceManagement.Services.ActiveDirectory.DTO;
using DataCatalog.DatasetResourceManagement.UnitTests.AutoMoqData;
using DataCatalog.DatasetResourceManagement.UnitTests.Logger;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace DataCatalog.DatasetResourceManagement.UnitTests.Services.ActiveDirectory
{
    // ReSharper disable once InconsistentNaming
    public class AadGroupService_Should
    {
        [Theory]
        [AutoMoqInfrastructure]
        public async Task Correctly_Return_Group_Id(
            Mock<ILogger<AzureActiveDirectoryGroupService>> loggerMock,
            Mock<IGraphServiceClient> graphServiceClientMock,
            Mock<HttpMessageHandler> handlerMock,
            CreateGroupResponseDto createGroupResponseDto,
            CreateGroup createGroup)
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(createGroupResponseDto))
            };
            
            graphServiceClientMock.Setup(x => x.Groups[createGroupResponseDto.GroupId].Request().GetAsync())
                .ReturnsAsync(new Group());
            
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(httpResponse).Verifiable();

            var httpClient = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://baseadress.com/") };
            var sut = new AzureActiveDirectoryGroupService(
                httpClient, loggerMock.Object, graphServiceClientMock.Object);

            // Act
            var groupResponse = await sut.CreateGroupAsync(createGroup);

            // Assert
            groupResponse.Id.ShouldBe(createGroupResponseDto.GroupId);
        }

        [Theory]
        [AutoMoqInfrastructure]
        public async Task Correctly_Send_Create_Group_Request(
            Mock<ILogger<AzureActiveDirectoryGroupService>> loggerMock,
            Mock<IGraphServiceClient> graphServiceClientMock,
            Mock<HttpMessageHandler> handlerMock,
            CreateGroupResponseDto createGroupResponseDto,
            CreateGroup createGroup)
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(createGroupResponseDto))
            };
            var httpRequest = new CreateGroupRequestDto
            {
                DisplayName = createGroup.DisplayName,
                Description = createGroup.Description,
                MailNickname = createGroup.MailNickname,
                Owners = createGroup.Owners,
                Members = createGroup.Members
            };

            graphServiceClientMock.Setup(x => x.Groups[createGroupResponseDto.GroupId].Request().GetAsync())
                .ReturnsAsync(new Group());

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(httpResponse).Verifiable();
            
            var httpClient = new HttpClient(handlerMock.Object) {BaseAddress = new Uri("http://baseadress.com/")};
            var sut = new AzureActiveDirectoryGroupService(
                httpClient, loggerMock.Object, graphServiceClientMock.Object);
            
            // Act
            await sut.CreateGroupAsync(createGroup);
            
            // Assert
            handlerMock.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(a =>
                    Equals(JsonConvert.SerializeObject(httpRequest), a.Content.ReadAsStringAsync().Result) &&
                    Equals(a.RequestUri.ToString(), "http://baseadress.com/v1/Groups") &&
                    Equals(a.Method, HttpMethod.Post)),
                ItExpr.IsAny<CancellationToken>());
        }

        [Theory]
        [MoqAutoData]
        public async Task Throw_When_SendAsync_Throws(
            Mock<ILogger<AzureActiveDirectoryGroupService>> loggerMock,
            Mock<IGraphServiceClient> graphServiceClientMock,
            Mock<HttpMessageHandler> handlerMock,
            Exception e,
            CreateGroup createGroup)
        {
            // Arrange
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                ).ThrowsAsync(e);

            var httpClient = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://baseadress.com/") };
            var sut = new AzureActiveDirectoryGroupService(
                httpClient, loggerMock.Object, graphServiceClientMock.Object);
            
            // Act
            var ex = await Assert.ThrowsAsync<Exception>(() => sut.CreateGroupAsync(createGroup));

            // Assert
            loggerMock.VerifyLogWasCalled(LogLevel.Error, $"Error sending request for group {createGroup.DisplayName}", e);
            ex.ShouldBe(e);
        }

        [Theory]
        [MoqAutoData]
        public async Task Correctly_Log_When_Create_Group_Fails(
            Mock<ILogger<AzureActiveDirectoryGroupService>> loggerMock,
            Mock<IGraphServiceClient> graphServiceClientMock,
            string internalServerErrorContent,
            Mock<HttpMessageHandler> handlerMock,
            CreateGroup createGroup)
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError) {Content = new StringContent(internalServerErrorContent) };

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(httpResponse).Verifiable();

            var httpClient = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://baseadress.com/") };
            var sut = new AzureActiveDirectoryGroupService(httpClient, loggerMock.Object, graphServiceClientMock.Object);

            // Act
            await sut.CreateGroupAsync(createGroup);

            // Assert
            loggerMock.VerifyLogWasCalled(LogLevel.Error, $"Error creating group {createGroup.DisplayName} with error message {internalServerErrorContent}");
        }

        [Theory]
        [MoqAutoData]
        public async Task Add_Member_If_Member_Is_Not_In_Group_On_AddMember_Call(
            GroupMembersCollectionWithReferencesPage memberPage,
            string groupId,
            string memberId,
            [Frozen] Mock<IGraphServiceClient> graphServiceClientMock,
            AzureActiveDirectoryGroupService sut)
        {
            // Arrange
            graphServiceClientMock.Setup(x => x.Groups[groupId].Members.Request().GetAsync())
                .ReturnsAsync(memberPage);

            graphServiceClientMock.Setup(x => x.Groups[groupId].Members.References.Request().AddAsync(It.IsAny<DirectoryObject>()))
                .Returns(Task.CompletedTask);

            // Act
            await sut.AddGroupMember(groupId, memberId);

            // Assert
            graphServiceClientMock.Verify(x => x.Groups[groupId].Members.References.Request().AddAsync(It.Is<DirectoryObject>(a => Equals(a.Id, memberId))), Times.Once);
        }

        [Theory]
        [MoqAutoData]
        public async Task Does_Not_Add_Member_If_Member_Is_Already_In_Group_On_AddMember_Call(
            GroupMembersCollectionWithReferencesPage memberPage,
            string groupId,
            string memberId,
            [Frozen] Mock<IGraphServiceClient> graphServiceClientMock,
            AzureActiveDirectoryGroupService sut)
        {
            // Arrange
            memberPage.Add(new DirectoryObject { Id = memberId });
            graphServiceClientMock.Setup(x => x.Groups[groupId].Members.Request().GetAsync())
                .ReturnsAsync(memberPage);

            graphServiceClientMock.Setup(x => x.Groups[groupId].Members.References.Request().AddAsync(It.IsAny<DirectoryObject>()))
                .Returns(Task.CompletedTask);

            // Act
            await sut.AddGroupMember(groupId, memberId);

            // Assert
            graphServiceClientMock.Verify(x => x.Groups[groupId].Members.References.Request().AddAsync(It.Is<DirectoryObject>(a => Equals(a.Id, memberId))), Times.Never);
        }

        [Theory]
        [AutoMoqInfrastructure]
        public async Task Correctly_Get_Group_On_GetGroupAsync_Call(
            string displayName,
            string groupId,
            GraphServiceGroupsCollectionPage groupCollectionPage,
            [Frozen]Mock<IGraphServiceClient> graphServiceClientMock,
            AzureActiveDirectoryGroupService sut)
        {
            // Arrange
            var group = new Group { Id = groupId, DisplayName = displayName };
            groupCollectionPage.Add(group);
            graphServiceClientMock.Setup(x => x.Groups.Request().Filter($"displayName eq '{displayName}'").GetAsync())
                .ReturnsAsync(groupCollectionPage);

            // Act
            var groupResult = await sut.GetGroupAsync(displayName);

            // Assert
            groupResult.Id.ShouldBe(groupId);
            groupResult.DisplayName.ShouldBe(displayName);
            graphServiceClientMock.Verify(x => x.Groups.Request().Filter($"displayName eq '{displayName}'").GetAsync(), Times.Once);
        }
    }
}
