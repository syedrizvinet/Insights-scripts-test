﻿// <auto-generated />
using System;
using Knapcode.ExplorePackages.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Knapcode.ExplorePackages.Entities.Migrations.SqlServer
{
    [DbContext(typeof(SqlServerEntityContext))]
    partial class SqlServerEntityContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.CatalogCommitEntity", b =>
                {
                    b.Property<long>("CatalogCommitKey")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CatalogPageKey");

                    b.Property<string>("CommitId")
                        .IsRequired();

                    b.Property<long>("CommitTimestamp");

                    b.Property<int>("Count");

                    b.HasKey("CatalogCommitKey");

                    b.HasIndex("CatalogPageKey");

                    b.HasIndex("CommitId")
                        .IsUnique();

                    b.HasIndex("CommitTimestamp")
                        .IsUnique();

                    b.ToTable("CatalogCommits");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.CatalogLeafEntity", b =>
                {
                    b.Property<long>("CatalogLeafKey")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CatalogCommitKey");

                    b.Property<bool>("IsListed");

                    b.Property<long>("PackageKey");

                    b.Property<string>("RelativePath");

                    b.Property<int?>("SemVerType");

                    b.Property<int>("Type");

                    b.HasKey("CatalogLeafKey");

                    b.HasIndex("CatalogCommitKey");

                    b.HasIndex("PackageKey");

                    b.ToTable("CatalogLeaves");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.CatalogPackageEntity", b =>
                {
                    b.Property<long>("PackageKey");

                    b.Property<bool>("Deleted");

                    b.Property<long>("FirstCommitTimestamp");

                    b.Property<long>("LastCommitTimestamp");

                    b.Property<bool>("Listed");

                    b.Property<int?>("SemVerType");

                    b.HasKey("PackageKey");

                    b.HasIndex("LastCommitTimestamp")
                        .HasAnnotation("SqlServer:Include", new[] { "Deleted", "FirstCommitTimestamp", "Listed", "SemVerType" });

                    b.ToTable("CatalogPackages");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.CatalogPackageRegistrationEntity", b =>
                {
                    b.Property<long>("PackageRegistrationKey");

                    b.Property<long>("FirstCommitTimestamp");

                    b.Property<long>("LastCommitTimestamp");

                    b.HasKey("PackageRegistrationKey");

                    b.HasIndex("LastCommitTimestamp")
                        .HasAnnotation("SqlServer:Include", new[] { "FirstCommitTimestamp" });

                    b.ToTable("CatalogPackageRegistrations");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.CatalogPageEntity", b =>
                {
                    b.Property<long>("CatalogPageKey")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Url")
                        .IsRequired();

                    b.HasKey("CatalogPageKey");

                    b.HasIndex("Url")
                        .IsUnique();

                    b.ToTable("CatalogPages");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.CommitCollectorProgressTokenEntity", b =>
                {
                    b.Property<long>("CommitCollectorProgressTokenKey")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("FirstCommitTimestamp");

                    b.Property<long>("LastCommitTimestamp");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("SerializedProgressToken");

                    b.HasKey("CommitCollectorProgressTokenKey");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("CommitCollectorProgressTokens");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.CursorEntity", b =>
                {
                    b.Property<long>("CursorKey")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Value");

                    b.HasKey("ETagKey");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("ETags");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.FrameworkEntity", b =>
                {
                    b.Property<long>("FrameworkKey")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("OriginalValue")
                        .IsRequired();

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("FrameworkKey");

                    b.HasIndex("OriginalValue")
                        .IsUnique();

                    b.ToTable("Frameworks");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.LeaseEntity", b =>
                {
                    b.Property<long>("LeaseKey")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset?>("End");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("LeaseKey");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Leases");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.PackageArchiveEntity", b =>
                {
                    b.Property<long>("PackageKey");

                    b.Property<long>("CentralDirectorySize");

                    b.Property<byte[]>("Comment")
                        .IsRequired();

                    b.Property<int>("CommentSize");

                    b.Property<int>("DiskWithStartOfCentralDirectory");

                    b.Property<int>("EntriesForWholeCentralDirectory");

                    b.Property<int>("EntriesInThisDisk");

                    b.Property<int>("EntryCount");

                    b.Property<int>("NumberOfThisDisk");

                    b.Property<long>("OffsetAfterEndOfCentralDirectory");

                    b.Property<long>("OffsetOfCentralDirectory");

                    b.Property<long>("Size");

                    b.Property<decimal?>("Zip64CentralDirectorySize")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<long?>("Zip64DiskWithStartOfCentralDirectory");

                    b.Property<long?>("Zip64DiskWithStartOfEndOfCentralDirectory");

                    b.Property<decimal?>("Zip64EndOfCentralDirectoryOffset")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<decimal?>("Zip64EntriesForWholeCentralDirectory")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<decimal?>("Zip64EntriesInThisDisk")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<long?>("Zip64NumberOfThisDisk");

                    b.Property<long?>("Zip64OffsetAfterEndOfCentralDirectoryLocator");

                    b.Property<decimal?>("Zip64OffsetOfCentralDirectory")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<decimal?>("Zip64SizeOfCentralDirectoryRecord")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<long?>("Zip64TotalNumberOfDisks");

                    b.Property<int?>("Zip64VersionMadeBy");

                    b.Property<int?>("Zip64VersionToExtract");

                    b.HasKey("PackageKey");

                    b.ToTable("PackageArchives");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.PackageDependencyEntity", b =>
                {
                    b.Property<long>("PackageDependencyKey")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("BestDependencyPackageKey");

                    b.Property<long>("DependencyPackageRegistrationKey");

                    b.Property<long?>("FrameworkKey");

                    b.Property<long?>("MinimumDependencyPackageKey");

                    b.Property<string>("OriginalVersionRange");

                    b.Property<long>("ParentPackageKey");

                    b.Property<string>("VersionRange");

                    b.HasKey("PackageDependencyKey");

                    b.HasIndex("BestDependencyPackageKey");

                    b.HasIndex("FrameworkKey");

                    b.HasIndex("MinimumDependencyPackageKey");

                    b.HasIndex("ParentPackageKey")
                        .HasAnnotation("SqlServer:Include", new[] { "BestDependencyPackageKey", "DependencyPackageRegistrationKey", "FrameworkKey", "MinimumDependencyPackageKey", "OriginalVersionRange", "VersionRange" });

                    b.HasIndex("DependencyPackageRegistrationKey", "ParentPackageKey");

                    b.HasIndex("ParentPackageKey", "DependencyPackageRegistrationKey", "FrameworkKey")
                        .IsUnique()
                        .HasFilter("[FrameworkKey] IS NOT NULL");

                    b.ToTable("PackageDependencies");
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
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Identity")
                        .IsRequired();

                    b.Property<long>("PackageRegistrationKey");

                    b.Property<string>("Version")
                        .IsRequired();

                    b.HasKey("PackageKey");

                    b.HasIndex("Identity")
                        .IsUnique();

                    b.HasIndex("PackageRegistrationKey", "Version")
                        .IsUnique();

                    b.ToTable("Packages");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.PackageEntryEntity", b =>
                {
                    b.Property<long>("PackageEntryKey")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("Comment")
                        .IsRequired();

                    b.Property<int>("CommentSize");

                    b.Property<long>("CompressedSize");

                    b.Property<int>("CompressionMethod");

                    b.Property<long>("Crc32");

                    b.Property<int>("DiskNumberStart");

                    b.Property<long>("ExternalAttributes");

                    b.Property<byte[]>("ExtraField")
                        .IsRequired();

                    b.Property<int>("ExtraFieldSize");

                    b.Property<int>("Flags");

                    b.Property<decimal>("Index")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<int>("InternalAttributes");

                    b.Property<int>("LastModifiedDate");

                    b.Property<int>("LastModifiedTime");

                    b.Property<long>("LocalHeaderOffset");

                    b.Property<byte[]>("Name")
                        .IsRequired();

                    b.Property<int>("NameSize");

                    b.Property<long>("PackageKey");

                    b.Property<long>("UncompressedSize");

                    b.Property<int>("VersionMadeBy");

                    b.Property<int>("VersionToExtract");

                    b.HasKey("PackageEntryKey");

                    b.HasIndex("PackageKey", "Index")
                        .IsUnique();

                    b.ToTable("PackageEntries");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.PackageQueryEntity", b =>
                {
                    b.Property<long>("PackageQueryKey")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Id")
                        .IsRequired();

                    b.HasKey("PackageRegistrationKey");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("PackageRegistrations");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.V2PackageEntity", b =>
                {
                    b.Property<long>("PackageKey");

                    b.Property<long>("CreatedTimestamp");

                    b.Property<long?>("LastEditedTimestamp");

                    b.Property<long>("LastUpdatedTimestamp");

                    b.Property<bool>("Listed");

                    b.Property<long>("PublishedTimestamp");

                    b.HasKey("PackageKey");

                    b.HasIndex("CreatedTimestamp");

                    b.HasIndex("LastEditedTimestamp");

                    b.ToTable("V2Packages");
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.CatalogCommitEntity", b =>
                {
                    b.HasOne("Knapcode.ExplorePackages.Entities.CatalogPageEntity", "CatalogPage")
                        .WithMany("CatalogCommits")
                        .HasForeignKey("CatalogPageKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.CatalogLeafEntity", b =>
                {
                    b.HasOne("Knapcode.ExplorePackages.Entities.CatalogCommitEntity", "CatalogCommit")
                        .WithMany("CatalogLeaves")
                        .HasForeignKey("CatalogCommitKey")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Knapcode.ExplorePackages.Entities.CatalogPackageEntity", "CatalogPackage")
                        .WithMany("CatalogLeaves")
                        .HasForeignKey("PackageKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.CatalogPackageEntity", b =>
                {
                    b.HasOne("Knapcode.ExplorePackages.Entities.PackageEntity", "Package")
                        .WithOne("CatalogPackage")
                        .HasForeignKey("Knapcode.ExplorePackages.Entities.CatalogPackageEntity", "PackageKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.CatalogPackageRegistrationEntity", b =>
                {
                    b.HasOne("Knapcode.ExplorePackages.Entities.PackageRegistrationEntity", "PackageRegistration")
                        .WithOne("CatalogPackageRegistration")
                        .HasForeignKey("Knapcode.ExplorePackages.Entities.CatalogPackageRegistrationEntity", "PackageRegistrationKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.PackageArchiveEntity", b =>
                {
                    b.HasOne("Knapcode.ExplorePackages.Entities.PackageEntity", "Package")
                        .WithOne("PackageArchive")
                        .HasForeignKey("Knapcode.ExplorePackages.Entities.PackageArchiveEntity", "PackageKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.PackageDependencyEntity", b =>
                {
                    b.HasOne("Knapcode.ExplorePackages.Entities.PackageEntity", "BestDependencyPackage")
                        .WithMany("BestPackageDependents")
                        .HasForeignKey("BestDependencyPackageKey");

                    b.HasOne("Knapcode.ExplorePackages.Entities.PackageRegistrationEntity", "DependencyPackageRegistration")
                        .WithMany("PackageDependents")
                        .HasForeignKey("DependencyPackageRegistrationKey")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Knapcode.ExplorePackages.Entities.FrameworkEntity", "Framework")
                        .WithMany("PackageDependencies")
                        .HasForeignKey("FrameworkKey");

                    b.HasOne("Knapcode.ExplorePackages.Entities.PackageEntity", "MinimumDependencyPackage")
                        .WithMany("MinimumPackageDependents")
                        .HasForeignKey("MinimumDependencyPackageKey");

                    b.HasOne("Knapcode.ExplorePackages.Entities.PackageEntity", "ParentPackage")
                        .WithMany("PackageDependencies")
                        .HasForeignKey("ParentPackageKey")
                        .OnDelete(DeleteBehavior.Restrict);
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

            modelBuilder.Entity("Knapcode.ExplorePackages.Entities.PackageEntryEntity", b =>
                {
                    b.HasOne("Knapcode.ExplorePackages.Entities.PackageArchiveEntity", "PackageArchive")
                        .WithMany("PackageEntries")
                        .HasForeignKey("PackageKey")
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
