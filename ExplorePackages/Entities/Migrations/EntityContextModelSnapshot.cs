﻿// <auto-generated />
using Knapcode.ExplorePackages.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace Knapcode.ExplorePackages.Entities.Migrations
{
    [DbContext(typeof(EntityContext))]
    partial class EntityContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.CatalogPackageEntity", b =>
                {
                    b.Property<long>("PackageKey");

                    b.Property<bool>("Deleted");

                    b.Property<long>("FirstCommitTimestamp");

                    b.Property<long>("LastCommitTimestamp");

                    b.HasKey("PackageKey");

                    b.HasIndex("LastCommitTimestamp");

                    b.ToTable("CatalogPackages");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.CursorEntity", b =>
                {
                    b.Property<long>("CursorKey")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<long>("Value");

                    b.HasKey("CursorKey");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Cursors");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.ETagEntity", b =>
                {
                    b.Property<long>("ETagKey")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Value");

                    b.HasKey("ETagKey");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("ETags");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.PackageArchiveEntity", b =>
                {
                    b.Property<long>("PackageKey");

                    b.Property<long>("EntryCount");

                    b.Property<long>("Size");

                    b.HasKey("PackageKey");

                    b.ToTable("PackageArchives");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.PackageDownloadsEntity", b =>
                {
                    b.Property<long>("PackageKey");

                    b.Property<long>("Downloads");

                    b.HasKey("PackageKey");

                    b.ToTable("PackageDownloads");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.PackageEntity", b =>
                {
                    b.Property<long>("PackageKey")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Identity")
                        .IsRequired()
                        .HasColumnType("TEXT COLLATE NOCASE");

                    b.Property<long>("PackageRegistrationKey");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("TEXT COLLATE NOCASE");

                    b.HasKey("PackageKey");

                    b.HasIndex("Identity")
                        .IsUnique();

                    b.HasIndex("PackageRegistrationKey", "Version")
                        .IsUnique();

                    b.ToTable("Packages");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.PackageQueryEntity", b =>
                {
                    b.Property<long>("PackageQueryKey")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("CursorKey");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("PackageQueryKey");

                    b.HasIndex("CursorKey");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("PackageQueries");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.PackageQueryMatchEntity", b =>
                {
                    b.Property<long>("PackageQueryMatchKey")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("PackageKey");

                    b.Property<long>("PackageQueryKey");

                    b.HasKey("PackageQueryMatchKey");

                    b.HasIndex("PackageKey");

                    b.HasIndex("PackageQueryKey", "PackageKey")
                        .IsUnique();

                    b.ToTable("PackageQueryMatches");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.PackageRegistrationEntity", b =>
                {
                    b.Property<long>("PackageRegistrationKey")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Id")
                        .IsRequired()
                        .HasColumnType("TEXT COLLATE NOCASE");

                    b.HasKey("PackageRegistrationKey");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("PackageRegistrations");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.V2PackageEntity", b =>
                {
                    b.Property<long>("PackageKey");

                    b.Property<long>("CreatedTimestamp");

                    b.HasKey("PackageKey");

                    b.HasIndex("CreatedTimestamp");

                    b.ToTable("V2Packages");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.CatalogPackageEntity", b =>
                {
                    b.HasOne("Knapcode.ExplorePackages.Entities.PackageEntity", "Package")
                        .WithOne("CatalogPackage")
                        .HasForeignKey("Knapcode.ExplorePackages.Entities.CatalogPackageEntity", "PackageKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.PackageArchiveEntity", b =>
                {
                    b.HasOne("Knapcode.ExplorePackages.Entities.PackageEntity", "Package")
                        .WithOne("PackageArchive")
                        .HasForeignKey("Knapcode.ExplorePackages.Entities.PackageArchiveEntity", "PackageKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.PackageDownloadsEntity", b =>
                {
                    b.HasOne("Knapcode.ExplorePackages.Entities.PackageEntity", "Package")
                        .WithOne("PackageDownloads")
                        .HasForeignKey("Knapcode.ExplorePackages.Entities.PackageDownloadsEntity", "PackageKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.PackageEntity", b =>
                {
                    b.HasOne("Knapcode.ExplorePackages.Entities.PackageRegistrationEntity", "PackageRegistration")
                        .WithMany("Packages")
                        .HasForeignKey("PackageRegistrationKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.PackageQueryEntity", b =>
                {
                    b.HasOne("Knapcode.ExplorePackages.Entities.CursorEntity", "Cursor")
                        .WithMany()
                        .HasForeignKey("CursorKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.PackageQueryMatchEntity", b =>
                {
                    b.HasOne("Knapcode.ExplorePackages.Entities.PackageEntity", "Package")
                        .WithMany("PackageQueryMatches")
                        .HasForeignKey("PackageKey")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Knapcode.ExplorePackages.Entities.PackageQueryEntity", "PackageQuery")
                        .WithMany()
                        .HasForeignKey("PackageQueryKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.V2PackageEntity", b =>
                {
                    b.HasOne("Knapcode.ExplorePackages.Entities.PackageEntity", "Package")
                        .WithOne("V2Package")
                        .HasForeignKey("Knapcode.ExplorePackages.Entities.V2PackageEntity", "PackageKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
