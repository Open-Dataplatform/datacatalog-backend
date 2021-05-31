using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataCatalog.Data.Extensions
{
    public static class ModelBuilderExtension
    {
        public static void UseEntityNamesForTableNames(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
                entityType.SetTableName(entityType.DisplayName());
        }

        public static void SetDefaultDateValues(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                var createdDate = entityType.FindProperty("CreatedDate");
                if (createdDate != null)
                    createdDate.SetDefaultValueSql("getutcdate()");

                var modifiedDate = entityType.FindProperty("ModifiedDate");
                if (modifiedDate != null)
                    modifiedDate.SetDefaultValueSql("getutcdate()");
            }
        }
    }
}