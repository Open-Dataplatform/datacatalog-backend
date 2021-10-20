using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataCatalog.Common.Enums;

namespace DataCatalog.Data.Model
{
    public class DatasetPermissionChange
    {
        public Guid Id { get; set; }
        public PermissionChangeType PermissionChangeType { get; set; }
        public AccessType AccessType { get; set; }
        public AccessMemberType AccessMemberType { get; set; }
        public string DirectoryObjectId { get; set; }
        public string DisplayName { get; set; }
        public string Mail { get; set; }

        public Guid DatasetChangeLogId { get; set; }
        public DatasetChangeLog DatasetChangeLog { get; set; }
    }

    public enum PermissionChangeType {
        Added,
        Removed
    }

    public enum AccessType {
        Read,
        Write
    }
}