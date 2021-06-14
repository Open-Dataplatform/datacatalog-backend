﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Domain;

namespace DataCatalog.Api.Services.AD
{
    public interface IGroupService
    {
        Task<IEnumerable<AccessMember>> GetGroupMembersAsync(string id);
        Task<AccessMember> GetAccessMemberAsync(string id);
        Task RemoveGroupMemberAsync(string groupId, string memberId);
        Task AddGroupMemberAsync(string groupId, string memberId);
        Task<IEnumerable<AdSearchResult>> SearchAsync(string searchString);
    }
}