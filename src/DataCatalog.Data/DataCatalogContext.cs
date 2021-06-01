using DataCatalog.Data.Model;
using DataCatalog.Common.Enums;
using DataCatalog.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using System;

namespace DataCatalog.Data
{
    public class DataCatalogContext : DbContext
    {
        internal static Guid DPGuid => new Guid("8c30dee9-af3f-4bd1-bca4-641492e7690b");

        public DbSet<IdentityProvider> IdentityProvider { get; set; }
        public DbSet<Dataset> Datasets { get; set; }
        public DbSet<DataField> DataFields { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<DatasetCategory> DatasetCategories { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<MemberGroup> MemberGroups { get; set; }
        public DbSet<MemberGroupMember> MemberGroupMembers { get; set; }
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
                e.Property(a => a.ContactId).IsRequired();
                e.Property(a => a.SourceId).IsRequired().HasDefaultValue(new Guid("5777efb5-f55c-4f16-bb83-0125b810f995"));
                e.Property(a => a.Status).IsRequired().HasDefaultValue(DatasetStatus.Draft);
                e.Property(a => a.Confidentiality).IsRequired().HasDefaultValue(Confidentiality.Public);
                e.Property(a => a.RefinementLevel).IsRequired().HasDefaultValue(RefinementLevel.Raw);
                e.Property(a => a.HierarchyId).IsRequired().HasDefaultValue(new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0"));
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

            //MemberGroup
            modelBuilder.Entity<MemberGroup>(e =>
            {
                e.HasMany(a => a.Datasets).WithOne(a => a.Contact).HasForeignKey(a => a.ContactId);
                e.Property(a => a.Email).IsRequired();
                e.Property(a => a.Name).IsRequired();
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
            modelBuilder.Entity<MemberGroupMember>(e => e.HasKey(a => new { a.MemberGroupId, a.MemberId }));
            modelBuilder.Entity<DatasetGroupDataset>(e => e.HasKey(a => new { a.DatasetGroupId, a.DatasetId }));
            modelBuilder.Entity<DatasetDuration>(e => e.HasKey(a => new { a.DatasetId, a.DurationId, a.DurationType }));

            //Seed data
            SeedData(modelBuilder);
        }

        void SeedData(ModelBuilder modelBuilder)
        {
            //Seed categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = new Guid("8713b259-0294-480a-960c-08a9c9983961"), Name = "Ancillary Services", Colour = "#2A939B", OriginEnvironment = "prod" },
                new Category { Id = new Guid("b94863a9-685a-44d6-aae4-2cfb3a63c489"), Name = "Auctions, Transmission Capacity", Colour = "#389B88", OriginEnvironment = "prod" },
                new Category { Id = new Guid("e5b2f760-e965-452f-b0ab-70ca506d4c20"), Name = "Emissions", Colour = "#452A9B", OriginEnvironment = "prod" },
                new Category { Id = new Guid("08e58f12-51bf-4af9-af3a-02022cab77c7"), Name = "Day Ahead Market", Colour = "#2A9B65", OriginEnvironment = "prod" },
                new Category { Id = new Guid("23ab1cd5-ae16-4aa4-bab7-25380c1634d5"), Name = "Electric Boilers", Colour = "#B27736", OriginEnvironment = "prod" },
                new Category { Id = new Guid("8a95d290-417e-4a53-a807-a13293f3117d"), Name = "Gas", Colour = "#F8AE3C", OriginEnvironment = "prod" },
                new Category { Id = new Guid("5873b2e5-5db1-42db-bf25-83fd9dded293"), Name = "Production and Consumption", Colour = "#663BCC", OriginEnvironment = "prod" },
                new Category { Id = new Guid("084e6353-82a8-4f99-b9f3-b247081c34c8"), Name = "Electricity Consumption", Colour = "#A0C1C2", OriginEnvironment = "prod" },
                new Category { Id = new Guid("5b39fbdd-76c5-44df-9f7d-d314db760fc1"), Name = "Intra Day Market", Colour = "#819B38", OriginEnvironment = "prod" },
                new Category { Id = new Guid("a90abc39-c2de-4c29-a9f8-4e10f41a50f0"), Name = "Electricity Production", Colour = "#293A4C", OriginEnvironment = "prod" },
                new Category { Id = new Guid("9ac5af8d-4165-448a-ace5-2edcf0c3e202"), Name = "Regulating Power", Colour = "#548E80", OriginEnvironment = "prod" },
                new Category { Id = new Guid("7434d8a3-7ebb-4212-967c-061d5a2666d7"), Name = "Whole Sale Market", Colour = "#7D8E1C", OriginEnvironment = "prod" },
                new Category { Id = new Guid("90909fcd-4a5a-455a-a543-9c2c8a45e453"), Name = "Reserves", Colour = "#398C22", OriginEnvironment = "prod" },
                new Category { Id = new Guid("d6944781-c069-4c14-859f-07865164763f"), Name = "Solar Power", Colour = "#FFD424", OriginEnvironment = "prod" },
                new Category { Id = new Guid("b4cef11a-d0f4-4c33-9796-b80ea6d2489a"), Name = "Transmission Lines", Colour = "#547B8E", OriginEnvironment = "prod" },
                new Category { Id = new Guid("418e657d-3efb-4a30-ad3f-3f95a6ab8888"), Name = "Wind Power", Colour = "#C2E4F0", OriginEnvironment = "prod" }
            );

            //Seed durations
            modelBuilder.Entity<Duration>().HasData(
                new Duration { Id = new Guid("f745f308-1542-4a7a-975f-0cd6f1e73668"), Code = "PT1H", Description = "1 hour" },
                new Duration { Id = new Guid("2ff90ae1-27a8-4cb8-a697-6e81a5e36fb0"), Code = "P1Y", Description = "1 year" },
                new Duration { Id = new Guid("6817a5e6-7c80-470f-a1b2-fa79ed3c125d"), Code = "P1M", Description = "1 month" },
                new Duration { Id = new Guid("c70a3773-3473-4074-8c08-4a1bfffaa1d1"), Code = "P7D", Description = "7 days" },
                new Duration { Id = new Guid("20986017-c3a8-41a0-8221-b5e1a5995c05"), Code = "P1D", Description = "1 day" },
                new Duration { Id = new Guid("2b3d0756-0817-4281-a2ca-76c2b0eb90d6"), Code = "PT12H", Description = "12 hours" },
                new Duration { Id = new Guid("984b1eb4-0420-4354-8ae7-a9b6c9c33aa8"), Code = "PT6H", Description = "6 hours" },
                new Duration { Id = new Guid("2fb37506-3252-4065-a81b-5c633e158a7c"), Code = "PT15M", Description = "15 minutes" },
                new Duration { Id = new Guid("b64a8a70-cb40-4117-93ba-093571ea4aeb"), Code = "PT5M", Description = "5 minutes" },
                new Duration { Id = new Guid("d05a8570-6155-41bd-95b1-20e6f2856cc2"), Code = "PT3M", Description = "3 minutes" },
                new Duration { Id = new Guid("b57f0531-0eb0-4bc2-b81c-266a3f52368e"), Code = "PT1M", Description = "1 minute" });

            ////Seed members
            //modelBuilder.Entity<Member>().HasData(
            //    new { Id = new Guid("64564746-b34b-4416-a412-df3503bf45db"), Name = "Dummy User", Email = "user@dummy.com", Password = "dummy", MemberRole = MemberRole.User },
            //    new { Id = new Guid("d81769d1-b643-41e6-ad5a-ec6b86f7e608"), Name = "Dummy Admin", Email = "admin@dummy.com", Password = "IAmAdmin", MemberRole = MemberRole.Admin },
            //    new { Id = new Guid("8c30dee9-af3f-4bd1-bca4-641492e7690b"), Name = "Data Platform", Email = "dataplatform@energinet.dk", Password = "GoesWithoutSaying", MemberRole = MemberRole.Admin },
            //    new { Id = new Guid("2fe99cad-bee1-4b48-9336-a793ebdc6e92"), Name = "Sune Buss Vels Jensen", Email = "sjj@energinet.dk", Password = "MegetHemmeligt", MemberRole = MemberRole.DataSteward },
            //    new { Id = new Guid("327bf4e6-bccb-4702-8966-0f7afefdd617"), Name = "James Clifford Stegmann", Email = "jtg@energinet.dk", Password = "IngenProblemer", MemberRole = MemberRole.DataSteward },
            //    new { Id = new Guid("a2edfe21-83d9-4fb1-9365-101f4f0d38c3"), Name = "André Bryde Alnor (bruger)", Email = "ach-user@energinet.dk", Password = "ÆNådDoKaGæt", MemberRole = MemberRole.User },
            //    new { Id = new Guid("905984a3-0e70-4ff5-85be-6325b3d1bab8"), Name = "André Bryde Alnor (data steward)", Email = "ach-datasteward@energinet.dk", Password = "ÆNådDoKaGæt", MemberRole = MemberRole.DataSteward });

            ////Seed member groups
            //modelBuilder.Entity<MemberGroup>().HasData(
            //    new MemberGroup { Id = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "Sune's Group", Email = "sjj@energinet.dk" });
            //modelBuilder.Entity<MemberGroupMember>().HasData(
            //    new MemberGroupMember { MemberGroupId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), MemberId = new Guid("2fe99cad-bee1-4b48-9336-a793ebdc6e92") });

            //modelBuilder.Entity<MemberGroup>().HasData(
            //    new MemberGroup { Id = new Guid("ce24f2c1-2156-4d4f-b330-5fc7c4988f09"), Name = "James's Group", Email = "jtg@energinet.dk" });
            //modelBuilder.Entity<MemberGroupMember>().HasData(
            //    new MemberGroupMember { MemberGroupId = new Guid("ce24f2c1-2156-4d4f-b330-5fc7c4988f09"), MemberId = new Guid("327bf4e6-bccb-4702-8966-0f7afefdd617") });

            //Seed hierarchies
            modelBuilder.Entity<Hierarchy>().HasData(
                //Electricity
                new Hierarchy { Id = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0"), Name = "Electricity", Description = "Anything mainly related to electricity" },
                new Hierarchy { Id = new Guid("ad240fb2-ca95-4ec0-9bc2-58c73bc4ac31"), Name = "Consumption", Description = "Electricity consumption", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("276af4b1-72ea-4654-8ce3-37e73b2657c0"), Name = "Production", Description = "Electricity production", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("a9a780d7-8f2c-4d86-b863-1327bd264575"), Name = "Consumption and Production", Description = "Electricity consumption and production", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"), Name = "Reserves", Description = "Electricity reserves", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("a6b40fdf-9831-49ec-87b0-cfed8f447577"), Name = "Ancillary Services", Description = "Electricity ancillary services", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("5f2d33fc-decc-4061-af3c-5d58e65bbc9c"), Name = "CO2 Emissions", Description = "Electricity CO2 Emissions", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("d27b19d3-e589-4f98-809a-351a062635e8"), Name = "Regulating Power", Description = "Electricity regulating power", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("fb668c70-219c-4edf-b31a-7ad36ee43d88"), Name = "Solar Power", Description = "Electricity solar power", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("ded03663-ec2e-445f-b897-b5b78c00f2d4"), Name = "Wind Power", Description = "Electricity wind power", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("c5dfc671-dcb3-4f10-bf11-758c1d3e97be"), Name = "Capacities", Description = "Electricity capacities", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                new Hierarchy { Id = new Guid("fc20e468-5ab4-48db-8a0b-09e86935c3aa"), Name = "Forecasts", Description = "Electricity forecasts", ParentHierarchyId = new Guid("668f1737-5501-4db5-a072-c2fa37ef26d0") },
                //Gas
                new Hierarchy { Id = new Guid("586c321c-3f01-4cd2-8228-000670114e32"), Name = "Gas", Description = "Anything mainly related to gas" },
                new Hierarchy { Id = new Guid("5b22fc05-6837-40f4-a8a2-184e9aa25aff"), Name = "Consumption", Description = "Gas consumption", ParentHierarchyId = new Guid("586c321c-3f01-4cd2-8228-000670114e32") },
                new Hierarchy { Id = new Guid("49ca837b-79ba-4a4f-bbc5-e01a8f23befd"), Name = "Production", Description = "Gas Production", ParentHierarchyId = new Guid("586c321c-3f01-4cd2-8228-000670114e32") },
                new Hierarchy { Id = new Guid("7330f2ef-f86f-4b7d-8592-e7c56ef52d15"), Name = "Consumption and Production", Description = "Gas consumption and production", ParentHierarchyId = new Guid("586c321c-3f01-4cd2-8228-000670114e32") },
                new Hierarchy { Id = new Guid("334740dd-7158-475d-84e9-c8e83403b515"), Name = "Storage", Description = "Gas storage", ParentHierarchyId = new Guid("586c321c-3f01-4cd2-8228-000670114e32") },
                new Hierarchy { Id = new Guid("1653a169-9548-46f6-aa47-ea9a90088b0f"), Name = "CO2 Emissions", Description = "Gas CO2 Emissions", ParentHierarchyId = new Guid("586c321c-3f01-4cd2-8228-000670114e32") },
                //Weather
                new Hierarchy { Id = new Guid("596203b3-743a-4482-8b20-f4bc6eb844b6"), Name = "Weather", Description = "Anything mainly related to weather" },
                new Hierarchy { Id = new Guid("a8adde4d-4dc9-4028-8993-0ef9bc153d7d"), Name = "Forecasts", Description = "Weather forecasts", ParentHierarchyId = new Guid("596203b3-743a-4482-8b20-f4bc6eb844b6") },
                new Hierarchy { Id = new Guid("077a87f9-1842-40b4-b132-1c966b41d761"), Name = "Historical Data", Description = "Weather historical data", ParentHierarchyId = new Guid("596203b3-743a-4482-8b20-f4bc6eb844b6") }
            );

            //Seed data sources
            modelBuilder.Entity<DataSource>().HasData(
                new DataSource { Id = new Guid("14c7e740-6104-4036-af76-73891fdf0033"), OriginEnvironment = "prod", Name = "DataPlatform", Description = "Use for any dataset that originates from within the data platform itself", ContactInfo = "dataplatform@energinet.dk", SourceType = SourceType.DataPlatform },
                new DataSource { Id = new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab"), OriginEnvironment = "prod", Name = "DPS", Description = "Energinet system DPS", ContactInfo = "dps@energinet.dk", SourceType = SourceType.Internal },
                new DataSource { Id = new Guid("43a95e77-79e6-4da1-a4ea-2d64f3587b32"), OriginEnvironment = "prod", Name = "Propel", Description = "Energinet system Propel", ContactInfo = "propel@energinet.dk", SourceType = SourceType.Internal },
                new DataSource { Id = new Guid("44bbd193-dd57-4221-89ac-80e3509a0c73"), OriginEnvironment = "prod", Name = "Neptun", Description = "Energinet system Neptun", ContactInfo = "neptun@energinet.dk", SourceType = SourceType.Internal },
                new DataSource { Id = new Guid("bec46f78-9ef7-49f1-b98b-748f85aa823b"), OriginEnvironment = "prod", Name = "DMI", Description = "Danish Meteorological Institute", ContactInfo = "dmi.dk/kontakt", SourceType = SourceType.External }
            );

            ////Seed datasets
            //modelBuilder.Entity<Dataset>().HasData(
            //    //Propel
            //    new { Id = new Guid("913cd17d-f378-49bc-b96b-0bf28b31712d"), HierarchyId = new Guid("a9a780d7-8f2c-4d86-b863-1327bd264575"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "N-1 Branch", Location = "electricity/nminusone-calculations/branch" },
            //    new { Id = new Guid("38774081-f7d3-4c79-b2f3-1580d86c3bf5"), HierarchyId = new Guid("a9a780d7-8f2c-4d86-b863-1327bd264575"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "N-1 Bus", Location = "electricity/nminusone-calculations/bus" },
            //    new { Id = new Guid("a8ba149d-15ff-45bb-b910-cdb639d40a20"), HierarchyId = new Guid("a9a780d7-8f2c-4d86-b863-1327bd264575"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "N-1 Gen", Location = "electricity/nminusone-calculations/gen" },
            //    new { Id = new Guid("768890fc-cd26-4d53-a2ef-0dd51a5fc821"), HierarchyId = new Guid("a9a780d7-8f2c-4d86-b863-1327bd264575"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "N-1 Load", Location = "electricity/nminusone-calculations/load" },
            //    new { Id = new Guid("04476b3c-fa63-4034-bc19-3808c07fb0b4"), HierarchyId = new Guid("a9a780d7-8f2c-4d86-b863-1327bd264575"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "N-1 Shunt", Location = "electricity/nminusone-calculations/shunt" },
            //    //DPS
            //    new { Id = new Guid("ec93f72c-2693-44ed-ab8c-1199ce7051b6"), HierarchyId = new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "mFRR CM Needs", Location = "electricity/reserves/mfrr-cm-needs" },
            //    new { Id = new Guid("d396f455-310e-4221-b2ec-c4666d4fa572"), HierarchyId = new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "mFRR CM Bids", Location = "electricity/reserves/mfrr-cm-bids" },
            //    new { Id = new Guid("a949bcc8-d8f7-4aec-870b-5a10c2808b3b"), HierarchyId = new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "mFRR CM Results", Location = "electricity/reserves/mfrr-cm-results" },
            //    new { Id = new Guid("d05be7e3-fa3a-4124-aed8-a8009fe466c5"), HierarchyId = new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "FCR CM Needs", Location = "electricity/reserves/fcr-cm-needs" },
            //    new { Id = new Guid("23949217-0f4b-425b-94b9-e80725054b11"), HierarchyId = new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "FCR CM Bids", Location = "electricity/reserves/fcr-cm-bids" },
            //    new { Id = new Guid("bdd2c5a5-720e-4e1d-8cfc-16ecfe924b5e"), HierarchyId = new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "FCR CM Results", Location = "electricity/reserves/fcr-cm-results" },
            //    new { Id = new Guid("538e9138-f2c9-4c92-bca6-dc33d05d9419"), HierarchyId = new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "mFRR EAM Bids", Location = "electricity/reserves/mfrr-eam-bids" },
            //    new { Id = new Guid("9b181dcf-1062-4906-9efd-ebabd73e18b1"), HierarchyId = new Guid("ecce1213-9886-4361-bbc2-12ca79138f0e"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "mFRR EAM Activations", Location = "electricity/reserves/mfrr-eam-activations" },
            //    new { Id = new Guid("e287af45-a2fe-4210-9634-05fa7a160077"), HierarchyId = new Guid("c5dfc671-dcb3-4f10-bf11-758c1d3e97be"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "Interconnector Capacities Day Ahead", Location = "electricity/capacities/interconnector-day-ahead" },
            //    new { Id = new Guid("4556afbe-a7ee-4e02-bebd-9f3cbbb6f7e4"), HierarchyId = new Guid("c5dfc671-dcb3-4f10-bf11-758c1d3e97be"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "Interconnector Capacities Intraday", Location = "electricity/capacities/interconnector-intraday" },
            //    new { Id = new Guid("3c19ee43-3b3a-43bd-9b0d-101436bad4d7"), HierarchyId = new Guid("c5dfc671-dcb3-4f10-bf11-758c1d3e97be"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "Interconnector Capacities General", Location = "electricity/capacities/interconnector-general" },
            //    new { Id = new Guid("7c10db2a-ed65-4ff5-b9c7-8aba184941ca"), HierarchyId = new Guid("fc20e468-5ab4-48db-8a0b-09e86935c3aa"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "Renewable Energy Forecasts", Location = "electricity/forecasts/renewable-energy" },
            //    //Neptun
            //    new { Id = new Guid("0d43e440-5306-4cc9-94f4-37559cf304e0"), HierarchyId = new Guid("334740dd-7158-475d-84e9-c8e83403b515"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "hlnt_archive_3min", Location = "gas/neptun/hlnt_archive_3min" },
            //    new { Id = new Guid("b946363a-1b9c-4f59-b8dd-35c93f42c9ab"), HierarchyId = new Guid("334740dd-7158-475d-84e9-c8e83403b515"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "hlnt_archive_1hour", Location = "gas/neptun/hlnt_archive_1hour" },
            //    new { Id = new Guid("ed32e705-65ea-4cf8-be34-3a5df2e568d4"), HierarchyId = new Guid("334740dd-7158-475d-84e9-c8e83403b515"), ContactId = new Guid("1d51b693-b7b1-436d-896b-59c5d7b9d062"), Name = "hlnt_archive_1day", Location = "gas/neptun/hlnt_archive_1day" }
            //);

            ////Seed data contracts
            //modelBuilder.Entity<DataContract>().HasData(
            //    //Propel
            //    new DataContract { Id = new Guid("3fb7650e-4f4d-4033-8746-618e28f06cc6"), DatasetId = new Guid("913cd17d-f378-49bc-b96b-0bf28b31712d"), DataSourceId = new Guid("43a95e77-79e6-4da1-a4ea-2d64f3587b32") },
            //    new DataContract { Id = new Guid("3700071e-0899-4161-a730-36b21cdd8660"), DatasetId = new Guid("38774081-f7d3-4c79-b2f3-1580d86c3bf5"), DataSourceId = new Guid("43a95e77-79e6-4da1-a4ea-2d64f3587b32") },
            //    new DataContract { Id = new Guid("9f609764-32cf-426a-8943-4d7b3ef2469b"), DatasetId = new Guid("a8ba149d-15ff-45bb-b910-cdb639d40a20"), DataSourceId = new Guid("43a95e77-79e6-4da1-a4ea-2d64f3587b32") },
            //    new DataContract { Id = new Guid("51b1f28e-5406-4c4d-ba3c-161f8ecb4010"), DatasetId = new Guid("a8ba149d-15ff-45bb-b910-cdb639d40a20"), DataSourceId = new Guid("43a95e77-79e6-4da1-a4ea-2d64f3587b32") },
            //    new DataContract { Id = new Guid("e607cc4f-ddbb-465c-b041-047c065e898b"), DatasetId = new Guid("a8ba149d-15ff-45bb-b910-cdb639d40a20"), DataSourceId = new Guid("43a95e77-79e6-4da1-a4ea-2d64f3587b32") },
            //    //DPS
            //    new DataContract { Id = new Guid("a9d1aadd-9915-4884-9659-40d0cc63e410"), DatasetId = new Guid("ec93f72c-2693-44ed-ab8c-1199ce7051b6"), DataSourceId = new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab") },
            //    new DataContract { Id = new Guid("922142e8-07cc-468b-8b59-e8b93981a1e2"), DatasetId = new Guid("d396f455-310e-4221-b2ec-c4666d4fa572"), DataSourceId = new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab") },
            //    new DataContract { Id = new Guid("3f34b30d-93e8-4505-ad03-9362714e2113"), DatasetId = new Guid("a949bcc8-d8f7-4aec-870b-5a10c2808b3b"), DataSourceId = new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab") },
            //    new DataContract { Id = new Guid("81f8160d-7ac4-4e26-99da-e5188f0bee4a"), DatasetId = new Guid("d05be7e3-fa3a-4124-aed8-a8009fe466c5"), DataSourceId = new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab") },
            //    new DataContract { Id = new Guid("cf70172d-c271-4607-a04e-a17b34044d6b"), DatasetId = new Guid("23949217-0f4b-425b-94b9-e80725054b11"), DataSourceId = new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab") },
            //    new DataContract { Id = new Guid("34ab15a6-0bb1-40b0-9407-3971f41a0c68"), DatasetId = new Guid("bdd2c5a5-720e-4e1d-8cfc-16ecfe924b5e"), DataSourceId = new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab") },
            //    new DataContract { Id = new Guid("ea425ed9-7130-48cb-8464-edb4cbb5ebc2"), DatasetId = new Guid("538e9138-f2c9-4c92-bca6-dc33d05d9419"), DataSourceId = new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab") },
            //    new DataContract { Id = new Guid("7a534e40-97f3-4d6c-aff5-b443ba06b13e"), DatasetId = new Guid("9b181dcf-1062-4906-9efd-ebabd73e18b1"), DataSourceId = new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab") },
            //    new DataContract { Id = new Guid("ddacd6e0-4a26-4793-8c14-8bc843315963"), DatasetId = new Guid("e287af45-a2fe-4210-9634-05fa7a160077"), DataSourceId = new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab") },
            //    new DataContract { Id = new Guid("f1e409c1-81d0-4c2c-97a3-bfd8bbe22aeb"), DatasetId = new Guid("4556afbe-a7ee-4e02-bebd-9f3cbbb6f7e4"), DataSourceId = new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab") },
            //    new DataContract { Id = new Guid("899c4fef-892e-4a91-8191-42e7f970f58b"), DatasetId = new Guid("3c19ee43-3b3a-43bd-9b0d-101436bad4d7"), DataSourceId = new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab") },
            //    new DataContract { Id = new Guid("21389a17-bf24-4be8-ab38-5d8f8a012488"), DatasetId = new Guid("7c10db2a-ed65-4ff5-b9c7-8aba184941ca"), DataSourceId = new Guid("3db55181-4eee-4601-9d52-8aa69e2562ab") },
            //    //Neptun
            //    new DataContract { Id = new Guid("c7f1cb06-2441-4c3f-b4e2-d02cf4cc6b56"), DatasetId = new Guid("0d43e440-5306-4cc9-94f4-37559cf304e0"), DataSourceId = new Guid("44bbd193-dd57-4221-89ac-80e3509a0c73") },
            //    new DataContract { Id = new Guid("e52f8eb1-9ae8-40e1-acc2-052ec8e578ff"), DatasetId = new Guid("b946363a-1b9c-4f59-b8dd-35c93f42c9ab"), DataSourceId = new Guid("44bbd193-dd57-4221-89ac-80e3509a0c73") },
            //    new DataContract { Id = new Guid("b9e55105-16d2-4ecf-9633-1c221d5abc82"), DatasetId = new Guid("ed32e705-65ea-4cf8-be34-3a5df2e568d4"), DataSourceId = new Guid("44bbd193-dd57-4221-89ac-80e3509a0c73") }
            //);
        }
    }
}