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
using DataCatalog.Api.Services;

namespace DataCatalog.Api.IntegrationTests.Repositories
{
    public class DatasetRepositoryTests : BaseTest, IDisposable
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly DataCatalogContext _context;
        private readonly List<Dataset> _datasets;
        private readonly Category _commonCategory;
        private readonly PermissionUtils _adminPermissionUtils;
        private readonly PermissionUtils _dataStewardPermissionUtils;
        private readonly PermissionUtils _userPermissionUtils;
        private const int DatasetSize = 12;

        public DatasetRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataCatalogContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new DataCatalogContext(options);

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _fixture.RepeatCount = DatasetSize;

            _datasets = _fixture.Create<IEnumerable<Dataset>>().ToList();
            _datasets.ForEach(d => d.IsDeleted = false);
            _datasets.Take(4).ToList().ForEach(d => d.Status = DatasetStatus.Draft);
            _datasets.Skip(4).Take(4).ToList().ForEach(d => d.Status = DatasetStatus.Published);
            _datasets.Skip(8).Take(4).ToList().ForEach(d => d.Status = DatasetStatus.Source);
            var date = new DateTime(2020, 11, 20);
            _datasets.Take(4).ToList().ForEach(d => { d.CreatedDate = date; date = date.AddYears(-1); });
            date = new DateTime(2017, 11, 20);
            _datasets.Take(4).ToList().ForEach(d => { d.CreatedDate = date; date = date.AddYears(1); });
            date = new DateTime(2020, 11, 20);
            _datasets.Take(4).ToList().ForEach(d => { d.CreatedDate = date; date = date.AddYears(-1); });
            date = new DateTime(2017, 11, 20);
            _datasets.Take(4).ToList().ForEach(d => { d.ModifiedDate = date; date = date.AddYears(1); });
            date = new DateTime(2020, 11, 20);
            _datasets.Take(4).ToList().ForEach(d => { d.ModifiedDate = date; date = date.AddYears(-1); });
            date = new DateTime(2017, 11, 20);
            _datasets.Take(4).ToList().ForEach(d => { d.ModifiedDate = date; date = date.AddYears(1); });
            _datasets[0].Name = "Michael";
            _datasets[1].Name = "Gabriel";
            _datasets[2].Name = "Rafael";
            _datasets[3].Name = "Uriel";
            _datasets[4].Name = "Raguel";
            _datasets[5].Name = "Zerakiel";
            _datasets[6].Name = "Remiel";
            _datasets[7].Name = "Adam";
            _datasets[8].Name = "Eva";
            _datasets[9].Name = "Alexander";
            _datasets[10].Name = "Abel";
            _datasets[11].Name = "Kain";
            _commonCategory = _datasets[0].DatasetCategories[0].Category;
            _context.Categories.Add(_commonCategory);
            _context.SaveChanges();
            _datasets.ForEach(d => d.DatasetCategories = new List<DatasetCategory> { new DatasetCategory { Category = _commonCategory, Dataset = d} });
            _datasets.ForEach(c => _context.Datasets.Add(c));
            _context.SaveChanges();

            var adminCurrent = new Current
            {
                MemberId = Guid.NewGuid(),
                Roles = new List<Role>
                {
                    Role.Admin
                }
            };
            _adminPermissionUtils = new PermissionUtils(adminCurrent);
            
            var dataStewardCurrent = new Current
            {
                MemberId = Guid.NewGuid(),
                Roles = new List<Role>
                {
                    Role.DataSteward
                }
            };
            _dataStewardPermissionUtils = new PermissionUtils(dataStewardCurrent);
            
            var userCurrent = new Current
            {
                MemberId = Guid.NewGuid(),
                Roles = new List<Role>
                {
                    Role.User
                }
            };
            _userPermissionUtils = new PermissionUtils(userCurrent);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task FindByIdAsync_InvalidId_ShouldReturnNull()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _adminPermissionUtils);
            var invalidId = Guid.NewGuid();

            // ACT
            var datasets = await datasetRepository.FindByIdAsync(invalidId);

            // ASSERT
            datasets.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_ValidId_ShouldReturnDataset()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _adminPermissionUtils);
            var dataset = _datasets[0];

            // ACT
            var result = await datasetRepository.FindByIdAsync(dataset.Id);

            // ASSERT
            result.Should().NotBeNull();
            result.Name.Should().Be(dataset.Name);
            result.Description.Should().Be(dataset.Description);
            result.SlaDescription.Should().Be(dataset.SlaDescription);
            result.SlaLink.Should().Be(dataset.SlaLink);
            result.Owner.Should().Be(dataset.Owner);
            result.Status.Should().Be(dataset.Status);
            result.Confidentiality.Should().Be(dataset.Confidentiality);
            result.SourceId.Should().Be(dataset.SourceId);
            result.DataFields.Should().NotBeNull();
            result.DataFields.Count.Should().Be(dataset.DataFields.Count);
            result.DatasetCategories.Count.Should().Be(dataset.DatasetCategories.Count);
            result.TransformationDatasets.Count.Should().Be(dataset.TransformationDatasets.Count);
            result.DatasetChangeLogs.Count.Should().Be(dataset.DatasetChangeLogs.Count);
            result.DatasetDurations.Count.Should().Be(dataset.DatasetDurations.Count);
            result.DataContracts.Count.Should().Be(dataset.DataContracts.Count);
        }

        [Fact]
        public async Task ListSummariesAsync_All_ShouldReturnAllDatasets()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _adminPermissionUtils);

            // ACT
            var result = await datasetRepository.ListSummariesAsync();

            // ASSERT
            var enumerable = result as Dataset[] ?? result.ToArray();
            enumerable.Should().NotBeNull();
            enumerable.Length.Should().Be(12);
        }

        [Fact]
        public async Task ListSummariesAsync_OnlyPublished_ShouldReturnAllDatasets()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _userPermissionUtils);

            // ACT
            var result = await datasetRepository.ListSummariesAsync();

            // ASSERT
            var enumerable = result as Dataset[] ?? result.ToArray();
            enumerable.Should().NotBeNull();
            foreach (var dataset in enumerable)
            {
                dataset.Status.Should().Be(DatasetStatus.Published);
            }
            enumerable.Length.Should().Be(4);
        }

        [Fact]
        public async Task GetDatasetByCategoryAsync_SortByNameAsc_ShouldReturnSortedDatasets()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _adminPermissionUtils);

            // ACT
            var result = await datasetRepository.GetDatasetByCategoryAsync(_commonCategory.Id, SortType.ByNameAscending, 0, 0, 0);

            // ASSERT
            var enumerable = result as Dataset[] ?? result.ToArray();
            enumerable.Should().NotBeNull();
            var ordered = _datasets.OrderBy(d => d.Name).ToList();
            for (var i = 0; i < 12; i++)
                enumerable[i].Id.Should().Be(ordered[i].Id);
        }

        [Fact]
        public async Task GetDatasetByCategoryAsync_SortByNameDesc_ShouldReturnSortedDatasets()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _adminPermissionUtils);

            // ACT
            var result = await datasetRepository.GetDatasetByCategoryAsync(_commonCategory.Id, SortType.ByNameDescending, 0, 0, 0);

            // ASSERT
            var enumerable = result as Dataset[] ?? result.ToArray();
            enumerable.Should().NotBeNull();
            var ordered = _datasets.OrderByDescending(d => d.Name).ToList();
            for (var i = 0; i < 12; i++)
                enumerable[i].Id.Should().Be(ordered[i].Id);
        }

        [Fact]
        public async Task GetDatasetByCategoryAsync_SortByCreatedDateAsc_ShouldReturnSortedDatasets()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _adminPermissionUtils);

            // ACT
            var result = await datasetRepository.GetDatasetByCategoryAsync(_commonCategory.Id, SortType.ByCreatedDateAscending, 0, 0, 0);

            // ASSERT
            var enumerable = result as Dataset[] ?? result.ToArray();
            enumerable.Should().NotBeNull();
            var ordered = _datasets.OrderBy(d => d.CreatedDate).ToList();
            for (var i = 0; i < 12; i++)
                enumerable[i].Id.Should().Be(ordered[i].Id);
        }

        [Fact]
        public async Task GetDatasetByCategoryAsync_SortByCreatedDateDesc_ShouldReturnSortedDatasets()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _adminPermissionUtils);

            // ACT
            var result = await datasetRepository.GetDatasetByCategoryAsync(_commonCategory.Id, SortType.ByCreatedDateDescending, 0, 0, 0);

            // ASSERT
            var enumerable = result as Dataset[] ?? result.ToArray();
            enumerable.Should().NotBeNull();
            var ordered = _datasets.OrderByDescending(d => d.CreatedDate).ToList();
            for (var i = 0; i < 12; i++)
                enumerable[i].Id.Should().Be(ordered[i].Id);
        }

        [Fact]
        public async Task GetDatasetByCategoryAsync_SortByModifiedDateAscending_ShouldReturnSortedDatasets()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _adminPermissionUtils);

            // ACT
            var result = await datasetRepository.GetDatasetByCategoryAsync(_commonCategory.Id, SortType.ByModifiedDateAscending, 0, 0, 0);

            // ASSERT
            var enumerable = result as Dataset[] ?? result.ToArray();
            enumerable.Should().NotBeNull();
            var ordered = _datasets.OrderBy(d => d.ModifiedDate).ToList();
            for (var i = 0; i < 12; i++)
                enumerable[i].Id.Should().Be(ordered[i].Id);
        }

        [Fact]
        public async Task GetDatasetByCategoryAsync_SortByModifiedDateDescending_ShouldReturnSortedDatasets()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _adminPermissionUtils);

            // ACT
            var result = await datasetRepository.GetDatasetByCategoryAsync(_commonCategory.Id, SortType.ByModifiedDateDescending, 0, 0, 0);

            // ASSERT
            var enumerable = result as Dataset[] ?? result.ToArray();
            enumerable.Should().NotBeNull();
            var ordered = _datasets.OrderByDescending(d => d.ModifiedDate).ToList();
            for (int i = 0; i < 12; i++)
                enumerable[i].Id.Should().Be(ordered[i].Id);
        }

        [Fact]
        public async Task GetDatasetByCategoryAsync_UsePageSize_ShouldReturnSortedDatasets()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _adminPermissionUtils);
            var pageSize = 2;
            var pageIndex = 1;

            // ACT
            var result = await datasetRepository.GetDatasetByCategoryAsync(_commonCategory.Id, SortType.ByNameAscending, 0, pageSize, pageIndex);

            // ASSERT
            var enumerable = result as Dataset[] ?? result.ToArray();
            enumerable.Should().NotBeNull();
            enumerable.Length.Should().Be(2);
            var ordered = _datasets.OrderBy(d => d.Name).Skip(pageIndex * pageSize).ToList();
            ordered[0].Id.Should().Be(enumerable[0].Id);
            ordered[1].Id.Should().Be(enumerable[1].Id);
        }

        [Fact]
        public async Task GetDatasetByCategoryAsync_UseTake_ShouldReturnSortedDatasets()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _adminPermissionUtils);
            var take = 5;

            // ACT
            var result = await datasetRepository.GetDatasetByCategoryAsync(_commonCategory.Id, SortType.ByNameAscending, take, 0, 0);

            // ASSERT
            var enumerable = result as Dataset[] ?? result.ToArray();
            enumerable.Should().NotBeNull();
            enumerable.Length.Should().Be(take);
            var ordered = _datasets.OrderBy(d => d.Name).Take(take).ToList();
            for (var i = 0; i < take; i++)
                ordered[i].Id.Should().Be(enumerable[i].Id);
        }

        [Fact]
        public async Task GetDatasetsBySearchTermQuery_SearchDraft_ShouldReturnDraftDatasets()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _adminPermissionUtils);

            // ACT
            var result = await datasetRepository.GetDatasetsBySearchTermQueryAsync(" drAft  ", SortType.ByNameAscending, 0, 0, 0);

            // ASSERT
            var enumerable = result as Dataset[] ?? result.ToArray();
            enumerable.Should().NotBeNull();
            enumerable.ToList().ForEach(d => d.Status.Should().Be(DatasetStatus.Draft));
        }

        [Fact]
        public async Task GetDatasetsBySearchTermQuery_SearchPublished_ShouldReturnPublishedDatasets()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _adminPermissionUtils);

            // ACT
            var result = await datasetRepository.GetDatasetsBySearchTermQueryAsync("published  ", SortType.ByNameAscending, 0, 0, 0);

            // ASSERT
            var enumerable = result as Dataset[] ?? result.ToArray();
            enumerable.Should().NotBeNull();
            enumerable.ToList().ForEach(d => d.Status.Should().Be(DatasetStatus.Published));
        }

        [Fact]
        public async Task GetDatasetsBySearchTermQuery_SearchSourceTerm_ShouldReturnSourceDatasets()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _dataStewardPermissionUtils);

            // ACT
            var result = await datasetRepository.GetDatasetsBySearchTermQueryAsync("sourcE", SortType.ByNameAscending, 0, 0, 0);

            // ASSERT
            var enumerable = result as Dataset[] ?? result.ToArray();
            enumerable.Should().NotBeNull();
            enumerable.Length.Should().Be(4);
        }

        [Fact]
        public async Task GetDatasetsBySearchTermQuery_SearchOtherTerm()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _adminPermissionUtils);

            // ACT
            var result = await datasetRepository.GetDatasetsBySearchTermQueryAsync("el", SortType.ByNameAscending, 0, 0, 0);

            // ASSERT
            var enumerable = result as Dataset[] ?? result.ToArray();
            enumerable.Should().NotBeNull();
            enumerable.Length.Should().Be(8);
        }
        
        [Fact]
        public async Task GetDatasetsBySearchTermQuery_SearchSourceTerm_UserAccess_ShouldNotReturnSourceDatasets()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _userPermissionUtils);

            // ACT
            var result = await datasetRepository.GetDatasetsBySearchTermQueryAsync("sourcE", SortType.ByNameAscending, 0, 0, 0);

            // ASSERT
            var enumerable = result as Dataset[] ?? result.ToArray();
            enumerable.Should().NotBeNull();
            enumerable.Length.Should().Be(0);
        }

        [Fact]
        public async Task AddAsync_ShouldAddDataset()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _adminPermissionUtils);
            var datasetEntity = _fixture.Create<Dataset>();
            datasetEntity.IsDeleted = false;
            
            // ACT
            await datasetRepository.AddAsync(datasetEntity);
            await _context.SaveChangesAsync();

            // ASSERT
            var datasets = await datasetRepository.ListSummariesAsync();
            var datasetArray = datasets as Dataset[] ?? datasets.ToArray();
            datasetArray.Should().NotBeNull();
            datasetArray.Length.Should().Be(DatasetSize + 1);
            datasetArray.SingleOrDefault(c => c.Id == datasetEntity.Id).Should().NotBeNull();
        }
        
        [Fact]
        public async Task AddAsync_AddDeletedDatasetShouldNotIncreaseListSize()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _adminPermissionUtils);
            var datasetEntity = _fixture.Create<Dataset>();
            datasetEntity.IsDeleted = true;
            
            // ACT
            await datasetRepository.AddAsync(datasetEntity);
            await _context.SaveChangesAsync();

            // ASSERT
            var datasets = await datasetRepository.ListSummariesAsync();
            var datasetArray = datasets as Dataset[] ?? datasets.ToArray();
            datasetArray.Should().NotBeNull();
            datasetArray.Length.Should().Be(DatasetSize);
            datasetArray.SingleOrDefault(c => c.Id == datasetEntity.Id).Should().BeNull();
        }

        [Fact]
        public async Task Remove_ShouldBeRemoved()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _adminPermissionUtils);
            var theDatasetToRemove = await datasetRepository.FindByIdAsync(_datasets[0].Id);

            // ACT
            datasetRepository.Remove(theDatasetToRemove);
            await _context.SaveChangesAsync();

            // ASSERT
            var nonExistingDataset = await datasetRepository.FindByIdAsync(theDatasetToRemove.Id);
            nonExistingDataset.Should().BeNull();
        }

        [Fact]
        public async Task UpdateProvisioningStatusAsync_ShouldChangeStatus()
        {
            // ARRANGE
            var datasetRepository = new DatasetRepository(_context, _adminPermissionUtils);
            _datasets[0].ProvisionStatus = ProvisionDatasetStatusEnum.Pending;
            var datasetToChange = await datasetRepository.FindByIdAsync(_datasets[0].Id);

            // ACT
            await datasetRepository.UpdateProvisioningStatusAsync(_datasets[0].Id, ProvisionDatasetStatusEnum.Succeeded);
            await _context.SaveChangesAsync();

            // ASSERT
            var changedDataset = await datasetRepository.FindByIdAsync(_datasets[0].Id);
            changedDataset.Should().NotBeNull();
            changedDataset.ProvisionStatus.Should().Be(ProvisionDatasetStatusEnum.Succeeded);
        }
    }
}
