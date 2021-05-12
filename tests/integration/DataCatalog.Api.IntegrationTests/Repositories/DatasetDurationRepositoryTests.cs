using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using DataCatalog.Api.Data;
using DataCatalog.Api.Data.Common;
using DataCatalog.Api.Data.Model;
using DataCatalog.Api.Enums;
using DataCatalog.Api.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DataCatalog.Api.IntegrationTests.Repositories
{
    public class DatasetDurationRepositoryTests : BaseTest, IDisposable
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly DataCatalogContext _context;
        private readonly List<DatasetDuration> _datasetDuration;

        public DatasetDurationRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataCatalogContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new DataCatalogContext(options);

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _datasetDuration = _fixture.Create<IEnumerable<DatasetDuration>>().ToList();
            _datasetDuration.ForEach(c => _context.DatasetDurations.Add(c));
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
            var datasetDurationRepository = new DatasetDurationRepository(_context);

            // ACT
            var datasetDurations = await datasetDurationRepository.ListAsync();

            // ASSERT
            var datasetDurationArray = datasetDurations as DatasetDuration[] ?? datasetDurations.ToArray();
            datasetDurationArray.Should().NotBeNull();
            datasetDurationArray.Length.Should().Be(3);
            datasetDurationArray.Count(c => c.DatasetId == _datasetDuration[0].DatasetId).Should().Be(1);
            datasetDurationArray.Count(c => c.DatasetId == _datasetDuration[1].DatasetId).Should().Be(1);
            datasetDurationArray.Count(c => c.DatasetId == _datasetDuration[2].DatasetId).Should().Be(1);
        }

        [Fact]
        public async Task FindByDatasetAndTypeAsync_InvalidDatasetId_ShouldReturnNull()
        {
            // ARRANGE
            var datasetDurationRepository = new DatasetDurationRepository(_context);
            var invalidDatasetId = Guid.NewGuid();

            // ACT
            var datasetDuration = await datasetDurationRepository.FindByDatasetAndTypeAsync(invalidDatasetId, DurationType.Frequency);

            // ASSERT
            datasetDuration.Should().BeNull();
        }

        [Fact]
        public async Task FindByDatasetAndTypeAsync_ValidId_ShouldReturnDatasetDuration()
        {
            // ARRANGE
            var datasetDurationRepository = new DatasetDurationRepository(_context);

            // ACT
            var datasetDuration = await datasetDurationRepository.FindByDatasetAndTypeAsync(_datasetDuration[1].DatasetId, _datasetDuration[1].DurationType);

            // ASSERT
            datasetDuration.Should().NotBeNull();
            datasetDuration.DatasetId.Should().Be(_datasetDuration[1].DatasetId);
        }

        [Fact]
        public async Task AddAsync_ShouldAddDatasetDuration()
        {
            // ARRANGE
            var datasetDurationRepository = new DatasetDurationRepository(_context);
            var datasetDurationEntity = _fixture.Create<DatasetDuration>();

            // ACT
            await datasetDurationRepository.AddAsync(datasetDurationEntity);
            await _context.SaveChangesAsync();

            // ASSERT
            var datasetDurations = await datasetDurationRepository.ListAsync();
            var datasetDurationArray = datasetDurations as DatasetDuration[] ?? datasetDurations.ToArray();
            datasetDurationArray.Should().NotBeNull();
            datasetDurationArray.Length.Should().Be(4);
            datasetDurationArray.SingleOrDefault(c => c.DatasetId == datasetDurationEntity.DatasetId).Should().NotBeNull();
        }

       [Fact]
        public async Task Remove_ShouldBeRemoved()
        {
            // ARRANGE
            var datasetDurationRepository = new DatasetDurationRepository(_context);
            var theDatasetDurationToRemove = await datasetDurationRepository.FindByDatasetAndTypeAsync(_datasetDuration[0].DatasetId, _datasetDuration[0].DurationType);

            // ACT
            datasetDurationRepository.Remove(theDatasetDurationToRemove);
            await _context.SaveChangesAsync();

            // ASSERT
            var nonExistingDatasetDuration = await datasetDurationRepository.FindByDatasetAndTypeAsync(theDatasetDurationToRemove.DatasetId, theDatasetDurationToRemove.DurationType);
            nonExistingDatasetDuration.Should().BeNull();
        }
    }
}
