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
    public class DataFieldRepositoryTests : BaseTest, IDisposable
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly DataCatalogContext _context;
        private readonly List<DataField> _dataFields;

        public DataFieldRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataCatalogContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new DataCatalogContext(options);

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _dataFields = _fixture.Create<IEnumerable<DataField>>().ToList();
            _dataFields.ForEach(c => _context.DataFields.Add(c));
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
            var dataFieldRepository = new DataFieldRepository(_context);

            // ACT
            var dataFields = await dataFieldRepository.ListAsync();

            // ASSERT
            var dataFieldArray = dataFields as DataField[] ?? dataFields.ToArray();
            dataFieldArray.Should().NotBeNull();
            dataFieldArray.Length.Should().Be(3);
            dataFieldArray.Count(c => c.Id == _dataFields[0].Id).Should().Be(1);
            dataFieldArray.Count(c => c.Id == _dataFields[1].Id).Should().Be(1);
            dataFieldArray.Count(c => c.Id == _dataFields[2].Id).Should().Be(1);
        }

        [Fact]
        public async Task FindByIdAsync_InvalidId_ShouldReturnNull()
        {
            // ARRANGE
            var dataFieldRepository = new DataFieldRepository(_context);
            var invalidId = Guid.NewGuid();

            // ACT
            var dataField = await dataFieldRepository.FindByIdAsync(invalidId);

            // ASSERT
            dataField.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_ValidId_ShouldReturnDataField()
        {
            // ARRANGE
            var dataFieldRepository = new DataFieldRepository(_context);

            // ACT
            var dataField = await dataFieldRepository.FindByIdAsync(_dataFields[1].Id);

            // ASSERT
            dataField.Should().NotBeNull();
            dataField.Id.Should().Be(_dataFields[1].Id);
        }

        [Fact]
        public async Task AddAsync_ShouldAddDataField()
        {
            // ARRANGE
            var dataFieldRepository = new DataFieldRepository(_context);
            var dataFieldEntity = _fixture.Create<DataField>();

            // ACT
            await dataFieldRepository.AddAsync(dataFieldEntity);
            await _context.SaveChangesAsync();

            // ASSERT
            var dataFields = await dataFieldRepository.ListAsync();
            var dataFieldArray = dataFields as DataField[] ?? dataFields.ToArray();
            dataFieldArray.Should().NotBeNull();
            dataFieldArray.Length.Should().Be(4);
            dataFieldArray.SingleOrDefault(c => c.Id == dataFieldEntity.Id).Should().NotBeNull();
        }

        [Fact]
        public async Task Update_ChangeProperties_PropertiesShouldBeChanged()
        {
            // ARRANGE
            var dataFieldRepository = new DataFieldRepository(_context);
            var dataFieldToUpdate = _dataFields[2];
            var newDataFieldValues = _fixture.Create<DataField>();
            dataFieldToUpdate.Name = newDataFieldValues.Name;
            dataFieldToUpdate.Type = newDataFieldValues.Type;
            dataFieldToUpdate.Description = newDataFieldValues.Description;
            dataFieldToUpdate.Format = newDataFieldValues.Format;
            dataFieldToUpdate.Validation = newDataFieldValues.Validation;
            dataFieldToUpdate.DatasetId = newDataFieldValues.DatasetId;

            // ACT
            dataFieldRepository.Update(dataFieldToUpdate);
            await _context.SaveChangesAsync();

            // ASSERT
            var updatedDataField = await dataFieldRepository.FindByIdAsync(dataFieldToUpdate.Id);
            updatedDataField.Should().NotBeNull();
            updatedDataField.Name.Should().Be(newDataFieldValues.Name);
            updatedDataField.Type.Should().Be(newDataFieldValues.Type);
            updatedDataField.Description.Should().Be(newDataFieldValues.Description);
            updatedDataField.Format.Should().Be(newDataFieldValues.Format);
            updatedDataField.Validation.Should().Be(newDataFieldValues.Validation);
            updatedDataField.DatasetId.Should().Be(newDataFieldValues.DatasetId);
        }

        [Fact]
        public async Task Remove_ShouldBeRemoved()
        {
            // ARRANGE
            var dataFieldRepository = new DataFieldRepository(_context);
            var theDataFieldToRemove = await dataFieldRepository.FindByIdAsync(_dataFields[0].Id);

            // ACT
            dataFieldRepository.Remove(theDataFieldToRemove);
            await _context.SaveChangesAsync();

            // ASSERT
            var nonExistingDataField = await dataFieldRepository.FindByIdAsync(theDataFieldToRemove.Id);
            nonExistingDataField.Should().BeNull();
        }
    }
}
