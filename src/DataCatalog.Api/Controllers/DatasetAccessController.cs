using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Api.Enums;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.Services.AD;
using DataCatalog.Api.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace DataCatalog.Api.Controllers
{
    [AuthorizeRoles(Role.DataSteward)]
    [Route("api/dataset")]
    [ApiController]
    public class DatasetAccessController : ControllerBase
    {
        private readonly IActiveDirectoryGroupService _activeDirectoryGroupService;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public DatasetAccessController(
            IActiveDirectoryGroupService activeDirectoryGroupService, 
            IStorageService storageService, 
            IMapper mapper)
        {
            _activeDirectoryGroupService = activeDirectoryGroupService;
            _storageService = storageService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get a dataset access list
        /// </summary>
        /// <param name="id">The id of the dataset to get the access list for</param>
        /// <returns>A dataset access list</returns>
        [HttpGet]
        [Route("{id}/access")]
        public async Task<ActionResult<DatasetAccessListResponse>> GetAccessList(Guid id)
        {
            var directoryMetadata = await _storageService.GetDirectoryMetadataAsync(id.ToString());

            if (directoryMetadata == null)
                return NotFound();
            
            directoryMetadata.TryGetValue(GroupConstants.ReaderGroup, out var readerGroupId);
            directoryMetadata.TryGetValue(GroupConstants.WriterGroup, out var writerGroupId);
            
            var readerMembers = readerGroupId == null
                ? null
                : await _activeDirectoryGroupService.GetGroupMembersAsync(readerGroupId);

            var writerMembers = writerGroupId == null
                ? null
                : await _activeDirectoryGroupService.GetGroupMembersAsync(writerGroupId);
            
            var result = new DatasetAccessListResponse
            {
                ReadAccessList = readerMembers?.Select(x => _mapper.Map<Data.Domain.AccessMember, DataAccessEntry>(x)),
                WriteAccessList = writerMembers?.Select(x => _mapper.Map<Data.Domain.AccessMember, DataAccessEntry>(x))
            };
            
            return Ok(result);
        }

        [HttpPost]
        [Route("{datasetId}/access/read")]
        public Task<ActionResult<DataAccessEntry>> AddReadAccessMember(Guid datasetId, AddDatasetAccessMemberRequestDto accessMemberRequest)
        {
            return AddMember(GroupConstants.ReaderGroup, datasetId, accessMemberRequest.MemberId);
        }

        [HttpPost]
        [Route("{datasetId}/access/write")]
        public Task<ActionResult<DataAccessEntry>> AddWriteAccessMember(Guid datasetId, AddDatasetAccessMemberRequestDto accessMemberRequest)
        {
            return AddMember(GroupConstants.WriterGroup, datasetId, accessMemberRequest.MemberId);
        }

        [HttpDelete]
        [Route("{datasetId}/access/{memberId}/read")]
        public Task<IActionResult> RemoveReadDataAccessMember(Guid datasetId, Guid memberId)
        {
            return RemoveMember(GroupConstants.ReaderGroup, datasetId, memberId);
        }

        [HttpDelete]
        [Route("{datasetId}/access/{memberId}/write")]
        public Task<IActionResult> RemoveWriteDataAccessMember(Guid datasetId, Guid memberId)
        {
            return RemoveMember(GroupConstants.WriterGroup, datasetId, memberId);
        }

        [HttpGet]
        [Route("access")]
        public async Task<ActionResult<IEnumerable<AdSearchResultResponse>>> Search(string search)
        {
            var searchResults = await _activeDirectoryGroupService.SearchAsync(search);
            
            return Ok(searchResults.Select(x => _mapper.Map<Data.Domain.AdSearchResult, AdSearchResultResponse>(x)));
        }

        private async Task<IActionResult> RemoveMember(string accessGroupType, Guid datasetId, Guid memberId)
        {
            var directoryMetadata = await _storageService.GetDirectoryMetadataAsync(datasetId.ToString());

            if (directoryMetadata == null)
                return NotFound();

            directoryMetadata.TryGetValue(accessGroupType, out var groupId);

            if (groupId == null)
                return NotFound();

            await _activeDirectoryGroupService.RemoveGroupMemberAsync(groupId, memberId.ToString());

            return Ok();
        }

        private async Task<ActionResult<DataAccessEntry>> AddMember(string accessGroupType, Guid datasetId, Guid memberId)
        {
            var directoryMetadata = await _storageService.GetDirectoryMetadataAsync(datasetId.ToString());

            if (directoryMetadata == null)
                return NotFound();

            directoryMetadata.TryGetValue(accessGroupType, out var groupId);

            if (groupId == null)
                return NotFound();

            await _activeDirectoryGroupService.AddGroupMemberAsync(groupId, memberId.ToString());
            var addedMember = await _activeDirectoryGroupService.GetAccessMemberAsync(memberId.ToString());
            
            return Ok(_mapper.Map<Data.Domain.AccessMember, DataAccessEntry>(addedMember));
        }
    }
}
