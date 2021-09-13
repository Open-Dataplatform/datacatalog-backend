using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataCatalog.Api.Data.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

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

        public async Task<IEnumerable<AccessMember>> GetGroupMembersAsync(string id)
        {
            try
            {
                var members = await _graphServiceClient.Groups[id].Members.Request().GetAsync();
                return members.Select(GetMemberInfo);
            }

            catch (ServiceException se)
            {
                _logger.LogInformation(se, $"Service exception caught from the graph api when trying to get members for group {id}");
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception caught when retrieving group members for group {id}");
                throw;
            }
        }

        public async Task<AccessMember> GetAccessMemberAsync(string id)
        {
            try
            {
                var directoryObject = await _graphServiceClient.DirectoryObjects[id].Request().GetAsync();
                return GetMemberInfo(directoryObject);
            }
            catch (ServiceException se)
            {
                _logger.LogInformation(se, $"Service exception caught from the graph api when trying to get member with id {id}");
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception caught when retrieving member with id {id}");
                throw;
            }
        }

        public async Task RemoveGroupMemberAsync(string groupId, string memberId)
        {
            try
            {
                await _graphServiceClient.Groups[groupId].Members[memberId].Reference.Request().DeleteAsync();
            }
            catch (ServiceException se)
            {
                _logger.LogWarning(se, $"Service exception caught from the graph api when trying to remove member {memberId} from group {groupId}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception caught when removing member {memberId} from group {groupId}");
                throw;
            }
        }

        public async Task AddGroupMemberAsync(string groupId, string memberId)
        {
            try
            {
                var members = await _graphServiceClient.Groups[groupId].Members.Request().GetAsync();

                // Just return if member is already present in the group
                if (members.Any(x => Equals(x.Id, memberId))) return;

                await _graphServiceClient.Groups[groupId].Members.References.Request()
                    .AddAsync(new DirectoryObject { Id = memberId });
            }
            catch (ServiceException se)
            {
                _logger.LogWarning(se, $"Service exception caught from the graph api when trying to add member {memberId} from group {groupId}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception caught when adding member {memberId} to group {groupId}");
                throw;
            }
        }

        public async Task<IEnumerable<AdSearchResult>> SearchAsync(string searchString)
        {
            var searchQueryOption = new QueryOption("$search", $"\"displayName:{searchString}\"");
            var consistencyLevelHeader = new HeaderOption("ConsistencyLevel", "eventual");
            
            var groupsTask = _graphServiceClient.Groups.Request(new List<Option> { searchQueryOption, consistencyLevelHeader }).GetAsync();
            var usersTask = _graphServiceClient.Users.Request(new List<Option> { searchQueryOption, consistencyLevelHeader }).GetAsync();
            var servicePrincipalsTask = _graphServiceClient.ServicePrincipals.Request(new List<Option> { searchQueryOption, consistencyLevelHeader }).GetAsync();

            await Task.WhenAll(groupsTask, usersTask, servicePrincipalsTask);

            var result = groupsTask.Result.Where(x => !x.GroupTypes.Contains("Unified")).Select(x => new AdSearchResult {Id = x.Id, DisplayName = x.DisplayName, Type = AdSearchResultType.Group})
                .Union(usersTask.Result.Select(x => new AdSearchResult { Id = x.Id, DisplayName = $"{x.DisplayName} ({x.Mail})", Type = AdSearchResultType.User }))
                .Union(servicePrincipalsTask.Result.Select(x => new AdSearchResult { Id = x.Id, DisplayName = x.DisplayName, Type = AdSearchResultType.ServicePrincipal }));

            return result;
        }

        private AccessMember GetMemberInfo(DirectoryObject memberDirectoryObject)
        {
            if (memberDirectoryObject is Group groupMember)
                return new AccessMember {Id = groupMember.Id, Name = groupMember.DisplayName, Type = AccessMemberType.Group};
            
            if (memberDirectoryObject is Microsoft.Graph.User userMember)
                return new AccessMember { Id = userMember.Id, Name = $"{userMember.DisplayName} ({userMember.Mail})", Type = AccessMemberType.User };
            
            if (memberDirectoryObject is ServicePrincipal servicePrincipalMember)
                return new AccessMember { Id = servicePrincipalMember.Id, Name = servicePrincipalMember.DisplayName, Type = AccessMemberType.ServicePrincipal };

            return new AccessMember {Id = memberDirectoryObject.Id, Name = memberDirectoryObject.Id, Type = AccessMemberType.Other};
        }
    }
}
