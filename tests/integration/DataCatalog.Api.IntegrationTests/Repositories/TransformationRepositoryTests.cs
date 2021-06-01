using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;

using DataCatalog.Common.Data;
using DataCatalog.Data.Model;
using DataCatalog.Common.Enums;
using DataCatalog.Api.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using DataCatalog.Data;

namespace DataCatalog.Api.IntegrationTests.Repositories
{
    public class TransformationRepositoryTests : BaseTest, IDisposable
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly DataCatalogContext _context;
        private readonly List<Dataset> _datasets;
        private readonly List<Transformation> _transformations;

        public TransformationRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataCatalogContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new DataCatalogContext(options);

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            
            // We do not use autofixture due to un-needed complexity...
            _datasets = new List<Dataset>
            {
                new Dataset { Id = Guid.NewGuid() },
                new Dataset { Id = Guid.NewGuid() },
                new Dataset { Id = Guid.NewGuid() },
                new Dataset { Id = Guid.NewGuid() },
                new Dataset { Id = Guid.NewGuid() },
                new Dataset { Id = Guid.NewGuid() }
            };

            _transformations = new List<Transformation>
            {
                new Transformation { Id = Guid.NewGuid() },
                new Transformation { Id = Guid.NewGuid() }
            };

            var transformationDatasets = new List<TransformationDataset>
            {
                new TransformationDataset { DatasetId = _datasets[0].Id, TransformationId = _transformations[0].Id, TransformationDirection = TransformationDirection.Source },
                new TransformationDataset { DatasetId = _datasets[1].Id, TransformationId = _transformations[0].Id, TransformationDirection = TransformationDirection.Source },
                new TransformationDataset { DatasetId = _datasets[2].Id, TransformationId = _transformations[0].Id, TransformationDirection = TransformationDirection.Sink },
                new TransformationDataset { DatasetId = _datasets[3].Id, TransformationId = _transformations[1].Id, TransformationDirection = TransformationDirection.Source },
                new TransformationDataset { DatasetId = _datasets[4].Id, TransformationId = _transformations[1].Id, TransformationDirection = TransformationDirection.Source },
                new TransformationDataset { DatasetId = _datasets[5].Id, TransformationId = _transformations[1].Id, TransformationDirection = TransformationDirection.Sink }
            };

            _datasets.ForEach(d => _context.Datasets.Add(d));
            _transformations.ForEach(c => _context.Transformations.Add(c));
            transformationDatasets.ForEach(t => _context.TransformationDatasets.Add(t));

            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task ListAsync_ShouldReturnAll()
        {
            // ARRANGE
            var transformationRepository = new TransformationRepository(_context);

            // ACT
            var transformations = await transformationRepository.ListAsync();

            // ASSERT
            var transformationArray = transformations as Transformation[] ?? transformations.ToArray();
            transformationArray.Should().NotBeNull();
            transformationArray.Length.Should().Be(2);
        }

        [Fact]
        public async Task AddAsync_ShouldAddTransformation()
        {
            // ARRANGE
            var transformationRepository = new TransformationRepository(_context);
            var transformationEntity = _fixture.Create<Transformation>();

            // ACT
            await transformationRepository.AddAsync(transformationEntity);
            await _context.SaveChangesAsync();

            // ASSERT
            var transformations = await transformationRepository.ListAsync();
            var transformationArray = transformations as Transformation[] ?? transformations.ToArray();
            transformationArray.Should().NotBeNull();
            transformationArray.Length.Should().Be(3);
            transformationArray.SingleOrDefault(c => c.Id == transformationEntity.Id).Should().NotBeNull();
        }

        [Fact]
        public async Task FindByIdAsync_InvalidId_ShouldReturnNull()
        {
            // ARRANGE
            var transformationRepository = new TransformationRepository(_context);
            var invalidId = Guid.NewGuid();

            // ACT
            var transformation = await transformationRepository.FindByIdAsync(invalidId);

            // ASSERT
            transformation.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_ValidId_ShouldReturnNull()
        {
            // ARRANGE
            var transformationRepository = new TransformationRepository(_context);

            // ACT
            var transformation = await transformationRepository.FindByIdAsync(_transformations[0].Id);

            // ASSERT
            transformation.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByDatasetIdAsync_ShouldReturnTransformationsWithSimilarSource()
        {
            // ARRANGE
            var transformationRepository = new TransformationRepository(_context);
            var dataset = _datasets[0];

            // ACT
            var transformations = await transformationRepository.GetByDatasetIdsAsync(new[] { dataset.Id });

            // ASSERT
            var transformationsArray = transformations as Transformation[] ?? transformations.ToArray();
            transformationsArray.Should().NotBeNull();
            transformationsArray.Length.Should().Be(1);
            transformationsArray[0].Id.Should().Be(_transformations[0].Id);
        }

        [Fact]
        public async Task Update_ChangeProperties_PropertiesShouldBeChanged()
        {
            // ARRANGE
            var transformationRepository = new TransformationRepository(_context);
            var transformationToUpdate = _transformations[0];
            transformationToUpdate.Description = Guid.NewGuid().ToString();
            transformationToUpdate.ShortDescription = Guid.NewGuid().ToString();

            // ACT
            transformationRepository.Update(transformationToUpdate);
            await _context.SaveChangesAsync();

            // ASSERT
            var updatedDataContract = await transformationRepository.FindByIdAsync(transformationToUpdate.Id);
            updatedDataContract.Should().NotBeNull();
            updatedDataContract.Description.Should().Be(transformationToUpdate.Description);
            updatedDataContract.ShortDescription.Should().Be(transformationToUpdate.ShortDescription);
        }

        [Fact]
        public async Task Remove_ShouldBeRemoved()
        {
            // ARRANGE
            var transformationRepository = new TransformationRepository(_context);
            var theTransformationToRemove = _transformations[0];

            // ACT
            await transformationRepository.RemoveAsync(theTransformationToRemove);
            await _context.SaveChangesAsync();

            // ASSERT
            var nonExistingDataContract = await transformationRepository.FindByIdAsync(theTransformationToRemove.Id);
            nonExistingDataContract.Should().BeNull();
        }
    }
}
