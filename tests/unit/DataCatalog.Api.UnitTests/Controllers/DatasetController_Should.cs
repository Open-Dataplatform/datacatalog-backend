using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using DataCatalog.Api.Controllers;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Common.Enums;
using DataCatalog.Common.Interfaces;
using DataCatalog.Api.Services;
using DataCatalog.Api.Services.AD;
using DataCatalog.Api.Services.Storage;
using DataCatalog.Api.UnitTests.AutoMoqAttribute;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using Xunit;
using DataCatalog.Api.Data.Domain;

namespace DataCatalog.Api.UnitTests.Controllers
{
    // ReSharper disable once InconsistentNaming
    public class DatasetController_Should
    {
        [Theory]
        [MapperAutoMoq]
        public async Task Correctly_Put_Dataset(
            DatasetUpdateRequest request,
            [Frozen] Mock<IStorageService> storageServiceMock,
            [Frozen] Mock<IDatasetService> datasetServiceMock,
            [Greedy] DatasetController sut)
        {
            // Arrange
            
            // Act
            var result = await sut.PutAsync(request);

            // Assert
            result.Result.ShouldBeOfType(typeof(OkObjectResult));
            datasetServiceMock.Verify(x => x.UpdateAsync(request), Times.Once);
            storageServiceMock.Verify(x => x.GetDirectoryMetadataAsync(request.Id.ToString()), Times.Once);
        }

        [Theory]
        [MapperInlineAutoMoq(Confidentiality.Internal)]
        [MapperInlineAutoMoq(Confidentiality.Confidential)]
        [MapperInlineAutoMoq(Confidentiality.StrictlyConfidential)]
        public async Task Correctly_Add_All_Users_Group_When_Confidentiality_Is_Updated_To_Public(
            Confidentiality confidentiality,
            DatasetUpdateRequest request,
            IDictionary<string, string> metadata,
            Dataset oldDataset,
            string readerGroupId,
            string allUsersGroupId,
            [Frozen] Mock<IGroupService> activeDirectoryGroupServiceMock,
            [Frozen] Mock<IAllUsersGroupProvider> allUsersProviderMock,
            [Frozen] Mock<IStorageService> storageServiceMock,
            [Frozen] Mock<IDatasetService> datasetServiceMock,
            [Greedy] DatasetController sut)
        {
            // Arrange
            request.Confidentiality = Confidentiality.Public;
            oldDataset.Confidentiality = confidentiality;
            allUsersProviderMock.Setup(x => x.GetAllUsersGroup()).Returns(allUsersGroupId);
            metadata.Add(GroupConstants.ReaderGroup, readerGroupId);
            storageServiceMock.Setup(x => x.GetDirectoryMetadataAsync(request.Id.ToString())).ReturnsAsync(metadata);
            datasetServiceMock.Setup(x => x.FindByIdAsync(request.Id)).ReturnsAsync(oldDataset);
            
            // Act
            var result = await sut.PutAsync(request);

            // Assert
            result.Result.ShouldBeOfType(typeof(OkObjectResult));

            activeDirectoryGroupServiceMock.Verify(x => x.AddGroupMemberAsync(readerGroupId, allUsersGroupId), Times.Once);
        }

        [Theory]
        [MapperInlineAutoMoq(Confidentiality.Internal)]
        [MapperInlineAutoMoq(Confidentiality.Confidential)]
        [MapperInlineAutoMoq(Confidentiality.StrictlyConfidential)]
        public async Task Correctly_Remove_All_Users_Group_When_Confidentiality_Is_Not_Public(
            Confidentiality confidentiality,
            DatasetUpdateRequest request,
            IDictionary<string, string> metadata,
            Dataset oldDataset,
            string readerGroupId,
            string allUsersGroupId,
            [Frozen] Mock<IGroupService> activeDirectoryGroupServiceMock,
            [Frozen] Mock<IAllUsersGroupProvider> allUsersProviderMock,
            [Frozen] Mock<IStorageService> storageServiceMock,
            [Frozen] Mock<IDatasetService> datasetServiceMock,
            [Greedy] DatasetController sut)
        {
            // Arrange
            request.Confidentiality = confidentiality;
            oldDataset.Confidentiality = Confidentiality.Public;
            allUsersProviderMock.Setup(x => x.GetAllUsersGroup()).Returns(allUsersGroupId);
            metadata.Add(GroupConstants.ReaderGroup, readerGroupId);
            storageServiceMock.Setup(x => x.GetDirectoryMetadataAsync(request.Id.ToString())).ReturnsAsync(metadata);
            datasetServiceMock.Setup(x => x.FindByIdAsync(request.Id)).ReturnsAsync(oldDataset);

            // Act
            var result = await sut.PutAsync(request);

            // Assert
            result.Result.ShouldBeOfType(typeof(OkObjectResult));

            activeDirectoryGroupServiceMock.Verify(x => x.RemoveGroupMemberAsync(readerGroupId, allUsersGroupId), Times.Once);
        }
    }
}
