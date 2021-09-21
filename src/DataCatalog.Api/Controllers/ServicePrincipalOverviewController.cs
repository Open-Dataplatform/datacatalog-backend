using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.Services;
using DataCatalog.Api.Services.AD;
using DataCatalog.Api.Services.Storage;
using DataCatalog.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DataCatalog.Api.Controllers
{
    [AuthorizeRoles(Role.Admin, Role.DataSteward)]
    [ApiController]
    [Route("api/[controller]")]
    public class ServicePrincipalOverviewController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IStorageService _storageService;
        private readonly IDatasetService _datasetService;
        private readonly IMapper _mapper;

        public ServicePrincipalOverviewController(IGroupService groupService, IDatasetService datasetService, IStorageService storageService, IMapper mapper)
        {
            _groupService = groupService;
            _datasetService = datasetService;
            _storageService = storageService;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SPDatasetAccessListDto>>> GetOverview()
        {
            var datasets = await _datasetService.GetAllSummariesAsync();
            var result = new List<SPDatasetAccessListDto>();
            foreach (var dataset in datasets)
            {
                var directoryMetadata = await _storageService.GetDirectoryMetadataWithRetry(dataset.Id);

                if (directoryMetadata == null)
                {
                    result.Add(new SPDatasetAccessListDto
                    {
                        DatasetId = dataset.Id,
                        ReadAccessList = Enumerable.Empty<DataAccessEntry>(),
                        WriteAccessList = Enumerable.Empty<DataAccessEntry>()
                    });
                    continue;
                }

                directoryMetadata.TryGetValue(GroupConstants.ReaderGroup, out var readerGroupId);
                directoryMetadata.TryGetValue(GroupConstants.WriterGroup, out var writerGroupId);
            
                var readerMembers = readerGroupId == null
                    ? null
                    : await _groupService.GetGroupMembersAsync(readerGroupId);

                var writerMembers = writerGroupId == null
                    ? null
                    : await _groupService.GetGroupMembersAsync(writerGroupId);
            
                result.Add(new SPDatasetAccessListDto
                {
                    DatasetId = dataset.Id,
                    ReadAccessList = readerMembers?.Select(x => _mapper.Map<Data.Domain.AccessMember, DataAccessEntry>(x)),
                    WriteAccessList = writerMembers?.Select(x => _mapper.Map<Data.Domain.AccessMember, DataAccessEntry>(x))
                });
            }

            return Ok(result);
        }
    }
}