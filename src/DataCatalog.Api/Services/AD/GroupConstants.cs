using System;
using DataCatalog.Common.Enums;

namespace DataCatalog.Api.Services.AD
{
    public class GroupConstants
    {
        public const string ReaderGroup = "ReaderGroupId";
        public const string WriterGroup = "WriterGroupId";

        public static string AccessGroupTypeKey(AccessType accessType) => 
            accessType switch
            {
                AccessType.Read => GroupConstants.ReaderGroup,
                AccessType.Write => GroupConstants.WriterGroup,
                _ => throw new ArgumentException("Unknown access type")
            };
    }
}
