﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Domain;
using DataCatalog.Api.Services.AD;
using DataCatalog.Common.Enums;
using DataCatalog.Common.Utils;

namespace DataCatalog.Api.Services.Local
{
    /// <summary>
    /// Dummy Group service implementation used ONLY for the local environment runtime.
    /// DO NOT use this in any other context! 
    /// </summary>
    public class LocalGroupService : IGroupService
    {
        private readonly AccessMember _localAccessMember;
        private readonly AdSearchResult _localSearchResult;

        public LocalGroupService()
        {
            if (!EnvironmentUtil.IsDevelopment())
            {
                throw new InvalidOperationException("This class cannot be used unless the environment is local");
            }
            
            _localAccessMember = new AccessMember()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "LocalMember",
                Mail = "local@test.dk",
                Type = AccessMemberType.User
            };
            _localSearchResult = new AdSearchResult()
            {
                DisplayName = "LocalSearchResult",
                Id = Guid.NewGuid().ToString(),
                Mail = "local@test.dk",
                Type = "LocalSearchResultType"
            };
        }

        public Task<IEnumerable<AccessMember>> GetGroupMembersAsync(string groupId)
        {
            return Task.FromResult(new List<AccessMember> {_localAccessMember}.AsEnumerable());
        }

        public Task<AccessMember> GetAccessMemberAsync(string groupId)
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