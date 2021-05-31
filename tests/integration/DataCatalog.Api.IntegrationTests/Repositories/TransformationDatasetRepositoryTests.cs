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
    public class TransformationDatasetRepositoryTests : BaseTest, IDisposable
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly DataCatalogContext _context;
        private readonly List<Dataset> _datasets;
        private readonly List<Transformation> _transformations;
        private readonly List<TransformationDataset> _transformationDatasets;

        public TransformationDatasetRepositoryTests()
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
                new Transformation { Id = Guid.NewGuid() },
                new Transformation { Id = Guid.NewGuid() }
            };

            _transformationDatasets = new List<TransformationDataset>
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
            _transformationDatasets.ForEach(t => _context.TransformationDatasets.Add(t));

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
            var transformationDatasetRepository = new TransformationDatasetRepository(_context);

            // ACT
            var transformationDatasets = await transformationDatasetRepository.ListAsync();

            // ASSERT
            var transformationDatasetArray = transformationDatasets as TransformationDataset[] ?? transformationDatasets.ToArray();
            transformationDatasetArray.Should().NotBeNull();
            transformationDatasetArray.Length.Should().Be(6);
        }

        [Theory]
        [InlineData(TransformationDirection.Sink)]
        [InlineData(TransformationDirection.Source)]
        public async Task FindByDatasetIdAndDirectionAsync_InvalidDatasetId_ShouldReturnNull(TransformationDirection direction)
        {
            // ARRANGE
            var transformationDatasetRepository = new TransformationDatasetRepository(_context);
            var invalidDatasetId = Guid.NewGuid();

            // ACT
            var transformationDataset = await transformationDatasetRepository.FindByDatasetIdAndDirectionAsync(invalidDatasetId, direction);

            // ASSERT
            transformationDataset.Should().BeNull();
        }

        [Fact]
        public async Task FindByDatasetIdAndDirectionAsync_ValidDatasetIdWrongDirection_ShouldReturnNull()
        {
            // ARRANGE
            var transformationDatasetRepository = new TransformationDatasetRepository(_context);

            // ACT
            var transformationDataset = await transformationDatasetRepository.FindByDatasetIdAndDirectionAsync(_datasets[0].Id, TransformationDirection.Sink);

            // ASSERT
            transformationDataset.Should().BeNull();
        }

        [Fact]
        public async Task FindByDatasetIdAndDirectionAsync_ValidDatasetId_ShouldReturnTransformationDataset()
        {
            // ARRANGE
            var transformationDatasetRepository = new TransformationDatasetRepository(_context);

            // ACT
            var transformationDataset = await transformationDatasetRepository.FindByDatasetIdAndDirectionAsync(_datasets[0].Id, TransformationDirection.Source);

            // ASSERT
            transformationDataset.Should().NotBeNull();
            transformationDataset.DatasetId.Should().Be(_datasets[0].Id);
            transformationDataset.TransformationId.Should().Be(_transformations[0].Id);
        }

        [Theory]
        [InlineData(TransformationDirection.Sink)]
        [InlineData(TransformationDirection.Source)]
        public async Task FindAllTransformationDatasetsForDatasetIdAndDirectionAsync_InvalidDatasetId_ShouldReturnNull(TransformationDirection direction)
        {
            // ARRANGE
            var transformationDatasetRepository = new TransformationDatasetRepository(_context);
            var invalidDatasetId = Guid.NewGuid();

            // ACT
            var transformationDataset = await transformationDatasetRepository.FindAllTransformationDatasetsForDatasetIdAndDirectionAsync(invalidDatasetId, direction);

            // ASSERT
            var transformationDatasetArray = transformationDataset as TransformationDataset[] ?? transformationDataset.ToArray();
            transformationDatasetArray.Should().NotBeNull();
            transformationDatasetArray.Should().BeEmpty();
        }

        [Fact]
        public async Task FindAllTransformationDatasetsForDatasetIdAndDirectionAsync_ValidDatasetIdWrongDirection_ShouldReturnNull()
        {
            // ARRANGE
            var transformationDatasetRepository = new TransformationDatasetRepository(_context);

            // ACT
            var transformationDataset = await transformationDatasetRepository.FindAllTransformationDatasetsForDatasetIdAndDirectionAsync(_datasets[0].Id, TransformationDirection.Sink);

            // ASSERT
            var transformationDatasetArray = transformationDataset as TransformationDataset[] ?? transformationDataset.ToArray();
            transformationDatasetArray.Should().NotBeNull();
            transformationDatasetArray.Should().BeEmpty();
        }

        [Fact]
        public async Task FindAllTransformationDatasetsForDatasetIdAndDirectionAsync_ValidDatasetId_ShouldReturnTransformationDataset()
        {
            // ARRANGE
            var transformationDatasetRepository = new TransformationDatasetRepository(_context);

            // ACT
            var transformationDataset = await transformationDatasetRepository.FindAllTransformationDatasetsForDatasetIdAndDirectionAsync(_datasets[0].Id, TransformationDirection.Source);

            // ASSERT
            transformationDataset.Should().NotBeNull();
        }

        [Theory]
        [InlineData(TransformationDirection.Sink)]
        [InlineData(TransformationDirection.Source)]
        public async Task FindAllByTransformationIdAndDirectionAsync_InvalidTransformationId_ShouldReturnNull(TransformationDirection direction)
        {
            // ARRANGE
            var transformationDatasetRepository = new TransformationDatasetRepository(_context);
            var invalidTransformationId = Guid.NewGuid();

            // ACT
            var transformationDataset = await transformationDatasetRepository.FindAllByTransformationIdAndDirectionAsync(invalidTransformationId, direction);

            // ASSERT
            var transformationDatasetArray = transformationDataset as TransformationDataset[] ?? transformationDataset.ToArray();
            transformationDatasetArray.Should().NotBeNull();
            transformationDatasetArray.Should().BeEmpty();
        }

        [Fact]
        public async Task FindAllByTransformationIdAndDirectionAsync_ValidTransformationIdWrongDirection_ShouldReturnNull()
        {
            // ARRANGE
            var transformationDatasetRepository = new TransformationDatasetRepository(_context);

            // ACT
            var transformationDataset = await transformationDatasetRepository.FindAllByTransformationIdAndDirectionAsync(_transformations[2].Id, TransformationDirection.Sink);

            // ASSERT
            var transformationDatasetArray = transformationDataset as TransformationDataset[] ?? transformationDataset.ToArray();
            transformationDatasetArray.Should().NotBeNull();
            transformationDatasetArray.Should().BeEmpty();
        }

        [Fact]
        public async Task FindAllByTransformationIdAndDirectionAsync_ValidTransformationId_ShouldReturTransformationDataset()
        {
            // ARRANGE
            var transformationDatasetRepository = new TransformationDatasetRepository(_context);

            // ACT
            var transformationDataset = await transformationDatasetRepository.FindAllByTransformationIdAndDirectionAsync(_transformations[0].Id, TransformationDirection.Source);

            // ASSERT
            transformationDataset.Should().NotBeNull();
        }

        [Fact]
        public async Task AddAsync_ShouldAddTransformationDataset()
        {
            // ARRANGE
            var transformationDatasetRepository = new TransformationDatasetRepository(_context);
            var transformationDatasetEntity = new TransformationDataset
            {
                DatasetId = _datasets[0].Id,
                TransformationId = _transformations[1].Id,
                TransformationDirection = TransformationDirection.Sink
            };

            // ACT
            await transformationDatasetRepository.AddAsync(transformationDatasetEntity);
            await _context.SaveChangesAsync();

            // ASSERT
            var transformationDatasets = await transformationDatasetRepository.ListAsync();
            var transformationDatasetArray = transformationDatasets as TransformationDataset[] ?? transformationDatasets.ToArray();
            transformationDatasetArray.Should().NotBeNull();
            transformationDatasetArray.SingleOrDefault(c => c.DatasetId == transformationDatasetEntity.DatasetId && c.TransformationId == transformationDatasetEntity.TransformationId && c.TransformationDirection == transformationDatasetEntity.TransformationDirection).Should().NotBeNull();
        }

        [Fact]
        public async Task Update_ChangeDirection_MustUpdateDirection()
        {
            // ARRANGE
            var transformationDatasetRepository = new TransformationDatasetRepository(_context);
            var transformationDatasetToChange = _transformationDatasets[0];
            transformationDatasetToChange.TransformationDirection =
                transformationDatasetToChange.TransformationDirection == TransformationDirection.Sink
                    ? TransformationDirection.Source
                    : TransformationDirection.Sink;

            // ACT
            transformationDatasetRepository.Update(transformationDatasetToChange);
            await _context.SaveChangesAsync();

            // ASSERT
            var transformationDatasets = await transformationDatasetRepository.ListAsync();
            var transformationDatasetArray = transformationDatasets as TransformationDataset[] ?? transformationDatasets.ToArray();
            transformationDatasetArray.Should().NotBeNull();
            transformationDatasetArray.SingleOrDefault(c => c.DatasetId == transformationDatasetToChange.DatasetId && c.TransformationDirection == transformationDatasetToChange.TransformationDirection).Should().NotBeNull();
        }

       [Fact]
        public async Task RemoveAsync_ShouldBeRemoved()
        {
            // ARRANGE
            var transformationDatasetRepository = new TransformationDatasetRepository(_context);
            var transformationDatasetToRemove = _transformationDatasets[0];

            // ACT
            await transformationDatasetRepository.RemoveAsync(transformationDatasetToRemove);
            await _context.SaveChangesAsync();

            // ASSERT
            var allTransformationDatasets = await transformationDatasetRepository.ListAsync();
            var transformationDatasetArray = allTransformationDatasets as TransformationDataset[] ?? allTransformationDatasets.ToArray();
            transformationDatasetArray.Should().NotBeNull();
            transformationDatasetArray.SingleOrDefault(t =>
                t.DatasetId == transformationDatasetToRemove.DatasetId &&
                t.TransformationId == transformationDatasetToRemove.TransformationId && t.TransformationDirection ==
                transformationDatasetToRemove.TransformationDirection).Should().BeNull();
        }

        [Fact]
        public async Task Remove_ShouldBeRemoved()
        {
            // ARRANGE
            var transformationDatasetRepository = new TransformationDatasetRepository(_context);
            var transformationDatasetsToRemove = _transformationDatasets.Take(2).ToList();

            // ACT
            transformationDatasetRepository.Remove(transformationDatasetsToRemove);
            await _context.SaveChangesAsync();

            // ASSERT
            var allTransformationDatasets = await transformationDatasetRepository.ListAsync();
            var transformationDatasetArray = allTransformationDatasets as TransformationDataset[] ?? allTransformationDatasets.ToArray();
            transformationDatasetArray.Should().NotBeNull();

            transformationDatasetsToRemove.ForEach(td =>
                transformationDatasetArray.SingleOrDefault(t =>
                    t.DatasetId == td.DatasetId &&
                    t.TransformationId == td.TransformationId && t.TransformationDirection ==
                    td.TransformationDirection).Should().BeNull());
        }
    }
}
