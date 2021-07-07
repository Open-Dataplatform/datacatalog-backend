using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Services.AD;
using DataCatalog.Api.UnitTests.AutoMoqAttribute;
using Microsoft.Graph;
using Moq;
using Shouldly;
using Xunit;
using User = Microsoft.Graph.User;

namespace DataCatalog.Api.UnitTests.Services.AD
{
    // ReSharper disable once InconsistentNaming
    public class AzureActiveDirectoryGroupService_Should
    {
        [Theory]
        [GraphAutoMoq]
        public async Task Successfully_Return_Group_Members_On_GetGroupMembersAsync(
            string groupId,
            Group groupMember,
            User userMember,
            ServicePrincipal spMember,
            GroupMembersCollectionWithReferencesPage memberPage,
            [Frozen] Mock<IGraphServiceClient> graphServiceClientMock,
            AzureGroupService sut)
        {
            // Arrange
            memberPage.Add(groupMember);
            memberPage.Add(userMember);
            memberPage.Add(spMember);
            graphServiceClientMock.Setup(x => x.Groups[groupId].Members.Request().GetAsync())
                .ReturnsAsync(memberPage);

            // Act
            var members = (await sut.GetGroupMembersAsync(groupId)).ToList();

            // Assert
            graphServiceClientMock.Verify(x => x.Groups[groupId].Members.Request().GetAsync(), Times.Once);
            
            members.Count.ShouldBe(3);
            members.Any(x => 
                Equals(x.Type, AccessMemberType.Group) && 
                Equals(x.Id, groupMember.Id) 
                && Equals(x.Name, groupMember.DisplayName)).ShouldBe(true);

            members.Any(x =>
                Equals(x.Type, AccessMemberType.User) &&
                Equals(x.Id, userMember.Id)
                && Equals(x.Name, userMember.DisplayName)).ShouldBe(true);

            members.Any(x =>
                Equals(x.Type, AccessMemberType.ServicePrincipal) &&
                Equals(x.Id, spMember.Id)
                && Equals(x.Name, spMember.DisplayName)).ShouldBe(true);
        }

        [Theory]
        [GraphAutoMoq]
        public async Task Return_Null_When_ServiceException_On_GetGroupMembersAsync(
            string groupId,
            ServiceException se,
            [Frozen] Mock<IGraphServiceClient> graphServiceClientMock,
            AzureGroupService sut)
        {
            // Arrange
            graphServiceClientMock.Setup(x => x.Groups[groupId].Members.Request().GetAsync())
                .ThrowsAsync(se);

            // Act
            var members = await sut.GetGroupMembersAsync(groupId);

            // Assert
            graphServiceClientMock.Verify(x => x.Groups[groupId].Members.Request().GetAsync(), Times.Once);
            members.ShouldBeNull();
        }

        [Theory]
        [GraphAutoMoq]
        public async Task Throw_When_Oncaught_Exception_On_GetGroupMembersAsync(
            string groupId,
            Exception e,
            [Frozen] Mock<IGraphServiceClient> graphServiceClientMock,
            AzureGroupService sut)
        {
            // Arrange
            graphServiceClientMock.Setup(x => x.Groups[groupId].Members.Request().GetAsync())
                .ThrowsAsync(e);

            // Act
            var ex = await Assert.ThrowsAsync<Exception>(() => sut.GetGroupMembersAsync(groupId));

            // Assert
            ex.ShouldBe(e);
        }
        
        [Theory]
        [GraphAutoMoq]
        public async Task Correctly_Return_Member_On_GetAccessMemberAsync(
            string memberId,
            User user,
            [Frozen] Mock<IGraphServiceClient> graphServiceClientMock,
            AzureGroupService sut)
        {
            // Arrange
            graphServiceClientMock.Setup(x => x.DirectoryObjects[memberId].Request().GetAsync())
                .ReturnsAsync(user);

            // Act
            var member = await sut.GetAccessMemberAsync(memberId);

            // Assert
            member.Id.ShouldBe(user.Id);
            member.Name.ShouldBe(user.DisplayName);
            member.Type.ShouldBe(AccessMemberType.User);
        }

        [Theory]
        [GraphAutoMoq]
        public async Task Return_Null_On_ServiceException_On_GetAccessMemberAsync(
            string memberId,
            ServiceException se,
            [Frozen] Mock<IGraphServiceClient> graphServiceClientMock,
            AzureGroupService sut)
        {
            // Arrange
            graphServiceClientMock.Setup(x => x.DirectoryObjects[memberId].Request().GetAsync())
                .ThrowsAsync(se);

            // Act
            var member = await sut.GetAccessMemberAsync(memberId);

            // Assert
            member.ShouldBeNull();
        }

        [Theory]
        [GraphAutoMoq]
        public async Task Throw_On_Exception_On_GetAccessMemberAsync(
            string memberId,
            Exception e,
            [Frozen] Mock<IGraphServiceClient> graphServiceClientMock,
            AzureGroupService sut)
        {
            // Arrange
            graphServiceClientMock.Setup(x => x.DirectoryObjects[memberId].Request().GetAsync())
                .ThrowsAsync(e);

            // Act
            var ex = await Assert.ThrowsAsync<Exception>(() => sut.GetAccessMemberAsync(memberId));

            // Assert
            ex.ShouldBe(e);
        }

        [Theory]
        [GraphAutoMoq]
        public async Task Add_Member_If_Member_Is_Not_In_Group_On_AddGroupMemberAsync(
            GroupMembersCollectionWithReferencesPage memberPage,
            string groupId,
            string memberId,
            [Frozen] Mock<IGraphServiceClient> graphServiceClientMock,
            AzureGroupService sut)
        {
            // Arrange
            graphServiceClientMock.Setup(x => x.Groups[groupId].Members.Request().GetAsync())
                .ReturnsAsync(memberPage);

            graphServiceClientMock.Setup(x => x.Groups[groupId].Members.References.Request().AddAsync(It.IsAny<DirectoryObject>()))
                .Returns(Task.CompletedTask);

            // Act
            await sut.AddGroupMemberAsync(groupId, memberId);

            // Assert
            graphServiceClientMock.Verify(x => x.Groups[groupId].Members.References.Request().AddAsync(It.Is<DirectoryObject>(a => Equals(a.Id, memberId))), Times.Once);
        }

        [Theory]
        [GraphAutoMoq]
        public async Task Does_Not_Add_Member_If_Member_Is_Already_In_Group_On_AddGroupMemberAsync(
                GroupMembersCollectionWithReferencesPage memberPage,
                string groupId,
                string memberId,
                [Frozen] Mock<IGraphServiceClient> graphServiceClientMock,
                AzureGroupService sut)
        {
            // Arrange
            memberPage.Add(new DirectoryObject { Id = memberId });
            graphServiceClientMock.Setup(x => x.Groups[groupId].Members.Request().GetAsync())
                .ReturnsAsync(memberPage);

            graphServiceClientMock.Setup(x => x.Groups[groupId].Members.References.Request().AddAsync(It.IsAny<DirectoryObject>()))
                .Returns(Task.CompletedTask);

            // Act
            await sut.AddGroupMemberAsync(groupId, memberId);

            // Assert
            graphServiceClientMock.Verify(x => x.Groups[groupId].Members.References.Request().AddAsync(It.Is<DirectoryObject>(a => Equals(a.Id, memberId))), Times.Never);
        }

        [Theory]
        [GraphAutoMoq]
        public async Task Throw_On_Exception_On_AddGroupMemberAsync(
            string groupId,
            string memberId,
            Exception e,
            [Frozen] Mock<IGraphServiceClient> graphServiceClientMock,
            AzureGroupService sut)
        {
            // Arrange
            graphServiceClientMock.Setup(x => x.Groups[groupId].Members.Request().GetAsync())
                .ThrowsAsync(e);

            graphServiceClientMock.Setup(x => x.Groups[groupId].Members.References.Request().AddAsync(It.IsAny<DirectoryObject>()))
                .Returns(Task.CompletedTask);

            // Act
            var ex = await Assert.ThrowsAsync<Exception>(() => sut.AddGroupMemberAsync(groupId, memberId));

            // Assert
            ex.ShouldBe(e);
        }

        [Theory]
        [GraphAutoMoq]
        public async Task Not_Throw_On_ServiceException_On_AddGroupMemberAsync(
            string groupId,
            string memberId,
            ServiceException se,
            [Frozen] Mock<IGraphServiceClient> graphServiceClientMock,
            AzureGroupService sut)
        {
            // Arrange
            graphServiceClientMock.Setup(x => x.Groups[groupId].Members.Request().GetAsync())
                .ThrowsAsync(se);

            graphServiceClientMock.Setup(x => x.Groups[groupId].Members.References.Request().AddAsync(It.IsAny<DirectoryObject>()))
                .Returns(Task.CompletedTask);

            // Act / Assert
            await sut.AddGroupMemberAsync(groupId, memberId).ShouldNotThrowAsync();
        }

        [Theory]
        [GraphAutoMoq]
        public async Task Correctly_Remove_Member_From_Group_On_RemoveGroupMemberAsync(
            GroupMembersCollectionWithReferencesPage memberPage,
            string groupId,
            string memberId,
            [Frozen] Mock<IGraphServiceClient> graphServiceClientMock,
            AzureGroupService sut)
        {
            // Arrange
            memberPage.Add(new DirectoryObject { Id = memberId });
            graphServiceClientMock.Setup(x => x.Groups[groupId].Members.Request().GetAsync())
                .ReturnsAsync(memberPage);

            graphServiceClientMock.Setup(x => x.Groups[groupId].Members[memberId].Reference.Request().DeleteAsync())
                .Returns(Task.CompletedTask);

            // Act
            await sut.RemoveGroupMemberAsync(groupId, memberId);

            // Assert
            graphServiceClientMock.Verify(x => x.Groups[groupId].Members[memberId].Reference.Request().DeleteAsync(),
                Times.Once);
        }

        [Theory]
        [GraphAutoMoq]
        public async Task Not_Throw_On_ServiceException_On_RemoveGroupMemberAsync(
            string groupId,
            string memberId,
            ServiceException se,
            [Frozen] Mock<IGraphServiceClient> graphServiceClientMock,
            AzureGroupService sut)
        {
            // Arrange
            graphServiceClientMock.Setup(x => x.Groups[groupId].Members[memberId].Request().GetAsync())
                .ThrowsAsync(se);

            // Act / Assert
            await sut.RemoveGroupMemberAsync(groupId, memberId).ShouldNotThrowAsync();
        }

        [Theory]
        [GraphAutoMoq]
        public async Task Throw_On_Exception_On_RemoveGroupMemberAsync(
            GroupMembersCollectionWithReferencesPage memberPage,
            string groupId,
            string memberId,
            Exception e,
            [Frozen] Mock<IGraphServiceClient> graphServiceClientMock,
            AzureGroupService sut)
        {
            // Arrange
            memberPage.Add(new DirectoryObject { Id = memberId });
            graphServiceClientMock.Setup(x => x.Groups[groupId].Members.Request().GetAsync())
                .ReturnsAsync(memberPage);

            graphServiceClientMock.Setup(x => x.Groups[groupId].Members[memberId].Reference.Request().DeleteAsync())
                .ThrowsAsync(e);

            // Act 
            var ex = await Assert.ThrowsAsync<Exception>(() => sut.RemoveGroupMemberAsync(groupId, memberId));
            
            // Assert
            ex.ShouldBe(e);
        }

        [Theory]
        [GraphAutoMoq]
        public async Task Correctly_Return_Groups_On_SearchAsync(
            string searchString,
            Group groupResult1,
            Group groupResult2,
            Group groupResult3,
            GraphServiceGroupsCollectionPage groupsCollectionPage,
            [Frozen] Mock<IGraphServiceClient> graphServiceClientMock,
            AzureGroupService sut)
        {
            // Arrange
            groupsCollectionPage.Add(groupResult1);
            groupsCollectionPage.Add(groupResult2);
            groupsCollectionPage.Add(groupResult3);
            graphServiceClientMock.Setup(x => x.Groups.Request(It.IsAny<IEnumerable<Option>>()).GetAsync())
                .ReturnsAsync(groupsCollectionPage);

            // Act 
            var results = (await sut.SearchAsync(searchString)).ToList();

            // Assert
            results.Any(x =>
                Equals(x.DisplayName, groupResult1.DisplayName) && Equals(x.Id, groupResult1.Id) &&
                Equals(x.Type, AdSearchResultType.Group)).ShouldBeTrue();
            results.Any(x =>
                Equals(x.DisplayName, groupResult2.DisplayName) && Equals(x.Id, groupResult2.Id) &&
                Equals(x.Type, AdSearchResultType.Group)).ShouldBeTrue();
            results.Any(x =>
                Equals(x.DisplayName, groupResult3.DisplayName) && Equals(x.Id, groupResult3.Id) &&
                Equals(x.Type, AdSearchResultType.Group)).ShouldBeTrue();
        }

        [Theory]
        [GraphAutoMoq]
        public async Task Not_Return_Unified_Groups_On_SearchAsync(
            string searchString,
            Group groupResult1,
            Group groupResult2,
            Group groupResult3,
            GraphServiceGroupsCollectionPage groupsCollectionPage,
            [Frozen] Mock<IGraphServiceClient> graphServiceClientMock,
            AzureGroupService sut)
        {
            // Arrange
            groupResult1.GroupTypes = new List<string> { "Unified" };
            groupResult2.GroupTypes = new List<string> { "Unified" };
            groupResult3.GroupTypes = new List<string> { "Unified" };
            groupsCollectionPage.Add(groupResult1);
            groupsCollectionPage.Add(groupResult2);
            groupsCollectionPage.Add(groupResult3);
            graphServiceClientMock.Setup(x => x.Groups.Request(It.IsAny<IEnumerable<Option>>()).GetAsync())
                .ReturnsAsync(groupsCollectionPage);

            // Act 
            var results = await sut.SearchAsync(searchString);

            // Assert
            results.Where(x => Equals(x.Type, AdSearchResultType.Group)).ShouldBeEmpty();
        }

        [Theory]
        [GraphAutoMoq]
        public async Task Correctly_Return_ServicePrincipals_On_SearchAsync(
            string searchString,
            ServicePrincipal sp1,
            ServicePrincipal sp2,
            ServicePrincipal sp3,
            GraphServiceServicePrincipalsCollectionPage servicePrincipalsCollectionPage,
            [Frozen] Mock<IGraphServiceClient> graphServiceClientMock,
            AzureGroupService sut)
        {
            // Arrange
            servicePrincipalsCollectionPage.Add(sp1);
            servicePrincipalsCollectionPage.Add(sp2);
            servicePrincipalsCollectionPage.Add(sp3);
            graphServiceClientMock.Setup(x => x.ServicePrincipals.Request(It.IsAny<IEnumerable<Option>>()).GetAsync())
                .ReturnsAsync(servicePrincipalsCollectionPage);

            // Act 
            var results = (await sut.SearchAsync(searchString)).ToList();

            // Assert
            results.Any(x =>
                Equals(x.DisplayName, sp1.DisplayName) && Equals(x.Id, sp1.Id) &&
                Equals(x.Type, AdSearchResultType.ServicePrincipal)).ShouldBeTrue();
            results.Any(x =>
                Equals(x.DisplayName, sp2.DisplayName) && Equals(x.Id, sp2.Id) &&
                Equals(x.Type, AdSearchResultType.ServicePrincipal)).ShouldBeTrue();
            results.Any(x =>
                Equals(x.DisplayName, sp3.DisplayName) && Equals(x.Id, sp3.Id) &&
                Equals(x.Type, AdSearchResultType.ServicePrincipal)).ShouldBeTrue();
        }

        [Theory]
        [GraphAutoMoq]
        public async Task Correctly_Return_Users_On_SearchAsync(
            string searchString,
            User user1,
            User user2,
            User user3,
            GraphServiceUsersCollectionPage usersCollectionPage,
            [Frozen] Mock<IGraphServiceClient> graphServiceClientMock,
            AzureGroupService sut)
        {
            // Arrange
            usersCollectionPage.Add(user1);
            usersCollectionPage.Add(user2);
            usersCollectionPage.Add(user3);
            graphServiceClientMock.Setup(x => x.Users.Request(It.IsAny<IEnumerable<Option>>()).GetAsync())
                .ReturnsAsync(usersCollectionPage);

            // Act 
            var results = (await sut.SearchAsync(searchString)).ToList();

            // Assert
            results.Any(x =>
                Equals(x.DisplayName, user1.DisplayName) && Equals(x.Id, user1.Id) &&
                Equals(x.Type, AdSearchResultType.User)).ShouldBeTrue();
            results.Any(x =>
                Equals(x.DisplayName, user2.DisplayName) && Equals(x.Id, user2.Id) &&
                Equals(x.Type, AdSearchResultType.User)).ShouldBeTrue();
            results.Any(x =>
                Equals(x.DisplayName, user3.DisplayName) && Equals(x.Id, user3.Id) &&
                Equals(x.Type, AdSearchResultType.User)).ShouldBeTrue();
        }
    }
}
