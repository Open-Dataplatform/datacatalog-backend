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
    [Migration("20200325145223_SeedCategories")]
    partial class SeedCategories
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
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Category");

                    b.HasData(
                        new
                        {
                            Id = new Guid("8713b259-0294-480a-960c-08a9c9983961"),
                            Colour = "#2A939B",
                            CreatedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(4390),
                            ModifiedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(3411),
                            Name = "Ancillary Services"
                        },
                        new
                        {
                            Id = new Guid("b94863a9-685a-44d6-aae4-2cfb3a63c489"),
                            Colour = "#389B88",
                            CreatedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6851),
                            ModifiedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6844),
                            Name = "Auctions, Transmission Capacity"
                        },
                        new
                        {
                            Id = new Guid("e5b2f760-e965-452f-b0ab-70ca506d4c20"),
                            Colour = "#452A9B",
                            CreatedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6893),
                            ModifiedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6893),
                            Name = "Emissions"
                        },
                        new
                        {
                            Id = new Guid("08e58f12-51bf-4af9-af3a-02022cab77c7"),
                            Colour = "#2A9B65",
                            CreatedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6900),
                            ModifiedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6900),
                            Name = "Day Ahead Market"
                        },
                        new
                        {
                            Id = new Guid("23ab1cd5-ae16-4aa4-bab7-25380c1634d5"),
                            Colour = "#B27736",
                            CreatedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6904),
                            ModifiedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6904),
                            Name = "Electric Boilers"
                        },
                        new
                        {
                            Id = new Guid("8a95d290-417e-4a53-a807-a13293f3117d"),
                            Colour = "#F8AE3C",
                            CreatedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6911),
                            ModifiedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6911),
                            Name = "Gas"
                        },
                        new
                        {
                            Id = new Guid("5873b2e5-5db1-42db-bf25-83fd9dded293"),
                            Colour = "#663BCC",
                            CreatedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6939),
                            ModifiedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6939),
                            Name = "Production and Consumption"
                        },
                        new
                        {
                            Id = new Guid("084e6353-82a8-4f99-b9f3-b247081c34c8"),
                            Colour = "#A0C1C2",
                            CreatedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6943),
                            ModifiedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6943),
                            Name = "Electricity Consumption"
                        },
                        new
                        {
                            Id = new Guid("5b39fbdd-76c5-44df-9f7d-d314db760fc1"),
                            Colour = "#819B38",
                            CreatedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6950),
                            ModifiedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6950),
                            Name = "Intra Day Market"
                        },
                        new
                        {
                            Id = new Guid("a90abc39-c2de-4c29-a9f8-4e10f41a50f0"),
                            Colour = "#293A4C",
                            CreatedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6953),
                            ModifiedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6953),
                            Name = "Electricity Production"
                        },
                        new
                        {
                            Id = new Guid("9ac5af8d-4165-448a-ace5-2edcf0c3e202"),
                            Colour = "#548E80",
                            CreatedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6957),
                            ModifiedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6957),
                            Name = "Regulating Power"
                        },
                        new
                        {
                            Id = new Guid("7434d8a3-7ebb-4212-967c-061d5a2666d7"),
                            Colour = "#7D8E1C",
                            CreatedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6960),
                            ModifiedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6960),
                            Name = "Whole Sale Market"
                        },
                        new
                        {
                            Id = new Guid("90909fcd-4a5a-455a-a543-9c2c8a45e453"),
                            Colour = "#398C22",
                            CreatedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6964),
                            ModifiedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6964),
                            Name = "Reserves"
                        },
                        new
                        {
                            Id = new Guid("d6944781-c069-4c14-859f-07865164763f"),
                            Colour = "#FFD424",
                            CreatedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6967),
                            ModifiedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6967),
                            Name = "Solar Power"
                        },
                        new
                        {
                            Id = new Guid("b4cef11a-d0f4-4c33-9796-b80ea6d2489a"),
                            Colour = "#547B8E",
                            CreatedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6974),
                            ModifiedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6974),
                            Name = "Transmission Lines"
                        },
                        new
                        {
                            Id = new Guid("418e657d-3efb-4a30-ad3f-3f95a6ab8888"),
                            Colour = "#C2E4F0",
                            CreatedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6978),
                            ModifiedDate = new DateTime(2020, 3, 25, 14, 52, 22, 755, DateTimeKind.Utc).AddTicks(6978),
                            Name = "Wind Power"
                        });
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.DataField", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Access")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DatasetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Format")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Validation")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

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
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Frequency")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FrequencyDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Resolution")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResolutionDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SLA")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Source")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Version")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("VersionDate")
                        .HasColumnType("datetime2");

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
                        .HasColumnType("datetime2");

                    b.HasKey("DatasetId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("DatasetCategory");
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.Member", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Member");
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.MemberGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
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
                        .HasColumnType("datetime2");

                    b.HasKey("MemberGroupId", "MemberId");

                    b.HasIndex("MemberId");

                    b.ToTable("MemberGroupMember");
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.Transformation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ShortDescription")
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
                        .HasColumnType("datetime2");

                    b.Property<int>("TransformationDirection")
                        .HasColumnType("int");

                    b.HasKey("DatasetId", "TransformationId");

                    b.HasIndex("TransformationId");

                    b.ToTable("TransformationDataset");
                });

            modelBuilder.Entity("DataCatalog.Api.Data.Model.DataField", b =>
                {
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
