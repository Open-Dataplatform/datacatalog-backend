﻿// <auto-generated />
using System;
using DataCatalog.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataCatalog.Data.Migrations
{
    [DbContext(typeof(DataCatalogContext))]
    [Migration("20210920092744_RemoveContact")]
    partial class RemoveContact
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.9")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DataCatalog.Data.Model.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Colour")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("ImageUri")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("OriginDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("OriginEnvironment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.DataContract", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<Guid>("DataSourceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DatasetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<bool>("OriginDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("OriginEnvironment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DataSourceId");

                    b.HasIndex("DatasetId");

                    b.ToTable("DataContract");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.DataField", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<Guid>("DatasetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Format")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Validation")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DatasetId");

                    b.ToTable("DataField");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.DataSource", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ContactInfo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("OriginDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("OriginEnvironment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SourceType")
                        .HasColumnType("int");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("DataSource");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.Dataset", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Confidentiality")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("HierarchyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("OriginDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("OriginEnvironment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Owner")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ProvisionStatus")
                        .HasColumnType("int");

                    b.Property<int>("RefinementLevel")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("SlaDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SlaLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SourceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<int>("Version")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.HasKey("Id");

                    b.HasIndex("HierarchyId");

                    b.ToTable("Dataset");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.DatasetCategory", b =>
                {
                    b.Property<Guid>("DatasetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.HasKey("DatasetId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("DatasetCategory");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.DatasetChangeLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<Guid>("DatasetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("MemberId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DatasetId");

                    b.HasIndex("MemberId");

                    b.ToTable("DatasetChangeLog");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.DatasetDuration", b =>
                {
                    b.Property<Guid>("DatasetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DurationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DurationType")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.HasKey("DatasetId", "DurationId", "DurationType");

                    b.HasIndex("DurationId");

                    b.ToTable("DatasetDuration");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.DatasetGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("MemberId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.ToTable("DatasetGroup");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.DatasetGroupDataset", b =>
                {
                    b.Property<Guid>("DatasetGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DatasetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.HasKey("DatasetGroupId", "DatasetId");

                    b.HasIndex("DatasetId");

                    b.ToTable("DatasetGroupDataset");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.Duration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.HasKey("Id");

                    b.ToTable("Duration");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.Hierarchy", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ParentHierarchyId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ParentHierarchyId");

                    b.ToTable("Hierarchy");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.IdentityProvider", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TenantId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("IdentityProvider");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.Member", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("IdentityProviderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.HasKey("Id");

                    b.HasIndex("IdentityProviderId");

                    b.ToTable("Member");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.Transformation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("ShortDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Transformation");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.TransformationDataset", b =>
                {
                    b.Property<Guid>("DatasetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TransformationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<int>("TransformationDirection")
                        .HasColumnType("int");

                    b.HasKey("DatasetId", "TransformationId");

                    b.HasIndex("TransformationId");

                    b.ToTable("TransformationDataset");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.DataContract", b =>
                {
                    b.HasOne("DataCatalog.Data.Model.DataSource", "DataSource")
                        .WithMany("DataContracts")
                        .HasForeignKey("DataSourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataCatalog.Data.Model.Dataset", "Dataset")
                        .WithMany("DataContracts")
                        .HasForeignKey("DatasetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Dataset");

                    b.Navigation("DataSource");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.DataField", b =>
                {
                    b.HasOne("DataCatalog.Data.Model.Dataset", "Dataset")
                        .WithMany("DataFields")
                        .HasForeignKey("DatasetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Dataset");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.Dataset", b =>
                {
                    b.HasOne("DataCatalog.Data.Model.Hierarchy", "Hierarchy")
                        .WithMany("Datasets")
                        .HasForeignKey("HierarchyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hierarchy");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.DatasetCategory", b =>
                {
                    b.HasOne("DataCatalog.Data.Model.Category", "Category")
                        .WithMany("DatasetCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataCatalog.Data.Model.Dataset", "Dataset")
                        .WithMany("DatasetCategories")
                        .HasForeignKey("DatasetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Dataset");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.DatasetChangeLog", b =>
                {
                    b.HasOne("DataCatalog.Data.Model.Dataset", "Dataset")
                        .WithMany("DatasetChangeLogs")
                        .HasForeignKey("DatasetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataCatalog.Data.Model.Member", "Member")
                        .WithMany("DatasetChangeLogs")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Dataset");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.DatasetDuration", b =>
                {
                    b.HasOne("DataCatalog.Data.Model.Dataset", "Dataset")
                        .WithMany("DatasetDurations")
                        .HasForeignKey("DatasetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataCatalog.Data.Model.Duration", "Duration")
                        .WithMany("DatasetsDurations")
                        .HasForeignKey("DurationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Dataset");

                    b.Navigation("Duration");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.DatasetGroup", b =>
                {
                    b.HasOne("DataCatalog.Data.Model.Member", "Member")
                        .WithMany()
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Member");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.DatasetGroupDataset", b =>
                {
                    b.HasOne("DataCatalog.Data.Model.DatasetGroup", "DatasetGroup")
                        .WithMany("DatasetGroupDatasets")
                        .HasForeignKey("DatasetGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataCatalog.Data.Model.Dataset", "Dataset")
                        .WithMany()
                        .HasForeignKey("DatasetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Dataset");

                    b.Navigation("DatasetGroup");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.Hierarchy", b =>
                {
                    b.HasOne("DataCatalog.Data.Model.Hierarchy", "ParentHierarchy")
                        .WithMany("ChildHierarchies")
                        .HasForeignKey("ParentHierarchyId");

                    b.Navigation("ParentHierarchy");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.Member", b =>
                {
                    b.HasOne("DataCatalog.Data.Model.IdentityProvider", "IdentityProvider")
                        .WithMany()
                        .HasForeignKey("IdentityProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("IdentityProvider");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.TransformationDataset", b =>
                {
                    b.HasOne("DataCatalog.Data.Model.Dataset", "Dataset")
                        .WithMany("TransformationDatasets")
                        .HasForeignKey("DatasetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataCatalog.Data.Model.Transformation", "Transformation")
                        .WithMany("TransformationDatasets")
                        .HasForeignKey("TransformationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Dataset");

                    b.Navigation("Transformation");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.Category", b =>
                {
                    b.Navigation("DatasetCategories");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.DataSource", b =>
                {
                    b.Navigation("DataContracts");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.Dataset", b =>
                {
                    b.Navigation("DataContracts");

                    b.Navigation("DataFields");

                    b.Navigation("DatasetCategories");

                    b.Navigation("DatasetChangeLogs");

                    b.Navigation("DatasetDurations");

                    b.Navigation("TransformationDatasets");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.DatasetGroup", b =>
                {
                    b.Navigation("DatasetGroupDatasets");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.Duration", b =>
                {
                    b.Navigation("DatasetsDurations");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.Hierarchy", b =>
                {
                    b.Navigation("ChildHierarchies");

                    b.Navigation("Datasets");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.Member", b =>
                {
                    b.Navigation("DatasetChangeLogs");
                });

            modelBuilder.Entity("DataCatalog.Data.Model.Transformation", b =>
                {
                    b.Navigation("TransformationDatasets");
                });
#pragma warning restore 612, 618
        }
    }
}
