using DataCatalog.Api.Data.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Common.Enums;

namespace DataCatalog.Api.Services
{
    public interface IDatasetService
    {
        Task<Dataset> FindByIdAsync(Guid id);
        Task<IEnumerable<Dataset>> GetAllSummariesAsync();
        Task<Dataset> SaveAsync(DatasetCreateRequest datasetCreateRequest);
        Task<Dataset> UpdateAsync(DatasetUpdateRequest datasetUpdateRequest);
        Task DeleteAsync(Guid id);
        Task SoftDeleteAsync(Guid id);
        Task<IEnumerable<Dataset>> GetDatasetByCategoryAsync(Guid categoryId, SortType sortType, int take, int pageSize, int pageIndex);
        Task<IEnumerable<Dataset>> GetDatasetsBySearchTermAsync(string searchTerm, SortType sortType, int take, int pageSize, int pageIndex);
        Task<LineageDataset> GetDatasetLineageAsync(Guid id);
        Task InsertOrUpdateAvailability(DataAvailabilityInfoDto request);
    }
}
