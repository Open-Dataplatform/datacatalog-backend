﻿using DataCatalog.Api.Data;
using DataCatalog.Api.Data.Model;
using DataCatalog.Api.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public class DatasetRepository : BaseRepository, IDatasetRepository
    {
        public DatasetRepository(DataCatalogContext context) : base(context)
        { }

        public async Task<Dataset> FindByIdAsync(Guid id)
        {
            var dataset = await _context.Datasets
                .Include(a => a.DataFields)
                .Include(a => a.Contact)
                .Include(a => a.DatasetCategories).ThenInclude(a => a.Category)
                .Include(a => a.DatasetDurations).ThenInclude(a => a.Duration)
                .Include(a => a.DatasetChangeLogs).ThenInclude(a => a.Member)
                .Include(a => a.Hierarchy).ThenInclude(a => a.ParentHierarchy)
                .Include(a => a.DataContracts).ThenInclude(a => a.DataSource)
                .FirstOrDefaultAsync(a => a.Id == id);

            //Only load source transformation and related datasets
            await _context.TransformationDatasets.Include(a => a.Transformation)
                .ThenInclude(a => a.TransformationDatasets).ThenInclude(a => a.Dataset)
                .Where(a => a.DatasetId == id && a.TransformationDirection == TransformationDirection.Sink)
                .LoadAsync();

            return dataset;
        }

        public async Task<IEnumerable<Dataset>> ListSummariesAsync(bool onlyPublished)
        {
            return await GetIncludeQueryable(onlyPublished).ToArrayAsync();
        }

        public async Task<IEnumerable<Dataset>> GetDatasetByCategoryAsync(Guid categoryId, SortType sortType, int take, int pageSize, int pageIndex, bool onlyPublished)
        {
            var query = GetIncludeQueryable(onlyPublished).Where(a => a.DatasetCategories.Any(b => b.CategoryId == categoryId));
            
            query = GetOrderedAndChunkedQuery(query, sortType, take, pageSize, pageIndex);

            return await query.ToArrayAsync();
        }

        public async Task<IEnumerable<Dataset>> GetDatasetsBySearchTermQueryAsync(string searchTerm, SortType sortType, int take, int pageSize, int pageIndex, bool onlyPublished)
        {
            var query = GetIncludeQueryable(onlyPublished);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = Regex.Replace(searchTerm, @"\s+", " ").ToLower(); //Replace multiple whitespace characters with single space
                if (term == "draft")
                    query = query.Where(a => a.Status == DatasetStatus.Draft);
                else if (term == "published")
                    query = query.Where(a => a.Status == DatasetStatus.Published);
                else if (term == "archived")
                    query = query.Where(a => a.Status == DatasetStatus.Archived);
                else
                {
                    var terms = term.Split(' ').Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();

                    var unionQuery = GetSearchTermQuery(query, terms[0]);
                    for (int i = 1; i < terms.Length; i++)
                        unionQuery = unionQuery.Union(GetSearchTermQuery(query, terms[i]));
                    query = unionQuery;
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
        }

        private IQueryable<Dataset> GetIncludeQueryable(bool filterUnpublished)
        {
            var query = _context.Datasets.AsQueryable();

            if (filterUnpublished)
                query = query.Where(a => a.Status == DatasetStatus.Published);

            query = query.Include(a => a.DatasetCategories).ThenInclude(a => a.Category);

            return query;
        }

        private static IQueryable<Dataset> GetSearchTermQuery(IQueryable<Dataset> query, string t)
        {
            return from ds in query
                   where ds.Name.Contains(t)
                         || t == "draft" && ds.Status == DatasetStatus.Draft
                         || t == "published" && ds.Status == DatasetStatus.Published
                         || t == "archived" && ds.Status == DatasetStatus.Archived
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
    }
}
