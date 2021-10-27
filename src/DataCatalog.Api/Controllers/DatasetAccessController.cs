using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Api.Data.Dto;
using DataCatalog.Common.Enums;
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
        private readonly IGroupService _groupService;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public DatasetAccessController(
            IGroupService groupService, 
            IStorageService storageService, 
            IMapper mapper)
        {
            _groupService = groupService;
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
            var directoryMetadata = await _storageService.GetDirectoryMetadataWithRetry(id);

            if (directoryMetadata == null)
            {
                return NotFound("Dataset access assignment not ready yet");
            }
            
            directoryMetadata.TryGetValue(GroupConstants.ReaderGroup, out var readerGroupId);
            directoryMetadata.TryGetValue(GroupConstants.WriterGroup, out var writerGroupId);
            
            var readerMembers = readerGroupId == null
                ? null
                : await _groupService.GetGroupMembersAsync(readerGroupId);

            var writerMembers = writerGroupId == null
                ? null
                : await _groupService.GetGroupMembersAsync(writerGroupId);
            
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
            return AddMember(AccessType.Read, datasetId, accessMemberRequest.MemberId);
        }

        [HttpPost]
        [Route("{datasetId}/access/write")]
        public Task<ActionResult<DataAccessEntry>> AddWriteAccessMember(Guid datasetId, AddDatasetAccessMemberRequestDto accessMemberRequest)
        {
            return AddMember(AccessType.Write, datasetId, accessMemberRequest.MemberId);
        }

        [HttpDelete]
        [Route("{datasetId}/access/{memberId}/read")]
        public Task<IActionResult> RemoveReadDataAccessMember(Guid datasetId, Guid memberId)
        {
            return RemoveMember(AccessType.Read, datasetId, memberId);
        }

        [HttpDelete]
        [Route("{datasetId}/access/{memberId}/write")]
        public Task<IActionResult> RemoveWriteDataAccessMember(Guid datasetId, Guid memberId)
        {
            return RemoveMember(AccessType.Write, datasetId, memberId);
        }

        [HttpGet]
        [Route("access")]
        public async Task<ActionResult<IEnumerable<AdSearchResultResponse>>> Search(string search)
        {
            var searchResults = await _groupService.SearchAsync(search);
            
            return Ok(searchResults.Select(x => _mapper.Map<Data.Domain.AdSearchResult, AdSearchResultResponse>(x)));
        }

        private async Task<IActionResult> RemoveMember(AccessType accessType, Guid datasetId, Guid memberId)
        {
            var directoryMetadata = await _storageService.GetDirectoryMetadataWithRetry(datasetId);

            if (directoryMetadata == null)
            {
                return NotFound();
            }

            directoryMetadata.TryGetValue(GroupConstants.AccessGroupTypeKey(accessType), out var groupId);

            if (groupId == null)
            {
                return NotFound();
            }

            await _groupService.RemoveGroupMemberAsync(datasetId, groupId, memberId.ToString(), accessType);

            return Ok();
        }

        private async Task<ActionResult<DataAccessEntry>> AddMember(AccessType accessType, Guid datasetId, Guid memberId)
        {
            var directoryMetadata = await _storageService.GetDirectoryMetadataWithRetry(datasetId);

            if (directoryMetadata == null)
            {
                return NotFound();
            }

            directoryMetadata.TryGetValue(GroupConstants.AccessGroupTypeKey(accessType), out var groupId);

            if (groupId == null)
            {
                return NotFound();
            }

            var addedMember = await _groupService.AddGroupMemberAsync(datasetId, groupId, memberId.ToString(), accessType);

            if (addedMember == null)
            {
                addedMember = await _groupService.GetAccessMemberAsync(groupId);
            }

            return Ok(_mapper.Map<Data.Domain.AccessMember, DataAccessEntry>(addedMember));
        }
    }
}
