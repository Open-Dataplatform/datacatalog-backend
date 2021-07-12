using System.Collections.Generic;

namespace DataCatalog.DatasetResourceManagement.Commands.AccessControlList
{
    public class CreateGroupsInAccessControlList
    {
        public string StorageContainer { get; set; }
        public string Path { get; set; }
        public IEnumerable<AccessControlListGroupEntry> GroupEntries { get; set; }
    }
}
