using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataCatalog.Api.Messages;
using DataCatalog.Common.Utils;
using DataCatalog.DatasetResourceManagement.Commands.AccessControlList;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.ActiveDirectory;
using DataCatalog.DatasetResourceManagement.Common.ServiceInterfaces.Storage;
using DataCatalog.DatasetResourceManagement.Messages;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;
using IAllUsersGroupProvider = DataCatalog.Common.Interfaces.IAllUsersGroupProvider;

namespace DataCatalog.DatasetResourceManagement.MessageHandlers
{
    public class DatasetCreatedHandler : IHandleMessages<DatasetCreatedMessage>
    {
        private readonly ILogger<DatasetCreatedHandler> _logger;
        private readonly IStorageService _storageService;
        private readonly IActiveDirectoryGroupService _activeDirectoryGroupService;
        private readonly IAccessControlListService _accessControlListService;
        private readonly IActiveDirectoryGroupProvider _activeDirectoryGroupProvider;
        private readonly IActiveDirectoryRootGroupProvider _activeDirectoryRootGroupProvider;
        private readonly IAllUsersGroupProvider _allUsersGroupProvider;
        private readonly IBus _bus;

        public DatasetCreatedHandler(ILogger<DatasetCreatedHandler> logger, 
            IStorageService storageService, 
            IActiveDirectoryGroupService activeDirectoryGroupService, 
            IAccessControlListService accessControlListService, 
            IActiveDirectoryGroupProvider activeDirectoryGroupProvider, 
            IActiveDirectoryRootGroupProvider activeDirectoryRootGroupProvider, 
            IAllUsersGroupProvider allUsersGroupProvider, 
            IBus bus)
        {
            _logger = logger;
            _storageService = storageService;
            _activeDirectoryGroupService = activeDirectoryGroupService;
            _accessControlListService = accessControlListService;
            _activeDirectoryGroupProvider = activeDirectoryGroupProvider;
            _activeDirectoryRootGroupProvider = activeDirectoryRootGroupProvider;
            _allUsersGroupProvider = allUsersGroupProvider;
            _bus = bus;
        }

        public async Task Handle(DatasetCreatedMessage datasetCreatedMessage)
        {
            try
            {

                const string container = "datasets";
                var path = datasetCreatedMessage.DatasetId.ToString();

                var rootGroupId = await _activeDirectoryRootGroupProvider.ProvideGroupAsync(
                    $"DataPlatform-{container}-Zone-{EnvironmentUtil.GetCurrentEnvironment()}-Reader",
                    $"Root reader group for the {container} zone", container);

                await _storageService.CreateDirectoryIfNeeded(container, path);

                string readerGroupId;
                string writerGroupId;

                await using (var lease = await _storageService.AcquireLeaseAsync(container, path))
                {
                    readerGroupId = await _activeDirectoryGroupProvider.ProvideGroupAsync(
                        $"DataPlatform-DataSet_{datasetCreatedMessage.DatasetId}-{EnvironmentUtil.GetCurrentEnvironment()}-Reader",
                        $"Reader group with id {datasetCreatedMessage.DatasetId} and name at creation {datasetCreatedMessage.DatasetName}", new[] { datasetCreatedMessage.Owner });

                    writerGroupId = await _activeDirectoryGroupProvider.ProvideGroupAsync(
                        $"DataPlatform-DataSet_{datasetCreatedMessage.DatasetId}-{EnvironmentUtil.GetCurrentEnvironment()}-Writer",
                        $"Writer group with id {datasetCreatedMessage.DatasetId} and name at creation {datasetCreatedMessage.DatasetName}");

                    await _accessControlListService.SetGroupsInAccessControlListAsync(new CreateGroupsInAccessControlList
                    {
                        StorageContainer = container,
                        Path = path,
                        GroupEntries = new List<AccessControlListGroupEntry>
                    {
                        new() {Id = readerGroupId, Permissions = "r-x"},
                        new() {Id = readerGroupId, Permissions = "r-x", IsDefault = true},
                        new() {Id = writerGroupId, Permissions = "rwx"}, 
                        new() {Id = writerGroupId, Permissions = "rwx", IsDefault = true}
                    }
                    }, lease.LeaseId);

                    await _storageService.SetDirectoryMetadata(container, path,
                        new Dictionary<string, string>
                        {
                        { "ReaderGroupId", readerGroupId },
                        { "WriterGroupId", writerGroupId },
                        { "RootDirectoryDatasetId", datasetCreatedMessage.DatasetId.ToString() }
                        }, lease.LeaseId);

                    await _accessControlListService.RemoveGroupFromAccessControlListAsync(container, path, rootGroupId, lease.LeaseId);
                }

                await _activeDirectoryGroupService.AddGroupMember(rootGroupId, readerGroupId);
                await _activeDirectoryGroupService.AddGroupMember(rootGroupId, writerGroupId);
                
                if (datasetCreatedMessage.Public)
                    await _activeDirectoryGroupService.AddGroupMember(readerGroupId, _allUsersGroupProvider.GetAllUsersGroup());

                await _bus.Publish(new DatasetProvisionedMessage
                    {
                        DatasetId = datasetCreatedMessage.DatasetId, 
                        Status = "succeeded"
                    }
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred during processing of dataset created event");
                throw;
            }
        }
    }
}