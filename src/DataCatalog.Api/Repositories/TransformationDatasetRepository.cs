using System;

using DataCatalog.Data.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataCatalog.Common.Enums;
using DataCatalog.Data;

namespace DataCatalog.Api.Repositories
{
    public class TransformationDatasetRepository : BaseRepository, ITransformationDatasetRepository
    {
        public TransformationDatasetRepository(DataCatalogContext context) : base(context)
        { }

        public async Task<IEnumerable<TransformationDataset>> ListAsync()
        {
            return await _context.TransformationDatasets.ToListAsync();
        }

        public async Task<TransformationDataset> FindByDatasetIdAndDirectionAsync(Guid datasetId, TransformationDirection direction, Guid transformationId)
        {
            return await _context.TransformationDatasets
                .Include(transformationDataset => transformationDataset.Transformation).ThenInclude(a => a.TransformationDatasets)
                .Include(transformationDataset => transformationDataset.Dataset)
                .Where(transformationDataset => !transformationDataset.Dataset.IsDeleted)
                .Where(transformationDataset => 
                    transformationDataset.DatasetId == datasetId 
                    && transformationDataset.TransformationDirection == direction
                    && transformationDataset.TransformationId == transformationId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TransformationDataset>> FindAllTransformationDatasetsForDatasetIdAndDirectionAsync(Guid datasetId, TransformationDirection direction)
        {
            return await _context.TransformationDatasets
                .Include(transformationDataset => transformationDataset.Transformation)
                .Include(transformationDataset => transformationDataset.Dataset)
                .Where(transformationDataset => !transformationDataset.Dataset.IsDeleted)
                .Where(transformationDataset => transformationDataset.DatasetId == datasetId && transformationDataset.TransformationDirection == direction).ToArrayAsync();
        }

        public async Task<IEnumerable<TransformationDataset>> FindAllByTransformationIdAndDirectionAsync(Guid transformationId, TransformationDirection direction)
        {
            return await _context.TransformationDatasets
                .Include(transformationDataset => transformationDataset.Dataset)
                .Where(transformationDataset => !transformationDataset.Dataset.IsDeleted)
                .Where(transformationDataset => transformationDataset.TransformationId == transformationId && transformationDataset.TransformationDirection == direction).ToArrayAsync();
        }

        public async Task AddAsync(TransformationDataset transformationDataset)
        {
            await _context.TransformationDatasets.AddAsync(transformationDataset);
        }

        public void Update(TransformationDataset transformationDataset)
        {
            _context.TransformationDatasets.Update(transformationDataset);
        }

        public async Task RemoveAsync(TransformationDataset transformationDataset)
        {
            var entityToRemove = await _context.TransformationDatasets.SingleOrDefaultAsync(t =>
                t.DatasetId == transformationDataset.DatasetId &&
                t.TransformationId == transformationDataset.TransformationId &&
                t.TransformationDirection == transformationDataset.TransformationDirection);
            if (entityToRemove != null)
                _context.TransformationDatasets.Remove(entityToRemove);
        }
    }
}
