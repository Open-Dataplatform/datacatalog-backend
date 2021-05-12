﻿// <auto-generated />
using System;
using DataCatalog.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataCatalog.Api.Data.Migrations
{
    [DbContext(typeof(DataCatalogContext))]
    [Migration("20200401115146_Update")]
    partial class Update
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DataCatalog.Api.Data.Model.Category", b =>
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

                    b.HasKey("Id");

                    b.ToTable("Category");

                    b.HasData(
                        new
                        {
                            Id = new Guid("8713b259-0294-480a-960c-08a9c9983961"),
                            Colour = "#2A939B",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Ancillary Services"
                        },
                        new
                        {
                            Id = new Guid("b94863a9-685a-44d6-aae4-2cfb3a63c489"),
                            Colour = "#389B88",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Auctions, Transmission Capacity"
                        },
                        new
                        {
                            Id = new Guid("e5b2f760-e965-452f-b0ab-70ca506d4c20"),
                            Colour = "#452A9B",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Emissions"
                        },
                        new
                        {
                            Id = new Guid("08e58f12-51bf-4af9-af3a-02022cab77c7"),
                            Colour = "#2A9B65",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Day Ahead Market"
                        },
                        new
                        {
                            Id = new Guid("23ab1cd5-ae16-4aa4-bab7-25380c1634d5"),
                            Colour = "#B27736",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Electric Boilers"
                        },
                        new
                        {
                            Id = new Guid("8a95d290-417e-4a53-a807-a13293f3117d"),
                            Colour = "#F8AE3C",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Gas"
                        },
                        new
                        {
                            Id = new Guid("5873b2e5-5db1-42db-bf25-83fd9dded293"),
                            Colour = "#663BCC",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Production and Consumption"
                        },
                        new
                        {
                            Id = new Guid("084e6353-82a8-4f99-b9f3-b247081c34c8"),
                            Colour = "#A0C1C2",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Electricity Consumption"
                        },
                        new
                        {
                            Id = new Guid("5b39fbdd-76c5-44df-9f7d-d314db760fc1"),
                            Colour = "#819B38",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Intra Day Market"
                        },
                        new
                        {
                            Id = new Guid("a90abc39-c2de-4c29-a9f8-4e10f41a50f0"),
                            Colour = "#293A4C",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Electricity Production"
                        },
                        new
                        {
                            Id = new Guid("9ac5af8d-4165-448a-ace5-2edcf0c3e202"),
                            Colour = "#548E80",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Regulating Power"
                        },
                        new
                        {
                            Id = new Guid("7434d8a3-7ebb-4212-967c-061d5a2666d7"),
                            Colour = "#7D8E1C",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Whole Sale Market"
                        },
                        new
                        {
                            Id = new Guid("90909fcd-4a5a-455a-a543-9c2c8a45e453"),
                            Colour = "#398C22",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Reserves"
                        },
                        new
                        {
                            Id = new Guid("d6944781-c069-4c14-859f-07865164763f"),
                            Colour = "#FFD424",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Solar Power"
                        },
                        new
                        {
                            Id = new Guid("b4cef11a-d0f4-4c33-9796-b80ea6d2489a"),
                            Colour = "#547B8E",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Transmission Lines"
                        },
                        new
                        {
                            Id = new Guid("418e657d-3efb-4a30-ad3f-3f95a6ab8888"),
                            Colour = "#C2E4F0",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Wind Power"
                        });
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.Confidentiality", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Confidentiality");

                    b.HasData(
                        new
                        {
                            Id = new Guid("62c544b2-ee11-4434-b9b7-39ffb1ab9616"),
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "Freely available",
                            Level = 0,
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Nonconfidential"
                        },
                        new
                        {
                            Id = new Guid("5cf6fe55-13f4-4503-8ae0-394f6e09bf3d"),
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "Sensitive, but available for work-related tasks",
                            Level = 1,
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Confidential"
                        },
                        new
                        {
                            Id = new Guid("568f7357-fce8-4733-b15d-2e0c4c3d8bd6"),
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "Only available with special clearance",
                            Level = 2,
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Strictly confidential"
                        });
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.DataField", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ConfidentialityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValue(new Guid("62c544b2-ee11-4434-b9b7-39ffb1ab9616"));

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

                    b.HasIndex("ConfidentialityId");

                    b.HasIndex("DatasetId");

                    b.ToTable("DataField");
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.Dataset", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContactId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsArchived")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SlaDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SlaLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SourceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValue(new Guid("5777efb5-f55c-4f16-bb83-0125b810f995"));

                    b.HasKey("Id");

                    b.HasIndex("ContactId");

                    b.ToTable("Dataset");
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.DatasetCategory", b =>
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

            modelBuilder.Entity("DataCatalog.Api.Data.Model.DatasetChangeLog", b =>
                {
                    b.Property<Guid>("DatasetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MemberId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.HasKey("DatasetId", "MemberId");

                    b.HasIndex("MemberId");

                    b.ToTable("DatasetChangeLog");
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.DatasetDuration", b =>
                {
                    b.Property<Guid>("DatasetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DurationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<int>("DurationType")
                        .HasColumnType("int");

                    b.HasKey("DatasetId", "DurationId");

                    b.HasIndex("DurationId");

                    b.ToTable("DatasetDuration");
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.DatasetGroup", b =>
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

            modelBuilder.Entity("DataCatalog.Api.Data.Model.DatasetGroupDataset", b =>
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

            modelBuilder.Entity("DataCatalog.Api.Data.Model.Duration", b =>
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

                    b.HasData(
                        new
                        {
                            Id = new Guid("f745f308-1542-4a7a-975f-0cd6f1e73668"),
                            Code = "P1H",
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "Every hour",
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        });
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.Member", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("MemberRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValue(new Guid("f611f697-4deb-4cc6-901d-41dd76346359"));

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MemberRoleId");

                    b.ToTable("Member");

                    b.HasData(
                        new
                        {
                            Id = new Guid("7704274c-1a92-4d39-bc56-35ddd0e3ea7f"),
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "dummy@fake.com",
                            MemberRoleId = new Guid("2375e551-d07a-4417-8357-6d8784474273"),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Dummy Steward"
                        });
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.MemberGroup", b =>
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

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("MemberGroup");
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.MemberGroupMember", b =>
                {
                    b.Property<Guid>("MemberGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MemberId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.HasKey("MemberGroupId", "MemberId");

                    b.HasIndex("MemberId");

                    b.ToTable("MemberGroupMember");
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.MemberRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getutcdate()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("MemberRole");

                    b.HasData(
                        new
                        {
                            Id = new Guid("a32b86d0-1646-4ca7-b79e-e205b0d15868"),
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Admin"
                        },
                        new
                        {
                            Id = new Guid("2375e551-d07a-4417-8357-6d8784474273"),
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Data steward"
                        },
                        new
                        {
                            Id = new Guid("f611f697-4deb-4cc6-901d-41dd76346359"),
                            CreatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ModifiedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "User"
                        });
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.Transformation", b =>
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

            modelBuilder.Entity("DataCatalog.Api.Data.Model.TransformationDataset", b =>
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

            modelBuilder.Entity("DataCatalog.Api.Data.Model.DataField", b =>
                {
                    b.HasOne("DataCatalog.Api.Data.Model.Confidentiality", "Confidentiality")
                        .WithMany("DataFields")
                        .HasForeignKey("ConfidentialityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataCatalog.Api.Data.Model.Dataset", "Dataset")
                        .WithMany("DataFields")
                        .HasForeignKey("DatasetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.Dataset", b =>
                {
                    b.HasOne("DataCatalog.Api.Data.Model.MemberGroup", "Contact")
                        .WithMany("Datasets")
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.DatasetCategory", b =>
                {
                    b.HasOne("DataCatalog.Api.Data.Model.Category", "Category")
                        .WithMany("DatasetCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataCatalog.Api.Data.Model.Dataset", "Dataset")
                        .WithMany("DatasetCategories")
                        .HasForeignKey("DatasetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.DatasetChangeLog", b =>
                {
                    b.HasOne("DataCatalog.Api.Data.Model.Dataset", "Dataset")
                        .WithMany("DatasetChangeLogs")
                        .HasForeignKey("DatasetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataCatalog.Api.Data.Model.Member", "Member")
                        .WithMany("DatasetChangeLogs")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.DatasetDuration", b =>
                {
                    b.HasOne("DataCatalog.Api.Data.Model.Dataset", "Dataset")
                        .WithMany("DatasetDurations")
                        .HasForeignKey("DatasetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataCatalog.Api.Data.Model.Duration", "Duration")
                        .WithMany("DatasetsDurations")
                        .HasForeignKey("DurationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.DatasetGroup", b =>
                {
                    b.HasOne("DataCatalog.Api.Data.Model.Member", "Member")
                        .WithMany()
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.DatasetGroupDataset", b =>
                {
                    b.HasOne("DataCatalog.Api.Data.Model.DatasetGroup", "DatasetGroup")
                        .WithMany("DatasetGroupDatasets")
                        .HasForeignKey("DatasetGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataCatalog.Api.Data.Model.Dataset", "Dataset")
                        .WithMany()
                        .HasForeignKey("DatasetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.Member", b =>
                {
                    b.HasOne("DataCatalog.Api.Data.Model.MemberRole", "MemberRole")
                        .WithMany("Members")
                        .HasForeignKey("MemberRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.MemberGroupMember", b =>
                {
                    b.HasOne("DataCatalog.Api.Data.Model.MemberGroup", "MemberGroup")
                        .WithMany("MemberGroupMembers")
                        .HasForeignKey("MemberGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataCatalog.Api.Data.Model.Member", "Member")
                        .WithMany("MemberGroupMembers")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.TransformationDataset", b =>
                {
                    b.HasOne("DataCatalog.Api.Data.Model.Dataset", "Dataset")
                        .WithMany("TransformationDatasets")
                        .HasForeignKey("DatasetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataCatalog.Api.Data.Model.Transformation", "Transformation")
                        .WithMany("TransformationDatasets")
                        .HasForeignKey("TransformationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
