using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Common.Enums;
using DataCatalog.Api.Exceptions;
using DataCatalog.Api.Repositories;
using DataCatalog.Api.Services;
using FluentAssertions;
using Moq;
using Xunit;
using Dataset = DataCatalog.Data.Model.Dataset;
using DatasetDuration = DataCatalog.Data.Model.DatasetDuration;
using Duration = DataCatalog.Data.Model.Duration;
using Transformation = DataCatalog.Data.Model.Transformation;
using TransformationDataset = DataCatalog.Data.Model.TransformationDataset;
using DataCatalog.Api.Data;
using DataCatalog.Api.Messages;
using DataCatalog.DatasetResourceManagement.Messages;
using Rebus.Bus;
using Shouldly;

namespace DataCatalog.Api.UnitTests.Services
{
    public class DatasetServiceTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly DatasetCreateRequest _datasetCreateRequest;

        public DatasetServiceTests()
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            // Setup automapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });

            var mapper = config.CreateMapper();
            _fixture.Inject(mapper);
            _fixture.Freeze<IMapper>();
            _datasetCreateRequest = _fixture.Create<DatasetCreateRequest>();
            _datasetCreateRequest.Status = DatasetStatus.Draft;
            _datasetCreateRequest.Confidentiality = Confidentiality.Confidential;
        }

        [Fact]
        public async Task FindByIdAsync_InvalidId_ShouldReturnNull()
        {
            // Arrange
            var datasetRepositoryMock = new Mock<IDatasetRepository>();
            var invalidId = Guid.NewGuid();
            datasetRepositoryMock.Setup(x => x.FindByIdAsync(invalidId)).ReturnsAsync((Dataset)null);
            _fixture.Inject(datasetRepositoryMock.Object);
            _fixture.Freeze<IDatasetRepository>();
            var datasetService = _fixture.Create<DatasetService>();

            // Act
            var dataset = await datasetService.FindByIdAsync(invalidId);

            // Assert
            dataset.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_ValidId_ShouldReturnNull()
        {
            // Arrange
            var datasetRepositoryMock = new Mock<IDatasetRepository>();
            var datasetEntity = _fixture.Create<Dataset>();
            datasetRepositoryMock.Setup(x => x.FindByIdAsync(datasetEntity.Id)).ReturnsAsync(datasetEntity);
            _fixture.Inject(datasetRepositoryMock.Object);
            _fixture.Freeze<IDatasetRepository>();
            var datasetService = _fixture.Create<DatasetService>();

            // Act
            var dataset = await datasetService.FindByIdAsync(datasetEntity.Id);

            // Assert
            dataset.Should().NotBeNull();
            dataset.Id.Should().Be(datasetEntity.Id);
        }


        [Fact]
        public void SaveAsync_MissingCategories_ShouldThrowException()
        {
            // Arrange
            var datasetService = _fixture.Create<DatasetService>();
            _datasetCreateRequest.Categories = null;

            // Act / Assert
            Func<Task> f = async () => await datasetService.SaveAsync(_datasetCreateRequest);
            f.Should().ThrowAsync<ValidationExceptionCollection>().WithMessage("*Dataset must be assigned at least one category*");
        }

        [Fact]
        public void SaveAsync_MissingContact_ShouldThrowException()
        {
            // Arrange
            var datasetService = _fixture.Create<DatasetService>();
            _datasetCreateRequest.Contact = null;

            // Act / Assert
            Func<Task> f = async () => await datasetService.SaveAsync(_datasetCreateRequest);
            f.Should().ThrowAsync<ValidationExceptionCollection>().WithMessage("*Dataset must have a contact*");
        }

        [Fact]
        public void SaveAsync_MissingName_ShouldThrowException()
        {
            // Arrange
            var datasetService = _fixture.Create<DatasetService>();
            _datasetCreateRequest.Name = null;

            // Act / Assert
            Func<Task> f = async () => await datasetService.SaveAsync(_datasetCreateRequest);
            f.Should().ThrowAsync<ValidationExceptionCollection>().WithMessage("*Dataset must have a name*");
        }

        [Fact]
        public void SaveAsync_MissingFieldName_ShouldThrowException()
        {
            // Arrange
            var datasetService = _fixture.Create<DatasetService>();
            _datasetCreateRequest.DataFields.First().Name = "";

            // Act / Assert
            Func<Task> f = async () => await datasetService.SaveAsync(_datasetCreateRequest);
            f.Should().ThrowAsync<ValidationExceptionCollection>().WithMessage("*Data field must have a name*");
        }

        [Fact]
        public void SaveAsync_MissingFieldType_ShouldThrowException()
        {
            // Arrange
            var datasetService = _fixture.Create<DatasetService>();
            _datasetCreateRequest.DataFields.First().Type = null;

            // Act / Assert
            Func<Task> f = async () => await datasetService.SaveAsync(_datasetCreateRequest);
            f.Should().ThrowAsync<ValidationExceptionCollection>().WithMessage("*Data field must have a type*");
        }

        [Fact]
        public async Task SaveAsync_ValidateReturnObject()
        {
            // Arrange
            var datasetDurationRepositoryMock = new Mock<IDatasetDurationRepository>();
            var frequency = new Duration { Code = Guid.NewGuid().ToString() };
            var resolution = new Duration { Code = Guid.NewGuid().ToString() };
            var frequencyDatasetDuration = new DatasetDuration { Duration = frequency };
            var resolutionDatasetDuration = new DatasetDuration { Duration = resolution };
            datasetDurationRepositoryMock.Setup(x => x.FindByDatasetAndTypeAsync(It.IsAny<Guid>(), DurationType.Frequency)).ReturnsAsync(frequencyDatasetDuration);
            datasetDurationRepositoryMock.Setup(x => x.FindByDatasetAndTypeAsync(It.IsAny<Guid>(), DurationType.Resolution)).ReturnsAsync(resolutionDatasetDuration);
            _fixture.Inject(datasetDurationRepositoryMock.Object);
            _fixture.Freeze<IDatasetDurationRepository>();
            var durationRepository = new Mock<IDurationRepository>();
            durationRepository.Setup(x => x.FindByCodeAsync(_datasetCreateRequest.Frequency.Code.ToUpper())).ReturnsAsync(frequency);
            durationRepository.Setup(x => x.FindByCodeAsync(_datasetCreateRequest.Resolution.Code.ToUpper())).ReturnsAsync(resolution);
            _fixture.Inject(durationRepository.Object);
            _fixture.Freeze<IDurationRepository>();
            var transformationRepositoryMock = new Mock<ITransformationRepository>();
            var transformation = _fixture.Create<Transformation>();
            transformation.Id = _datasetCreateRequest.SourceTransformation.Id.Value;
            transformationRepositoryMock.Setup(x => x.FindByIdAsync(_datasetCreateRequest.SourceTransformation.Id.Value)).ReturnsAsync(transformation);
            _fixture.Inject(transformationRepositoryMock.Object);
            _fixture.Freeze<ITransformationRepository>();
            var messageBusSenderMock = new Mock<IBus>();
            _fixture.Inject(messageBusSenderMock.Object);
            var datasetService = _fixture.Create<DatasetService>();

            // Act
            var dataset = await datasetService.SaveAsync(_datasetCreateRequest);

            // Assert
            dataset.Should().NotBeNull();
            dataset.Confidentiality.Should().Be(_datasetCreateRequest.Confidentiality);
            dataset.ContactId.Should().Be(_datasetCreateRequest.Contact.Id);
            dataset.DataContracts.Count.Should().Be(3);
            dataset.DataContracts.First().DataSourceId.Should().Be(_datasetCreateRequest.DataSources.First().Id);
            dataset.DataFields.Length.Should().Be(3);
            dataset.DataFields.First().Description.Should().Be(_datasetCreateRequest.DataFields.First().Description);
            dataset.DataFields.First().Name.Should().Be(_datasetCreateRequest.DataFields.First().Name);
            dataset.DataFields.First().Type.Should().Be(_datasetCreateRequest.DataFields.First().Type.ToString());
            dataset.DataFields.First().Validation.Should().Be(_datasetCreateRequest.DataFields.First().Validation);
            dataset.DatasetCategories.Count.Should().Be(3);
            dataset.DatasetCategories.First().CategoryId.Should().Be(_datasetCreateRequest.Categories.First().Id);
            dataset.DatasetDurations.Count.Should().Be(2);
            dataset.DatasetDurations.Any(d => d.DurationType == DurationType.Frequency).Should().BeTrue();
            dataset.DatasetDurations.Any(d => d.DurationType == DurationType.Resolution).Should().BeTrue();
            dataset.DatasetDurations.Single(d => d.DurationType == DurationType.Frequency).Duration.Code.Should().Be((frequency.Code));
            dataset.DatasetDurations.Single(d => d.DurationType == DurationType.Resolution).Duration.Code.Should().Be((resolution.Code));
            dataset.Description.Should().Be(_datasetCreateRequest.Description);
            dataset.Name.Should().Be(_datasetCreateRequest.Name);
            dataset.Owner.Should().Be(_datasetCreateRequest.Owner);
            dataset.SlaDescription.Should().Be(_datasetCreateRequest.SlaDescription);
            dataset.SlaLink.Should().Be(_datasetCreateRequest.SlaLink);
            dataset.Status.Should().Be(_datasetCreateRequest.Status);
            dataset.Version.Should().Be(0);
            messageBusSenderMock.Verify(mock => mock.Publish(It.Is<DatasetCreatedMessage>(dto =>
                dto.DatasetName == _datasetCreateRequest.Name  &&
                dto.Owner == _datasetCreateRequest.Owner), It.IsAny<IDictionary<string, string>>()));
        }

        [Fact]
        public async Task DeleteAsync_InvalidDatasetId_MustNotCallRemoveOrComplete()
        {
            // Arrange
            var datasetRepositoryMock = new Mock<IDatasetRepository>();
            var datasetEntity = _fixture.Create<Data.Domain.Dataset>();
            datasetRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Dataset)null);
            _fixture.Inject(datasetRepositoryMock.Object);
            _fixture.Freeze<IDatasetRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IDatasetRepository>();
            var datasetService = _fixture.Create<DatasetService>();

            // Act
            var exception = await Record.ExceptionAsync(() => datasetService.DeleteAsync(datasetEntity.Id));

            // Assert
            exception
                .ShouldNotBeNull()
                .ShouldBeOfType<ObjectNotFoundException>();
            datasetRepositoryMock.Verify(mock => mock.Remove(It.IsAny<Dataset>()), Times.Never());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Never());
        }

        [Fact]
        public async Task DeleteAsync_ValidDatasetId_MustCallURemoveAndComplete()
        {
            // Arrange
            var datasetRepositoryMock = new Mock<IDatasetRepository>();
            var datasetEntity = _fixture.Create<Dataset>();
            var domainModelEntity = _fixture.Create<Data.Domain.Dataset>();
            domainModelEntity.Id = datasetEntity.Id;
            datasetRepositoryMock.Setup(x => x.FindByIdAsync(datasetEntity.Id)).ReturnsAsync(datasetEntity);
            _fixture.Inject(datasetRepositoryMock.Object);
            _fixture.Freeze<IDatasetRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            _fixture.Inject(unitOfWorkMock.Object);
            _fixture.Freeze<IDatasetRepository>();
            var datasetService = _fixture.Create<DatasetService>();

            // Act
            await datasetService.DeleteAsync(domainModelEntity.Id);

            // Assert
            datasetRepositoryMock.Verify(mock => mock.Remove(datasetEntity), Times.Once());
            unitOfWorkMock.Verify(mock => mock.CompleteAsync(), Times.Once());
        }

        [Fact]
        public async Task GetDatasetByCategoryAsync_EmptySet_MustReturnEmptyList()
        {
            // Arrange
            var datasetRepositoryMock = new Mock<IDatasetRepository>();
            datasetRepositoryMock.Setup(x => x.GetDatasetByCategoryAsync(It.IsAny<Guid>(), It.IsAny<SortType>(), 0, 0, 0)).ReturnsAsync(new List<Dataset>());
            _fixture.Inject(datasetRepositoryMock.Object);
            _fixture.Freeze<IDatasetRepository>();
            var datasetService = _fixture.Create<DatasetService>();

            // Act
            var result = await datasetService.GetDatasetByCategoryAsync(Guid.Empty, SortType.ByNameAscending, 0, 0, 0); 

            // Assert
            var resultArray = result as Data.Domain.Dataset[] ?? result.ToArray();
            resultArray.Should().NotBeNull();
            resultArray.Should().BeEmpty();
        }

        [Fact]
        public async Task GetDatasetByCategoryAsync_NotEmptySet_MustReturnList()
        {
            var datasetRepositoryMock = new Mock<IDatasetRepository>();
            datasetRepositoryMock.Setup(x => x.GetDatasetByCategoryAsync(It.IsAny<Guid>(), It.IsAny<SortType>(), 0, 0, 0)).ReturnsAsync(new List<Dataset> { new Dataset() });
            _fixture.Inject(datasetRepositoryMock.Object);
            _fixture.Freeze<IDatasetRepository>();
            var datasetService = _fixture.Create<DatasetService>();

            // Act
            var result = await datasetService.GetDatasetByCategoryAsync(Guid.Empty, SortType.ByNameAscending, 0, 0, 0);

            // Assert
            var resultArray = result as Data.Domain.Dataset[] ?? result.ToArray();
            resultArray.Should().NotBeNull();
            resultArray.Length.Should().Be(1);
        }

        [Fact]
        public async Task GetDatasetsBySearchTermAsync_EmptySet_MustReturnEmptyList()
        {
            // Arrange
            var datasetRepositoryMock = new Mock<IDatasetRepository>();
            datasetRepositoryMock.Setup(x => x.GetDatasetsBySearchTermQueryAsync("", It.IsAny<SortType>(), 0, 0, 0)).ReturnsAsync(new List<Dataset>());
            _fixture.Inject(datasetRepositoryMock.Object);
            _fixture.Freeze<IDatasetRepository>();
            var datasetService = _fixture.Create<DatasetService>();

            // Act
            var result = await datasetService.GetDatasetsBySearchTermAsync("", SortType.ByNameAscending, 0, 0, 0);

            // Assert
            var resultArray = result as Data.Domain.Dataset[] ?? result.ToArray();
            resultArray.Should().NotBeNull();
            resultArray.Should().BeEmpty();
        }

        [Fact]
        public async Task GetDatasetsBySearchTermAsync_NotEmptySet_MustReturnList()
        {
            // Arrange
            var datasetRepositoryMock = new Mock<IDatasetRepository>();
            datasetRepositoryMock.Setup(x => x.GetDatasetsBySearchTermQueryAsync("", It.IsAny<SortType>(), 0, 0, 0)).ReturnsAsync(new List<Dataset> { new Dataset() });
            _fixture.Inject(datasetRepositoryMock.Object);
            _fixture.Freeze<IDatasetRepository>();
            var datasetService = _fixture.Create<DatasetService>();

            // Act
            var result = await datasetService.GetDatasetsBySearchTermAsync("", SortType.ByNameAscending, 0, 0, 0);

            // Assert
            var resultArray = result as Data.Domain.Dataset[] ?? result.ToArray();
            resultArray.Should().NotBeNull();
            resultArray.Length.Should().Be(1);
        }

        [Fact]
        public async Task GetDatasetLineageAsync_ValidateTransformations()
        {
            // Arrange
            _fixture.RepeatCount = 5;
            var datasets = _fixture.Create<List<Dataset>>();
            var transformations = _fixture.Create<List<Transformation>>();
            var transformationDatasets = _fixture.Create<List<TransformationDataset>>();

            datasets[0].TransformationDatasets = new List<TransformationDataset> { transformationDatasets[0], transformationDatasets[4] };
            datasets[1].TransformationDatasets = new List<TransformationDataset> { transformationDatasets[1] };
            datasets[2].TransformationDatasets = new List<TransformationDataset> { transformationDatasets[2], transformationDatasets[3] };
            transformations[0].TransformationDatasets = new List<TransformationDataset> { transformationDatasets[0], transformationDatasets[1], transformationDatasets[2] };
            transformations[1].TransformationDatasets = new List<TransformationDataset> { transformationDatasets[3] };
            transformations[2].TransformationDatasets = new List<TransformationDataset> { transformationDatasets[4] };

            transformationDatasets[0].TransformationDirection = TransformationDirection.Source;
            transformationDatasets[0].TransformationId = transformations[0].Id;
            transformationDatasets[0].Transformation = transformations[0];
            transformationDatasets[0].DatasetId = datasets[0].Id;
            transformationDatasets[0].Dataset = datasets[0];
            transformationDatasets[1].TransformationDirection = TransformationDirection.Source;
            transformationDatasets[1].TransformationId = transformations[0].Id;
            transformationDatasets[1].Transformation = transformations[0];
            transformationDatasets[1].DatasetId = datasets[1].Id;
            transformationDatasets[1].Dataset = datasets[1];
            transformationDatasets[2].TransformationDirection = TransformationDirection.Sink;
            transformationDatasets[2].TransformationId = transformations[0].Id;
            transformationDatasets[2].Transformation = transformations[0];
            transformationDatasets[2].DatasetId = datasets[2].Id;
            transformationDatasets[2].Dataset = datasets[2];
            transformationDatasets[3].TransformationDirection = TransformationDirection.Source;
            transformationDatasets[3].TransformationId = transformations[1].Id;
            transformationDatasets[3].Transformation = transformations[1];
            transformationDatasets[3].DatasetId = datasets[2].Id;
            transformationDatasets[3].Dataset = datasets[2];
            transformationDatasets[4].TransformationDirection = TransformationDirection.Sink;
            transformationDatasets[4].TransformationId = transformations[2].Id;
            transformationDatasets[4].Transformation = transformations[2];
            transformationDatasets[4].DatasetId = datasets[0].Id;
            transformationDatasets[4].Dataset = datasets[0];

            var datasetRepositoryMock = new Mock<IDatasetRepository>();
            datasets.ForEach(d => datasetRepositoryMock.Setup(x => x.FindByIdAsync(d.Id)).ReturnsAsync(d));
            _fixture.Inject(datasetRepositoryMock.Object);
            _fixture.Freeze<IDatasetRepository>();

            var transformationDatasetRepositoryMock = new Mock<ITransformationDatasetRepository>();
            datasets.ForEach(d => transformationDatasetRepositoryMock.Setup(x => x.FindAllTransformationDatasetsForDatasetIdAndDirectionAsync(d.Id, TransformationDirection.Source))
                .ReturnsAsync(transformationDatasets.Where(td => td.DatasetId == d.Id && td.TransformationDirection == TransformationDirection.Source).ToList()));
            datasets.ForEach(d => transformationDatasetRepositoryMock.Setup(x => x.FindAllTransformationDatasetsForDatasetIdAndDirectionAsync(d.Id, TransformationDirection.Sink))
                .ReturnsAsync(transformationDatasets.Where(td => td.DatasetId == d.Id && td.TransformationDirection == TransformationDirection.Sink).ToList()));
            transformations.ForEach(t => transformationDatasetRepositoryMock.Setup(x => x.FindAllByTransformationIdAndDirectionAsync(t.Id, TransformationDirection.Source))
                .ReturnsAsync(transformationDatasets.Where(td => td.TransformationId == t.Id && td.TransformationDirection == TransformationDirection.Source).ToList()));
            transformations.ForEach(t => transformationDatasetRepositoryMock.Setup(x => x.FindAllByTransformationIdAndDirectionAsync(t.Id, TransformationDirection.Sink))
                .ReturnsAsync(transformationDatasets.Where(td => td.TransformationId == t.Id && td.TransformationDirection == TransformationDirection.Sink).ToList()));
            _fixture.Inject(transformationDatasetRepositoryMock.Object);
            _fixture.Freeze<ITransformationDatasetRepository>();

            var datasetService = _fixture.Create<DatasetService>();

            //Act
            var result = await datasetService.GetDatasetLineageAsync(datasets[0].Id);

            // Assert
            result.Should().NotBeNull();
            result.SinkTransformations.Should().NotBeNull();
            result.SinkTransformations.Count.Should().Be(1);
            result.SourceTransformations.Should().NotBeNull();
            result.SourceTransformations.Count.Should().Be(1);
            result.SinkTransformations.First().Datasets.Count.Should().Be(1); 
            result.SourceTransformations.First().Datasets.Count.Should().Be(0);
        }

    }
}
