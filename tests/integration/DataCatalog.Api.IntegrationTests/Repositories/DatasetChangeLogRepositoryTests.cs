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
    public class DatasetChangeLogRepositoryTests : BaseTest, IDisposable
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly DataCatalogContext _context;
        private readonly List<DatasetChangeLog> _datasetChangeLogs;

        public DatasetChangeLogRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataCatalogContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new DataCatalogContext(options);

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _datasetChangeLogs = _fixture.Create<IEnumerable<DatasetChangeLog>>().ToList();
            _datasetChangeLogs.ForEach(c => _context.DatasetChangeLogs.Add(c));
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
            var datasetChangeLogRepository = new DatasetChangeLogRepository(_context);

            // ACT
            var datasetChangeLogs = await datasetChangeLogRepository.ListAsync();

            // ASSERT
            var datasetChangeLogArray = datasetChangeLogs as DatasetChangeLog[] ?? datasetChangeLogs.ToArray();
            datasetChangeLogArray.Should().NotBeNull();
            datasetChangeLogArray.Length.Should().Be(3);
            datasetChangeLogArray.Count(c => c.Id == _datasetChangeLogs[0].Id).Should().Be(1);
            datasetChangeLogArray.Count(c => c.Id == _datasetChangeLogs[1].Id).Should().Be(1);
            datasetChangeLogArray.Count(c => c.Id == _datasetChangeLogs[2].Id).Should().Be(1);
        }

        [Fact]
        public async Task FindByIdAsync_InvalidId_ShouldReturnNull()
        {
            // ARRANGE
            var datasetChangeLogRepository = new DatasetChangeLogRepository(_context);
            var invalidId = Guid.NewGuid();

            // ACT
            var datasetChangeLog = await datasetChangeLogRepository.FindByIdAsync(invalidId);

            // ASSERT
            datasetChangeLog.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_ValidId_ShouldReturnDatasetChangeLog()
        {
            // ARRANGE
            var datasetChangeLogRepository = new DatasetChangeLogRepository(_context);

            // ACT
            var datasetChangeLog = await datasetChangeLogRepository.FindByIdAsync(_datasetChangeLogs[1].Id);

            // ASSERT
            datasetChangeLog.Should().NotBeNull();
            datasetChangeLog.Id.Should().Be(_datasetChangeLogs[1].Id);
        }

        [Fact]
        public async Task AddAsync_ShouldAddDatasetChangeLog()
        {
            // ARRANGE
            var datasetChangeLogRepository = new DatasetChangeLogRepository(_context);
            var datasetChangeLogEntity = _fixture.Create<DatasetChangeLog>();

            // ACT
            await datasetChangeLogRepository.AddAsync(datasetChangeLogEntity);
            await _context.SaveChangesAsync();

            // ASSERT
            var datasetChangeLogs = await datasetChangeLogRepository.ListAsync();
            var datasetChangeLogArray = datasetChangeLogs as DatasetChangeLog[] ?? datasetChangeLogs.ToArray();
            datasetChangeLogArray.Should().NotBeNull();
            datasetChangeLogArray.Length.Should().Be(4);
            datasetChangeLogArray.SingleOrDefault(c => c.Id == datasetChangeLogEntity.Id).Should().NotBeNull();
        }

        [Fact]
        public async Task Update_ChangeProperties_PropertiesShouldBeChanged()
        {
            // ARRANGE
            var datasetChangeLogRepository = new DatasetChangeLogRepository(_context);
            var datasetChangeLogToUpdate = _datasetChangeLogs[2];
            var newDatasetChangeLogValues = _fixture.Create<DatasetChangeLog>();
            datasetChangeLogToUpdate.DatasetId = newDatasetChangeLogValues.DatasetId;
            datasetChangeLogToUpdate.MemberId = newDatasetChangeLogValues.MemberId;
            datasetChangeLogToUpdate.Name = newDatasetChangeLogValues.Name;
            datasetChangeLogToUpdate.Email = newDatasetChangeLogValues.Email;

            // ACT
            datasetChangeLogRepository.Update(datasetChangeLogToUpdate);
            await _context.SaveChangesAsync();

            // ASSERT
            var updatedDatasetChangeLog = await datasetChangeLogRepository.FindByIdAsync(datasetChangeLogToUpdate.Id);
            updatedDatasetChangeLog.Should().NotBeNull();
            updatedDatasetChangeLog.DatasetId.Should().Be(newDatasetChangeLogValues.DatasetId);
            updatedDatasetChangeLog.MemberId.Should().Be(newDatasetChangeLogValues.MemberId);
            updatedDatasetChangeLog.Name.Should().Be(newDatasetChangeLogValues.Name);
            updatedDatasetChangeLog.Email.Should().Be(newDatasetChangeLogValues.Email);
        }

        [Fact]
        public async Task Remove_ShouldBeRemoved()
        {
            // ARRANGE
            var datasetChangeLogRepository = new DatasetChangeLogRepository(_context);
            var theDatasetChangeLogToRemove = await datasetChangeLogRepository.FindByIdAsync(_datasetChangeLogs[0].Id);

            // ACT
            datasetChangeLogRepository.Remove(theDatasetChangeLogToRemove);
            await _context.SaveChangesAsync();

            // ASSERT
            var nonExistingDatasetChangeLog = await datasetChangeLogRepository.FindByIdAsync(theDatasetChangeLogToRemove.Id);
            nonExistingDatasetChangeLog.Should().BeNull();
        }
    }
}
