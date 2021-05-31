using DataCatalog.Api.Data.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Common.Enums;

namespace DataCatalog.Api.Services
{
    public interface IDatasetService : IHandleMessageBusMessage
    {
        Task<Dataset> FindByIdAsync(Guid id);
        Task<IEnumerable<Dataset>> GetAllSummariesAsync(bool onlyPublished);
        Task<Dataset> SaveAsync(DatasetCreateRequest datasetCreateRequest);
        Task<Dataset> UpdateAsync(DatasetUpdateRequest datasetUpdateRequest);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Dataset>> GetDatasetByCategoryAsync(Guid categoryId, SortType sortType, int take, int pageSize, int pageIndex, bool filterUnpublished);
        Task<IEnumerable<Dataset>> GetDatasetsBySearchTermAsync(string searchTerm, SortType sortType, int take, int pageSize, int pageIndex, bool filterUnpublished);
        Task<string> GetDatasetLocationAsync(Guid? hierarchyId, string name);
        Task<LineageDataset> GetDatasetLineageAsync(Guid id);
        Task<Dataset> CopyDatasetInRawAsync(Guid id);
    }
}
