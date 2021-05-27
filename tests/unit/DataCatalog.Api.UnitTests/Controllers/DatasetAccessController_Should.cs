using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using DataCatalog.Api.Controllers;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Api.Extensions;
using DataCatalog.Api.Services.AD;
using DataCatalog.Api.Services.Storage;
using DataCatalog.Api.UnitTests.AutoMoqAttribute;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using Xunit;

namespace DataCatalog.Api.UnitTests.Controllers
{

    // ReSharper disable once InconsistentNaming
    public class DatasetAccessController_Should
    {
        [Theory]
        [MapperAutoMoq]
        public async Task Return_Access_List_On_GetAccessList(
            Guid datasetId,
            string readerGroupId,
            string writerGroupId,
            IList<AccessMember> readers,
            IList<AccessMember> writers,
            IDictionary<string, string> metadata,
            [Frozen] Mock<IGroupService> activeDirectoryServiceMock,
            [Frozen] Mock<IStorageService> storageServiceMock,
            [Greedy] DatasetAccessController sut)
        {
            // Arrange
            metadata.Add(GroupConstants.ReaderGroup, readerGroupId);
            metadata.Add(GroupConstants.WriterGroup, writerGroupId);
            storageServiceMock.Setup(x => x.GetDirectoryMetadataAsync(datasetId.ToString())).ReturnsAsync(metadata);
            activeDirectoryServiceMock.Setup(x => x.GetGroupMembersAsync(readerGroupId)).ReturnsAsync(readers);
            activeDirectoryServiceMock.Setup(x => x.GetGroupMembersAsync(writerGroupId)).ReturnsAsync(writers);

            // Act
            var result = await sut.GetAccessList(datasetId);

            // Assert
            storageServiceMock.Verify(x => x.GetDirectoryMetadataAsync(datasetId.ToString()), Times.Once());
            result.Result.ShouldBeOfType(typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            var value = (DatasetAccessListResponse)okResult.Value;

            foreach (var reader in readers)
                value.ReadAccessList.Any(x =>
                    Equals(x.Id, reader.Id) && 
                    Equals(x.Name, reader.Name) &&
                    Equals(x.MemberType, reader.Type.EnumNameToDescription())).ShouldBeTrue();

            foreach (var writer in writers)
                value.WriteAccessList.Any(x =>
                    Equals(x.Id, writer.Id) &&
                    Equals(x.Name, writer.Name) &&
                    Equals(x.MemberType, writer.Type.EnumNameToDescription())).ShouldBeTrue();
        }

        [Theory]
        [MapperAutoMoq]
        public async Task Return_NotFound_When_Directory_Does_Not_Exist_On_GetAccessList(
            Guid datasetId,
            [Frozen] Mock<IStorageService> storageServiceMock,
            [Greedy] DatasetAccessController sut)
        {
            // Arrange
            IDictionary<string, string> metadata = null;
            storageServiceMock.Setup(x => x.GetDirectoryMetadataAsync(datasetId.ToString())).ReturnsAsync(metadata);

            // Act
            var result = await sut.GetAccessList(datasetId);

            // Assert
            storageServiceMock.Verify(x => x.GetDirectoryMetadataAsync(datasetId.ToString()), Times.Once());
            result.Result.ShouldBeOfType(typeof(NotFoundResult));
        }

        [Theory]
        [MapperAutoMoq]
        public async Task Return_Ok_When_RemoveReadDataAccessMember_Succeeds(
            Guid datasetId,
            Guid memberId,
            string readerGroupId,
            IDictionary<string, string> metadata,
            [Frozen] Mock<IGroupService> activeDirectoryServiceMock,
            [Frozen] Mock<IStorageService> storageServiceMock,
            [Greedy] DatasetAccessController sut)
        {
            // Arrange
            metadata.Add(GroupConstants.ReaderGroup, readerGroupId);
            storageServiceMock.Setup(x => x.GetDirectoryMetadataAsync(datasetId.ToString())).ReturnsAsync(metadata);
            
            // Act
            var result = await sut.RemoveReadDataAccessMember(datasetId, memberId);

            // Assert
            result.ShouldBeOfType(typeof(OkResult));
            storageServiceMock.Verify(x => x.GetDirectoryMetadataAsync(datasetId.ToString()), Times.Once);
            activeDirectoryServiceMock.Verify(x => x.RemoveGroupMemberAsync(readerGroupId, memberId.ToString()), Times.Once);
        }

        [Theory]
        [MapperAutoMoq]
        public async Task Return_NotFound_When_Dataset_Directory_Does_Not_Exist_On_RemoveReadDataAccessMember(
            Guid datasetId,
            Guid memberId,
            [Greedy] DatasetAccessController sut)
        {
            // Arrange

            // Act
            var result = await sut.RemoveReadDataAccessMember(datasetId, memberId);

            // Assert
            result.ShouldBeOfType(typeof(NotFoundResult));
        }

        [Theory]
        [MapperAutoMoq]
        public async Task Return_NotFound_When_Reader_Group_Is_Not_In_Directory_Metadata_On_RemoveReadDataAccessMember(
            Guid datasetId,
            Guid memberId,
            IDictionary<string, string> metadata,
            [Frozen] Mock<IStorageService> storageServiceMock,
            [Greedy] DatasetAccessController sut)
        {
            // Arrange
            storageServiceMock.Setup(x => x.GetDirectoryMetadataAsync(datasetId.ToString())).ReturnsAsync(metadata);

            // Act
            var result = await sut.RemoveReadDataAccessMember(datasetId, memberId);

            // Assert
            result.ShouldBeOfType(typeof(NotFoundResult));
            storageServiceMock.Verify(x => x.GetDirectoryMetadataAsync(datasetId.ToString()), Times.Once);
        }

        [Theory]
        [MapperAutoMoq]
        public async Task Return_Ok_When_RemoveWriteDataAccessMember_Succeeds(
            Guid datasetId,
            Guid memberId,
            string writerGroupId,
            IDictionary<string, string> metadata,
            [Frozen] Mock<IGroupService> activeDirectoryServiceMock,
            [Frozen] Mock<IStorageService> storageServiceMock,
            [Greedy] DatasetAccessController sut)
        {
            // Arrange
            metadata.Add(GroupConstants.WriterGroup, writerGroupId);
            storageServiceMock.Setup(x => x.GetDirectoryMetadataAsync(datasetId.ToString())).ReturnsAsync(metadata);

            // Act
            var result = await sut.RemoveWriteDataAccessMember(datasetId, memberId);

            // Assert
            result.ShouldBeOfType(typeof(OkResult));
            storageServiceMock.Verify(x => x.GetDirectoryMetadataAsync(datasetId.ToString()), Times.Once);
            activeDirectoryServiceMock.Verify(x => x.RemoveGroupMemberAsync(writerGroupId, memberId.ToString()), Times.Once);
        }

        [Theory]
        [MapperAutoMoq]
        public async Task Return_NotFound_When_Dataset_Directory_Does_Not_Exist_On_RemoveWriteDataAccessMember(
            Guid datasetId,
            Guid memberId,
            [Greedy] DatasetAccessController sut)
        {
            // Arrange

            // Act
            var result = await sut.RemoveWriteDataAccessMember(datasetId, memberId);

            // Assert
            result.ShouldBeOfType(typeof(NotFoundResult));
        }

        [Theory]
        [MapperAutoMoq]
        public async Task Return_NotFound_When_Writer_Group_Is_Not_In_Directory_Metadata_On_RemoveWriteDataAccessMember(
            Guid datasetId,
            Guid memberId,
            IDictionary<string, string> metadata,
            [Frozen] Mock<IStorageService> storageServiceMock,
            [Greedy] DatasetAccessController sut)
        {
            // Arrange
            storageServiceMock.Setup(x => x.GetDirectoryMetadataAsync(datasetId.ToString())).ReturnsAsync(metadata);

            // Act
            var result = await sut.RemoveWriteDataAccessMember(datasetId, memberId);

            // Assert
            result.ShouldBeOfType(typeof(NotFoundResult));
            storageServiceMock.Verify(x => x.GetDirectoryMetadataAsync(datasetId.ToString()), Times.Once);
        }

        [Theory]
        [MapperAutoMoq]
        public async Task Successfully_Return_Access_Reader_On_AddReadAccessMember(
            Guid datasetId,
            string readerGroupId,
            IDictionary<string, string> metadata,
            AccessMember accessMember,
            AddDatasetAccessMemberRequestDto addDatasetAccessMemberRequest,
            [Frozen] Mock<IGroupService> activeDirectoryServiceMock,
            [Frozen] Mock<IStorageService> storageServiceMock,
            [Greedy] DatasetAccessController sut)
        {
            // Arrange
            metadata.Add(GroupConstants.ReaderGroup, readerGroupId);
            storageServiceMock.Setup(x => x.GetDirectoryMetadataAsync(datasetId.ToString())).ReturnsAsync(metadata);
            activeDirectoryServiceMock.Setup(x => x.GetAccessMemberAsync(addDatasetAccessMemberRequest.MemberId.ToString()))
                .ReturnsAsync(accessMember);

            // Act
            var result = await sut.AddReadAccessMember(datasetId, addDatasetAccessMemberRequest);

            // Assert
            result.Result.ShouldBeOfType(typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            var dataAccessEntryResult = (DataAccessEntry)okResult.Value;
            dataAccessEntryResult.Id.ShouldBe(accessMember.Id);
            dataAccessEntryResult.Name.ShouldBe(accessMember.Name);
            dataAccessEntryResult.MemberType.ShouldBe(accessMember.Type.EnumNameToDescription());
            storageServiceMock.Verify(x => x.GetDirectoryMetadataAsync(datasetId.ToString()), Times.Once);
            activeDirectoryServiceMock.Verify(
                x => x.AddGroupMemberAsync(readerGroupId, addDatasetAccessMemberRequest.MemberId.ToString()),
                Times.Once);
        }

        [Theory]
        [MapperAutoMoq]
        public async Task Successfully_Return_Access_Writer_On_AddWriteAccessMember(
            Guid datasetId,
            string writerGroupId,
            IDictionary<string, string> metadata,
            AccessMember accessMember,
            AddDatasetAccessMemberRequestDto addDatasetAccessMemberRequest,
            [Frozen] Mock<IGroupService> activeDirectoryServiceMock,
            [Frozen] Mock<IStorageService> storageServiceMock,
            [Greedy] DatasetAccessController sut)
        {
            // Arrange
            metadata.Add(GroupConstants.WriterGroup, writerGroupId);
            storageServiceMock.Setup(x => x.GetDirectoryMetadataAsync(datasetId.ToString())).ReturnsAsync(metadata);
            activeDirectoryServiceMock.Setup(x => x.GetAccessMemberAsync(addDatasetAccessMemberRequest.MemberId.ToString()))
                .ReturnsAsync(accessMember);

            // Act
            var result = await sut.AddWriteAccessMember(datasetId, addDatasetAccessMemberRequest);

            // Assert
            result.Result.ShouldBeOfType(typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            var dataAccessEntryResult = (DataAccessEntry)okResult.Value;
            dataAccessEntryResult.Id.ShouldBe(accessMember.Id);
            dataAccessEntryResult.Name.ShouldBe(accessMember.Name);
            dataAccessEntryResult.MemberType.ShouldBe(accessMember.Type.EnumNameToDescription());
            storageServiceMock.Verify(x => x.GetDirectoryMetadataAsync(datasetId.ToString()), Times.Once);
            activeDirectoryServiceMock.Verify(
                x => x.AddGroupMemberAsync(writerGroupId, addDatasetAccessMemberRequest.MemberId.ToString()),
                Times.Once);
        }

        [Theory]
        [MapperAutoMoq]
        public async Task Return_NotFound_When_Metadata_Does_Not_Exist_On_AddReadAccessMember(
            Guid datasetId,
            AddDatasetAccessMemberRequestDto addDatasetAccessMemberRequest,
            [Frozen] Mock<IStorageService> storageServiceMock,
            [Greedy] DatasetAccessController sut)
        {
            // Arrange

            // Act
            var result = await sut.AddReadAccessMember(datasetId, addDatasetAccessMemberRequest);

            // Assert
            result.Result.ShouldBeOfType(typeof(NotFoundResult));
            storageServiceMock.Verify(x => x.GetDirectoryMetadataAsync(datasetId.ToString()), Times.Once);
        }

        [Theory]
        [MapperAutoMoq]
        public async Task Return_NotFound_When_Metadata_Does_Not_Exist_On_AddWriteAccessMember(
            Guid datasetId,
            AddDatasetAccessMemberRequestDto addDatasetAccessMemberRequest,
            [Frozen] Mock<IStorageService> storageServiceMock,
            [Greedy] DatasetAccessController sut)
        {
            // Arrange

            // Act
            var result = await sut.AddWriteAccessMember(datasetId, addDatasetAccessMemberRequest);

            // Assert
            result.Result.ShouldBeOfType(typeof(NotFoundResult));
            storageServiceMock.Verify(x => x.GetDirectoryMetadataAsync(datasetId.ToString()), Times.Once);
        }

        [Theory]
        [MapperAutoMoq]
        public async Task NotFound_When_ReaderGroup_Is_Not_In_Metadata_On_AddReadAccessMember(
            Guid datasetId,
            IDictionary<string, string> metadata,
            AddDatasetAccessMemberRequestDto addDatasetAccessMemberRequest,
            [Frozen] Mock<IStorageService> storageServiceMock,
            [Greedy] DatasetAccessController sut)
        {
            // Arrange
            storageServiceMock.Setup(x => x.GetDirectoryMetadataAsync(datasetId.ToString())).ReturnsAsync(metadata);

            // Act
            var result = await sut.AddReadAccessMember(datasetId, addDatasetAccessMemberRequest);

            // Assert
            result.Result.ShouldBeOfType(typeof(NotFoundResult));
            storageServiceMock.Verify(x => x.GetDirectoryMetadataAsync(datasetId.ToString()), Times.Once);
        }

        [Theory]
        [MapperAutoMoq]
        public async Task NotFound_When_WriterGroup_Is_Not_In_Metadata_On_AddWriteAccessMember(
            Guid datasetId,
            IDictionary<string, string> metadata,
            AddDatasetAccessMemberRequestDto addDatasetAccessMemberRequest,
            [Frozen] Mock<IStorageService> storageServiceMock,
            [Greedy] DatasetAccessController sut)
        {
            // Arrange
            storageServiceMock.Setup(x => x.GetDirectoryMetadataAsync(datasetId.ToString())).ReturnsAsync(metadata);

            // Act
            var result = await sut.AddWriteAccessMember(datasetId, addDatasetAccessMemberRequest);

            // Assert
            result.Result.ShouldBeOfType(typeof(NotFoundResult));
            storageServiceMock.Verify(x => x.GetDirectoryMetadataAsync(datasetId.ToString()), Times.Once);
        }

        [Theory]
        [MapperAutoMoq]
        public async Task Correctly_Return_Results_On_Search(
            string searchString,
            IList<AdSearchResult> adSearchResults,
            [Frozen] Mock<IGroupService> activeDirectoryServiceMock,
            [Greedy] DatasetAccessController sut)
        {
            // Arrange
            activeDirectoryServiceMock.Setup(x => x.SearchAsync(searchString)).ReturnsAsync(adSearchResults);

            // Act
            var result = await sut.Search(searchString);

            // Assert
            result.Result.ShouldBeOfType(typeof(OkObjectResult));
            var objectResult = (OkObjectResult) result.Result;
            var valueResult = (IEnumerable<AdSearchResultResponse>)objectResult.Value;
            adSearchResults.All(x => valueResult.Any(y =>
                Equals(x.DisplayName, y.DisplayName) && 
                Equals(x.Id, y.Id) && 
                Equals(x.Type, y.Type))).ShouldBeTrue();
            activeDirectoryServiceMock.Verify(x => x.SearchAsync(searchString), Times.Once);
        }
    }
}
