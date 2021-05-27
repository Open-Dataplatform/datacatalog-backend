using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Services.AD;

namespace DataCatalog.Api.Services.Local
{
    public class LocalGroupService : IGroupService
    {
        private readonly AccessMember _localAccessMember;
        private readonly AdSearchResult _localSearchResult;

        public LocalGroupService()
        {
            _localAccessMember = new AccessMember()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "LocalMember",
                Type = AccessMemberType.User
            };
            _localSearchResult = new AdSearchResult()
            {
                DisplayName = "LocalSearchResult",
                Id = Guid.NewGuid().ToString(),
                Type = "LocalSearchResultType"
            };
        }

        public Task<IEnumerable<AccessMember>> GetGroupMembersAsync(string id)
        {
            return Task.FromResult(new List<AccessMember> {_localAccessMember}.AsEnumerable());
        }

        public Task<AccessMember> GetAccessMemberAsync(string id)
        {
            return Task.FromResult(_localAccessMember);
        }

        public Task RemoveGroupMemberAsync(string groupId, string memberId)
        {
            return Task.CompletedTask;
        }

        public Task AddGroupMemberAsync(string groupId, string memberId)
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<AdSearchResult>> SearchAsync(string searchString)
        {
            return Task.FromResult(new List<AdSearchResult> {_localSearchResult}.AsEnumerable());
        }
    }
}