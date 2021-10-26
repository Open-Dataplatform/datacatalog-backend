
using DataCatalog.Data.Model;
using DataCatalog.Common.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataCatalog.Data;
using DataCatalog.Api.Services;
using Microsoft.Extensions.Logging;

namespace DataCatalog.Api.Repositories
{
    public class DatasetRepository : BaseRepository, IDatasetRepository
    {
        private readonly IPermissionUtils _permissionUtils;
        private readonly ILogger<DatasetRepository> _logger;
        public DatasetRepository(DataCatalogContext context, IPermissionUtils permissionUtils, ILogger<DatasetRepository> logger) : base(context)
        {
            _permissionUtils = permissionUtils;
            _logger = logger;
        }

        public async Task<Dataset> FindByIdAsync(Guid id)
        {
            var dataset = GetIncludeQueryable()
                .Include(a => a.DataFields.OrderBy(df => df.SortingKey))
                .Include(a => a.DatasetDurations).ThenInclude(a => a.Duration)
                .Include(a => a.DatasetChangeLogs).ThenInclude(a => a.Member)
                .Include(a => a.DatasetChangeLogs).ThenInclude(a => a.DatasetPermissionChange)
                .Include(a => a.DataContracts).ThenInclude(a => a.DataSource)
                .Include(a => a.ServiceLevelAgreement)
                .AsSplitQuery()
                .FirstOrDefault(a => a.Id == id);
          
            //Only load source transformation and related datasets
            await _context.TransformationDatasets.Include(a => a.Transformation)
                .ThenInclude(a => a.TransformationDatasets).ThenInclude(a => a.Dataset)
                .Where(a => a.DatasetId == id && a.TransformationDirection == TransformationDirection.Sink)
                .LoadAsync();

            return dataset;
        }

        public async Task<IEnumerable<Dataset>> ListAllNonDeletedDatasets()
        {
            return await _context.Datasets
                .Where(d => !d.IsDeleted)
                .ToArrayAsync();
        }

        public async Task<IEnumerable<Dataset>> ListSummariesAsync()
        {
            return await GetIncludeQueryable().ToArrayAsync();
        }

        public async Task<IEnumerable<Dataset>> GetDatasetByCategoryAsync(Guid categoryId, SortType sortType, int take, int pageSize, int pageIndex)
        {
            var query = GetIncludeQueryable().Where(a => a.DatasetCategories.Any(b => b.CategoryId == categoryId));
            
            query = GetOrderedAndChunkedQuery(query, sortType, take, pageSize, pageIndex);

            return await query.ToArrayAsync();
        }

        public async Task<IEnumerable<Dataset>> GetDatasetsBySearchTermQueryAsync(string searchTerm, SortType sortType, int take, int pageSize, int pageIndex)
        {
            var query = GetIncludeQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = Regex.Replace(searchTerm, @"\s+", " ").ToLower(); //Replace multiple whitespace characters with single space
                switch (term)
                {
                    case "draft":
                        query = query.Where(a => a.Status == DatasetStatus.Draft);
                        break;
                    case "published":
                        query = query.Where(a => a.Status == DatasetStatus.Published);
                        break;
                    case "source":
                        query = query.Where(a => a.Status == DatasetStatus.Source);
                        break;
                    case "developing":
                        query = query.Where(a => a.Status == DatasetStatus.Developing);
                        break;
                    default:
                    {
                        if (Guid.TryParse(term, out var guid))
                        {
                            query = query.Where(a => a.Id == guid);
                            break;
                        }
                        var terms = term.Split(' ').Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();

                        var unionQuery = GetSearchTermQuery(query, terms[0]);
                        for (int i = 1; i < terms.Length; i++)
                            unionQuery = unionQuery.Union(GetSearchTermQuery(query, terms[i]));
                        query = unionQuery;
                        break;
                    }
                }
            }

            query = GetOrderedAndChunkedQuery(query, sortType, take, pageSize, pageIndex);

            return await query.ToArrayAsync();
        }

        public async Task AddAsync(Dataset dataset)
        {
            await _context.Datasets.AddAsync(dataset);
        }

        public void Remove(Dataset dataset)
        {
            _context.Datasets.Remove(dataset);
        }

        public async Task UpdateProvisioningStatusAsync(Guid id, ProvisionDatasetStatusEnum status)
        {
            var dataset = await _context.Datasets.SingleAsync(d => d.Id == id);
            dataset.ProvisionStatus = status;
            dataset.ModifiedDate = DateTime.UtcNow;
            _logger.LogInformation("Dataset with Id {DatasetId} was updated with provisioning status {ProvisioningStatus}", id, status);
        }

        private IQueryable<Dataset> GetIncludeQueryable()
        {
            var query = _context.Datasets.AsQueryable();

            if (_permissionUtils.FilterUnpublishedDatasets)
            { 
                query = query.Where(a => a.Status == DatasetStatus.Published || a.Status == DatasetStatus.Developing);
            }

            query = query.Where(dataset => !dataset.IsDeleted);
            query = query.Include(a => a.DatasetCategories).ThenInclude(a => a.Category);

            return query;
        }

        private static IQueryable<Dataset> GetSearchTermQuery(IQueryable<Dataset> query, string t)
        {
            return from ds in query
                   where ds.Name.Contains(t)
                         || t == "draft" && ds.Status == DatasetStatus.Draft
                         || t == "published" && ds.Status == DatasetStatus.Published
                         || t == "source" && ds.Status == DatasetStatus.Source
                         || t == "developing" && ds.Status == DatasetStatus.Developing
                         || ds.Description != null && ds.Description.Contains(t)
                         || ds.DatasetCategories.Any(b => b.Category.Name.Contains(t))
                         || ds.DataContracts.Any(b => b.DataSource.Name.Contains(t) || b.DataSource.Description != null && b.DataSource.Description.Contains(t))
                         || ds.DataFields.Any(b => b.Name.Contains(t) || (b.Description != null && b.Description.Contains(t)))
                   select ds;
        }

        private static IQueryable<Dataset> GetOrderedAndChunkedQuery(IQueryable<Dataset> query, SortType sortType, int take, int pageSize, int pageIndex)
        {
            var returnQuery = query.OrderByDescending(a => a.CreatedDate).AsQueryable();
            if (sortType == SortType.ByNameAscending) returnQuery = query.OrderBy(a => a.Name);
            if (sortType == SortType.ByNameDescending) returnQuery = query.OrderByDescending(a => a.Name);
            if (sortType == SortType.ByCreatedDateAscending) returnQuery = query.OrderBy(a => a.CreatedDate);
            if (sortType == SortType.ByModifiedDateAscending) returnQuery = query.OrderBy(a => a.ModifiedDate);
            if (sortType == SortType.ByModifiedDateDescending) returnQuery = query.OrderByDescending(a => a.ModifiedDate);

            if (pageSize > 0) returnQuery = returnQuery.Skip(pageIndex * pageSize).Take(pageSize);

            if (take > 0) returnQuery = returnQuery.Take(take);

            return returnQuery;
        }

        public async Task<ProvisionDatasetStatusEnum?> GetProvisioningStatusAsync(Guid id)
        {
            return await GetIncludeQueryable()
                .Where(ds => ds.Id == id)
                .Select(ds => ds.ProvisionStatus)
                .FirstOrDefaultAsync();
        }
    }
}
