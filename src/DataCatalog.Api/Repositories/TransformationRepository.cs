using DataCatalog.Api.Data;
using DataCatalog.Api.Data.Model;
using DataCatalog.Api.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public class TransformationRepository : BaseRepository, ITransformationRepository
    {
        public TransformationRepository(DataCatalogContext context) : base(context)
        { }

        public async Task<IEnumerable<Transformation>> ListAsync()
        {
            return await _context.Transformations
                .Include(a => a.TransformationDatasets)
                .ThenInclude(a => a.Dataset)
                .ToListAsync();
        }
        public async Task AddAsync(Transformation transformation)
        {
            await _context.Transformations.AddAsync(transformation);
        }

        public async Task<Transformation> FindByIdAsync(Guid id)
        {
            return await _context.Transformations
                .Include(a => a.TransformationDatasets)
                .ThenInclude(a => a.Dataset)
                .SingleOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Transformation>> GetByDatasetIdsAsync(Guid[] datasetIds)
        {
            var ids = datasetIds.ToArray();

            var preQuery = await (
                from t in _context.Transformations
                let tds = t.TransformationDatasets.Where(td =>
                    td.TransformationDirection == TransformationDirection.Source
                    && ids.Contains(td.DatasetId)).Select(a => a.DatasetId)
                where tds.Any()
                select new { t.Id, tds }).ToArrayAsync();

            var tIds = (
                from t in preQuery
                where !t.tds.Except(ids).Union(ids.Except(t.tds)).Any()
                select t.Id).ToArray();

            return await _context.Transformations
                .Include(a => a.TransformationDatasets)
                .ThenInclude(a => a.Dataset)
                .Where(a => tIds.Contains(a.Id))
                .ToListAsync();
        }

        public void Update(Transformation transformation)
        {
            _context.Transformations.Update(transformation);
        }

        public async Task RemoveAsync(Transformation transformation)
        {
            var entityToRemove = await _context.Transformations.SingleOrDefaultAsync(t => t.Id == transformation.Id);
            if (entityToRemove != null)
                _context.Transformations.Remove(entityToRemove);
        }
    }
}
