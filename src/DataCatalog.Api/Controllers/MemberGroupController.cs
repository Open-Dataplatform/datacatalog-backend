using DataCatalog.Api.Data.Dto;
using DataCatalog.Common.Enums;
using DataCatalog.Api.Infrastructure;
using DataCatalog.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DataCatalog.Common.Data;

namespace DataCatalog.Api.Controllers
{
    [AuthorizeRoles(Role.Admin, Role.DataSteward, Role.User)]
    [ApiController]
    [Route("api/[controller]")]
    public class MemberGroupController : ControllerBase
    {
        private readonly IMemberGroupService _memberGroupService;
        private readonly IMapper _mapper;
        private readonly Current _current;

        public MemberGroupController(IMemberGroupService memberGroupService, IMapper mapper, Current current)
        {
            _memberGroupService = memberGroupService;
            _mapper = mapper;
            _current = current;
        }

        /// <summary>
        /// Get all member groups
        /// </summary>
        /// <returns>A list of all member groups</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberGroupResponse>>> GetAllAsync()
        {
            var memberGroups = await _memberGroupService.ListAsync();

            if (memberGroups == null)
                return NotFound();

            var result = _mapper.Map<IEnumerable<Data.Domain.MemberGroup>, IEnumerable<MemberGroupResponse>>(memberGroups);

            return Ok(result);
        }

        /// <summary>
        /// Get a member group by id
        /// </summary>
        /// <param name="id">The id of the member group to get</param>
        /// <returns>The member group</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<MemberGroupResponse>> Get(Guid id)
        {
            var memberGroup = await _memberGroupService.FindByIdAsync(id);

            if (memberGroup == null)
                return NotFound();

            var result = _mapper.Map<Data.Domain.MemberGroup, MemberGroupResponse>(memberGroup);

            return Ok(result);
        }

        /// <summary>
        /// Get the groups that the current member is a member of
        /// </summary>
        /// <param name="id">The id of the member</param>
        /// <returns>The member groups</returns>
        [HttpGet]
        [Route("membergroups/{id}")]
        public async Task<ActionResult<IEnumerable<MemberGroupResponse>>> GetMemberGroups(Guid id)
        {
            var memberGroups = await _memberGroupService.GetMemberGroups(id);

            var result = _mapper.Map<IEnumerable<Data.Domain.MemberGroup>, IEnumerable<MemberGroupResponse>>(memberGroups);

            return Ok(result);
        }

        /// <summary>
        /// Create a new member group
        /// </summary> 
        /// <param name="request">The member group to create</param>
        /// <returns>The created member group</returns>
        [AuthorizeRoles(Role.Admin)]
        [HttpPost]
        public async Task<ActionResult<Guid>> PostAsync([FromBody] MemberGroupCreateRequest request)
        {
            var memberGroup = _mapper.Map<MemberGroupCreateRequest, Data.Domain.MemberGroup>(request);
            await _memberGroupService.SaveAsync(memberGroup);

            return Ok(memberGroup.Id);
        }

        /// <summary>
        /// Add member to member group
        /// </summary>
        /// <param name="request">The member group and member to match</param>
        [HttpPost("AddMember")]
        public async Task<IActionResult> AddMemberAsync([FromBody] MemberGroupAddRequest request)
        {
            if (_current.MemberId == request.MemberId)
                await _memberGroupService.AddMemberAsync(request.MemberGroupId, request.MemberId);
            else if (_current.Roles.Contains(Role.DataSteward) || _current.Roles.Contains(Role.Admin))
                await _memberGroupService.AddMemberAsync(request.MemberGroupId, request.MemberId);
            else
                return BadRequest();

            return Ok();
        }

        /// <summary>
        /// Remove member from member group
        /// </summary>
        /// <param name="request">The member group and member to match</param>
        [HttpPost("RemoveMember")]
        public async Task<IActionResult> RemoveMemberAsync([FromBody] MemberGroupRemoveRequest request)
        {
            if (_current.MemberId == request.MemberId)
                await _memberGroupService.RemoveMemberAsync(request.MemberGroupId, request.MemberId);
            else if (_current.Roles.Contains(Role.DataSteward) || _current.Roles.Contains(Role.Admin))
                await _memberGroupService.RemoveMemberAsync(request.MemberGroupId, request.MemberId);
            else
                return BadRequest();

            return Ok();
        }

        /// <summary>
        /// Delete a data source
        /// </summary>
        /// <param name="id">The id of the data source to delete</param>
        /// <remarks>Data sources with references to data contracts cannot be deleted!</remarks>
        [AuthorizeRoles(Role.Admin)]
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _memberGroupService.DeleteAsync(id);

            return Ok();
        }
    }
}