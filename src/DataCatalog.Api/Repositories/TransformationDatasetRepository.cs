using System;
using DataCatalog.Api.Data;
using DataCatalog.Api.Data.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataCatalog.Api.Enums;

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

        public async Task<TransformationDataset> FindByDatasetIdAndDirectionAsync(Guid datasetId, TransformationDirection direction)
        {
            return await _context.TransformationDatasets
                .Include(a => a.Transformation).ThenInclude(a => a.TransformationDatasets)
                .Where(a => a.DatasetId == datasetId && a.TransformationDirection == direction).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TransformationDataset>> FindAllTransformationDatasetsForDatasetIdAndDirectionAsync(Guid datasetId, TransformationDirection direction)
        {
            return await _context.TransformationDatasets
                .Include(a => a.Transformation)
                .Where(a => a.DatasetId == datasetId && a.TransformationDirection == direction).ToArrayAsync();
        }

        public async Task<IEnumerable<TransformationDataset>> FindAllByTransformationIdAndDirectionAsync(Guid transformationId, TransformationDirection direction)
        {
            return await _context.TransformationDatasets
                .Include(a => a.Dataset)
                .Where(a => a.TransformationId == transformationId && a.TransformationDirection == direction).ToArrayAsync();
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
            var entityToRemove = await _context.TransformationDatasets.SingleOrDefaultAsync(t => t.DatasetId == transformationDataset.DatasetId && t.TransformationId == transformationDataset.TransformationId && t.TransformationDirection == transformationDataset.TransformationDirection);
            if (entityToRemove != null)
                _context.TransformationDatasets.Remove(entityToRemove);
        }

        public void Remove(List<TransformationDataset> transformationDatasets)
        {
            Task.WaitAll(transformationDatasets.Select(RemoveAsync).ToArray());
        }
    }
}
