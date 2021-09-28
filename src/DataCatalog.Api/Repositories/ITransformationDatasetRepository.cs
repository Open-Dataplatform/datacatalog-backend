using System;
using DataCatalog.Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Common.Enums;

namespace DataCatalog.Api.Repositories
{
    public interface ITransformationDatasetRepository
    {
        Task<IEnumerable<TransformationDataset>> ListAsync();
        Task<TransformationDataset> FindByDatasetIdAndDirectionAsync(Guid datasetId, TransformationDirection direction, Guid transformationId);
        Task<IEnumerable<TransformationDataset>> FindAllTransformationDatasetsForDatasetIdAndDirectionAsync(Guid datasetId, TransformationDirection direction);
        Task<IEnumerable<TransformationDataset>> FindAllByTransformationIdAndDirectionAsync(Guid transformationId, TransformationDirection direction);
        Task AddAsync(TransformationDataset transformationDataset);
        void Update(TransformationDataset transformationDataset);
        Task RemoveAsync(TransformationDataset transformationDataset);
    }
}
