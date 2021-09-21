using DataCatalog.Data.Model;
using DataCatalog.Common.Enums;
using DataCatalog.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using System;

namespace DataCatalog.Data
{
    public class DataCatalogContext : DbContext
    {
        public DbSet<IdentityProvider> IdentityProvider { get; set; }
        public DbSet<Dataset> Datasets { get; set; }
        public DbSet<DataField> DataFields { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<DatasetCategory> DatasetCategories { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Transformation> Transformations { get; set; }
        public DbSet<TransformationDataset> TransformationDatasets { get; set; }
        public DbSet<DatasetGroup> DatasetGroups { get; set; }
        public DbSet<DatasetGroupDataset> DatasetGroupDatasets { get; set; }
        public DbSet<DatasetChangeLog> DatasetChangeLogs { get; set; }
        public DbSet<Duration> Durations { get; set; }
        public DbSet<DatasetDuration> DatasetDurations { get; set; }
        public DbSet<Hierarchy> Hierarchies { get; set; }
        public DbSet<DataSource> DataSources { get; set; }
        public DbSet<DataContract> DataContracts { get; set; }

        public DataCatalogContext(DbContextOptions options) : base(options) { }

        internal DataCatalogContext() { } // For unit test mocking

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseEntityNamesForTableNames();
            modelBuilder.SetDefaultDateValues();

           //Category
            modelBuilder.Entity<Category>(e =>
            {
                e.Property(a => a.Colour).IsRequired();
                e.Property(a => a.Name).IsRequired();
            });

            //DataField
            modelBuilder.Entity<DataField>(e =>
            {
                e.Property(a => a.Name).IsRequired();
                e.Property(a => a.Type).IsRequired();
            });

            //Dataset
            modelBuilder.Entity<Dataset>(e =>
            {
                e.Property(a => a.Version).IsRequired().HasDefaultValue(1);
                e.Property(a => a.Name).IsRequired();
                e.Property(a => a.SourceId).IsRequired();
                e.Property(a => a.Status).IsRequired().HasDefaultValue(DatasetStatus.Draft);
                e.Property(a => a.Confidentiality).IsRequired().HasDefaultValue(Confidentiality.Public);
                e.Property(a => a.RefinementLevel).IsRequired().HasDefaultValue(RefinementLevel.Raw);
                e.Property(a => a.HierarchyId).IsRequired();
            });

            //DatasetGroup
            modelBuilder.Entity<DatasetGroup>(e =>
            {
                e.Property(a => a.Name).IsRequired();
                e.Property(a => a.MemberId).IsRequired();
            });

            //DataSource
            modelBuilder.Entity<DataSource>(e =>
            {
                e.Property(a => a.Name).IsRequired();
                e.Property(a => a.SourceType).IsRequired();
            });

            //Duration
            modelBuilder.Entity<Duration>(e =>
            {
                e.Property(a => a.Code).IsRequired();
                e.Property(a => a.Description).IsRequired();
            });

            //Hierarchy
            modelBuilder.Entity<Hierarchy>(e =>
            {
                e.HasMany(a => a.ChildHierarchies).WithOne(a => a.ParentHierarchy).HasForeignKey(a => a.ParentHierarchyId);
                e.Property(a => a.Name).IsRequired();
            });

            //Member
            modelBuilder.Entity<Member>(e =>
            {
                e.Property(a => a.ExternalId).IsRequired();
            });

            //Transformation
            modelBuilder.Entity<Transformation>(e => e.Property(a => a.ShortDescription).IsRequired());

            //TransformationDataset
            modelBuilder.Entity<TransformationDataset>(e =>
            {
                e.HasKey(a => new { a.DatasetId, a.TransformationId });
                e.Property(a => a.TransformationDirection).IsRequired();
            });

            //Relation types
            modelBuilder.Entity<DatasetCategory>(e => e.HasKey(a => new { a.DatasetId, a.CategoryId }));
            modelBuilder.Entity<DatasetGroupDataset>(e => e.HasKey(a => new { a.DatasetGroupId, a.DatasetId }));
            modelBuilder.Entity<DatasetDuration>(e => e.HasKey(a => new { a.DatasetId, a.DurationId, a.DurationType }));
        }
    }
}