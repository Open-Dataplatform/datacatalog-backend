using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Serilog.Context;

namespace DataCatalog.Api.Services.AD
{
    public class AzureGroupService : IGroupService
    {
        private readonly IGraphServiceClient _graphServiceClient;
        private readonly ILogger<AzureGroupService> _logger;

        public AzureGroupService(
            IGraphServiceClient graphServiceClient, 
            ILogger<AzureGroupService> logger)
        {
            _graphServiceClient = graphServiceClient;
            _logger = logger;
        }

        public async Task<IEnumerable<AccessMember>> GetGroupMembersAsync(string groupId)
        {
            try
            {
                var members = await _graphServiceClient.Groups[groupId].Members.Request().GetAsync();
                using(LogContext.PushProperty("GroupMembers", members, true))
                {
                    _logger.LogDebug("Fetched group members from groupId {GroupId}", groupId);
                }
                return members.Select(GetMemberInfo);
            }

            catch (ServiceException se)
            {
                _logger.LogInformation(se, "Service exception caught from the graph api when trying to get members for group {GroupId}", groupId);
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception caught when retrieving group members for group {GroupId}", groupId);
                throw;
            }
        }

        public async Task<AccessMember> GetAccessMemberAsync(string groupId)
        {
            try
            {
                var directoryObject = await _graphServiceClient.DirectoryObjects[groupId].Request().GetAsync();
                var accessMember = GetMemberInfo(directoryObject);
                using(LogContext.PushProperty("AccessMembers", accessMember, true))
                {
                    _logger.LogDebug("Fetched access members from groupId {GroupId}", groupId);
                }
                return accessMember;
            }
            catch (ServiceException se)
            {
                _logger.LogInformation(se, "Service exception caught from the graph api when trying to get member with id {GroupId}", groupId);
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception caught when retrieving member with id {GroupId}", groupId);
                throw;
            }
        }

        public async Task RemoveGroupMemberAsync(string groupId, string memberId)
        {
            try
            {
                await _graphServiceClient.Groups[groupId].Members[memberId].Reference.Request().DeleteAsync();
                _logger.LogInformation("Removed the member with Id {MemberId} from the group with Id {GroupId}", memberId, groupId);
            }
            catch (ServiceException se)
            {
                _logger.LogWarning(se, "Service exception caught from the graph api when trying to remove member {MemberId} from group {GroupId}", memberId, groupId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception caught when removing member {MemberId} from group {GroupId}", memberId, groupId);
                throw;
            }
        }

        public async Task AddGroupMemberAsync(string groupId, string memberId)
        {
            try
            {
                var members = await _graphServiceClient.Groups[groupId].Members.Request().GetAsync();

                // Just return if member is already present in the group
                if (members.Any(x => Equals(x.Id, memberId)))
                {
                    _logger.LogInformation("Member with Id {MemberId} is already part of the group with Id {GroupId}", memberId, groupId);
                    return;
                }

                await _graphServiceClient.Groups[groupId].Members.References.Request()
                    .AddAsync(new DirectoryObject { Id = memberId });
                _logger.LogInformation("Added the member with Id {MemberId} to the group with Id {GroupId}", memberId, groupId);
            }
            catch (ServiceException se)
            {
                _logger.LogWarning(se, "Service exception caught from the graph api when trying to add member {MemberId} from group {GroupId}", memberId, groupId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception caught when adding member {MemberId} to group {GroupId}", memberId, groupId);
                throw;
            }
        }

        public async Task<IEnumerable<AdSearchResult>> SearchAsync(string searchString)
        {
            var displaySearchQueryOption = new QueryOption("$search", $"\"displayName:{searchString}\"");
            var mailSearchQueryOption = new QueryOption("$search", $"\"mail:{searchString}\"");
            var consistencyLevelHeader = new HeaderOption("ConsistencyLevel", "eventual");

            var groupsTask = _graphServiceClient.Groups.Request(new List<Option> { displaySearchQueryOption, consistencyLevelHeader }).GetAsync();
            var usersTask = _graphServiceClient.Users.Request(new List<Option> { displaySearchQueryOption, consistencyLevelHeader }).GetAsync();
            var mailTask = _graphServiceClient.Users.Request(new List<Option> { mailSearchQueryOption, consistencyLevelHeader }).GetAsync();
            var servicePrincipalsTask = _graphServiceClient.ServicePrincipals.Request(new List<Option> { displaySearchQueryOption, consistencyLevelHeader }).GetAsync();

            await Task.WhenAll(groupsTask, usersTask, mailTask, servicePrincipalsTask);

            var result = groupsTask.Result.Where(x => !x.GroupTypes.Contains("Unified")).Select(x => new AdSearchResult {Id = x.Id, DisplayName = x.DisplayName, Type = AdSearchResultType.Group})
                .Union(usersTask.Result.Select(x => new AdSearchResult { Id = x.Id, DisplayName = x.DisplayName, Mail = x.Mail, Type = AdSearchResultType.User }))
                .Union(mailTask.Result.Select(x => new AdSearchResult { Id = x.Id, DisplayName = x.DisplayName, Mail = x.Mail, Type = AdSearchResultType.User }))
                .Union(servicePrincipalsTask.Result.Select(x => new AdSearchResult { Id = x.Id, DisplayName = x.DisplayName, Type = AdSearchResultType.ServicePrincipal }));

            return result;
        }

        private AccessMember GetMemberInfo(DirectoryObject memberDirectoryObject)
        {
            switch (memberDirectoryObject)
            {
                case Group groupMember:
                    return new AccessMember {Id = groupMember.Id, Name = groupMember.DisplayName, Type = AccessMemberType.Group};
                case Microsoft.Graph.User userMember:
                    return new AccessMember { Id = userMember.Id, Name = userMember.DisplayName, Mail = userMember.Mail, Type = AccessMemberType.User };
                case ServicePrincipal servicePrincipalMember:
                    return new AccessMember { Id = servicePrincipalMember.Id, Name = servicePrincipalMember.DisplayName, Type = AccessMemberType.ServicePrincipal };
                default:
                    return new AccessMember {Id = memberDirectoryObject.Id, Name = memberDirectoryObject.Id, Type = AccessMemberType.Other};
            }
        }
    }
}
