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
            storageServiceMock.Verify(x => x.GetDirectoryMetadataWithRetry(request.Id), Times.Once);
        }

        [Theory]
        [MapperInlineAutoMoq(Confidentiality.Internal)]
        [MapperInlineAutoMoq(Confidentiality.Confidential)]
        [MapperInlineAutoMoq(Confidentiality.StrictlyConfidential)]
        [MapperInlineAutoMoq(DatasetStatus.Archived)]
        [MapperInlineAutoMoq(DatasetStatus.Draft)]
        [MapperInlineAutoMoq(DatasetStatus.Published)]
        public async Task Correctly_Add_All_Users_Group_When_Confidentiality_Is_Updated_To_Public_And_Published(
            Confidentiality confidentiality,
            DatasetStatus status,
            DatasetUpdateRequest request,
            IDictionary<string, string> metadata,
            Dataset oldDataset,
            Dataset newDataset,
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
            request.Status = DatasetStatus.Published;
            newDataset.Confidentiality = request.Confidentiality;
            newDataset.Status = request.Status;
            oldDataset.Confidentiality = confidentiality;
            oldDataset.Status = status;
            allUsersProviderMock.Setup(x => x.GetAllUsersGroup()).Returns(allUsersGroupId);
            metadata.Add(GroupConstants.ReaderGroup, readerGroupId);
            storageServiceMock.Setup(x => x.GetDirectoryMetadataWithRetry(request.Id)).ReturnsAsync(metadata);
            datasetServiceMock.Setup(x => x.FindByIdAsync(request.Id)).ReturnsAsync(oldDataset);
            datasetServiceMock.Setup(x => x.UpdateAsync(request)).ReturnsAsync(newDataset);
            
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
        [MapperInlineAutoMoq(DatasetStatus.Archived)]
        [MapperInlineAutoMoq(DatasetStatus.Draft)]
        [MapperInlineAutoMoq(DatasetStatus.Published)]
        public async Task Correctly_Remove_All_Users_Group_When_Confidentiality_Is_Not_Public(
            Confidentiality confidentiality,
            DatasetStatus status,
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
            request.Status = status;
            oldDataset.Confidentiality = Confidentiality.Public;
            oldDataset.Status = DatasetStatus.Published;
            allUsersProviderMock.Setup(x => x.GetAllUsersGroup()).Returns(allUsersGroupId);
            metadata.Add(GroupConstants.ReaderGroup, readerGroupId);
            storageServiceMock.Setup(x => x.GetDirectoryMetadataWithRetry(request.Id)).ReturnsAsync(metadata);
            datasetServiceMock.Setup(x => x.FindByIdAsync(request.Id)).ReturnsAsync(oldDataset);

            // Act
            var result = await sut.PutAsync(request);

            // Assert
            result.Result.ShouldBeOfType(typeof(OkObjectResult));

            activeDirectoryGroupServiceMock.Verify(x => x.RemoveGroupMemberAsync(readerGroupId, allUsersGroupId), Times.Once);
        }
    }
}
