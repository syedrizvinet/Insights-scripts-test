﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Knapcode.ExplorePackages.Entities.Migrations.SqlServer
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatalogPages",
                columns: table => new
                {
                    CatalogPageKey = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Url = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogPages", x => x.CatalogPageKey);
                });

            migrationBuilder.CreateTable(
                name: "Cursors",
                columns: table => new
                {
                    CursorKey = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursors", x => x.CursorKey);
                });

            migrationBuilder.CreateTable(
                name: "ETags",
                columns: table => new
                {
                    ETagKey = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ETags", x => x.ETagKey);
                });

            migrationBuilder.CreateTable(
                name: "Frameworks",
                columns: table => new
                {
                    FrameworkKey = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<string>(nullable: false),
                    OriginalValue = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frameworks", x => x.FrameworkKey);
                });

            migrationBuilder.CreateTable(
                name: "PackageRegistrations",
                columns: table => new
                {
                    PackageRegistrationKey = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageRegistrations", x => x.PackageRegistrationKey);
                });

            migrationBuilder.CreateTable(
                name: "CatalogCommits",
                columns: table => new
                {
                    CatalogCommitKey = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CatalogPageKey = table.Column<long>(nullable: false),
                    CommitId = table.Column<string>(nullable: false),
                    CommitTimestamp = table.Column<long>(nullable: false),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogCommits", x => x.CatalogCommitKey);
                    table.ForeignKey(
                        name: "FK_CatalogCommits_CatalogPages_CatalogPageKey",
                        column: x => x.CatalogPageKey,
                        principalTable: "CatalogPages",
                        principalColumn: "CatalogPageKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackageQueries",
                columns: table => new
                {
                    PackageQueryKey = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CursorKey = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageQueries", x => x.PackageQueryKey);
                    table.ForeignKey(
                        name: "FK_PackageQueries_Cursors_CursorKey",
                        column: x => x.CursorKey,
                        principalTable: "Cursors",
                        principalColumn: "CursorKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    PackageKey = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PackageRegistrationKey = table.Column<long>(nullable: false),
                    Version = table.Column<string>(nullable: false),
                    Identity = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.PackageKey);
                    table.ForeignKey(
                        name: "FK_Packages_PackageRegistrations_PackageRegistrationKey",
                        column: x => x.PackageRegistrationKey,
                        principalTable: "PackageRegistrations",
                        principalColumn: "PackageRegistrationKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatalogPackages",
                columns: table => new
                {
                    PackageKey = table.Column<long>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    FirstCommitTimestamp = table.Column<long>(nullable: false),
                    LastCommitTimestamp = table.Column<long>(nullable: false),
                    Listed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogPackages", x => x.PackageKey);
                    table.ForeignKey(
                        name: "FK_CatalogPackages_Packages_PackageKey",
                        column: x => x.PackageKey,
                        principalTable: "Packages",
                        principalColumn: "PackageKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackageArchives",
                columns: table => new
                {
                    PackageKey = table.Column<long>(nullable: false),
                    Size = table.Column<long>(nullable: false),
                    EntryCount = table.Column<int>(nullable: false),
                    CentralDirectorySize = table.Column<long>(nullable: false),
                    Comment = table.Column<byte[]>(nullable: false),
                    CommentSize = table.Column<int>(nullable: false),
                    DiskWithStartOfCentralDirectory = table.Column<int>(nullable: false),
                    EntriesForWholeCentralDirectory = table.Column<int>(nullable: false),
                    EntriesInThisDisk = table.Column<int>(nullable: false),
                    NumberOfThisDisk = table.Column<int>(nullable: false),
                    OffsetAfterEndOfCentralDirectory = table.Column<long>(nullable: false),
                    OffsetOfCentralDirectory = table.Column<long>(nullable: false),
                    Zip64CentralDirectorySize = table.Column<decimal>(nullable: true),
                    Zip64DiskWithStartOfCentralDirectory = table.Column<long>(nullable: true),
                    Zip64DiskWithStartOfEndOfCentralDirectory = table.Column<long>(nullable: true),
                    Zip64EndOfCentralDirectoryOffset = table.Column<decimal>(nullable: true),
                    Zip64EntriesForWholeCentralDirectory = table.Column<decimal>(nullable: true),
                    Zip64EntriesInThisDisk = table.Column<decimal>(nullable: true),
                    Zip64NumberOfThisDisk = table.Column<long>(nullable: true),
                    Zip64OffsetAfterEndOfCentralDirectoryLocator = table.Column<long>(nullable: true),
                    Zip64OffsetOfCentralDirectory = table.Column<decimal>(nullable: true),
                    Zip64TotalNumberOfDisks = table.Column<long>(nullable: true),
                    Zip64SizeOfCentralDirectoryRecord = table.Column<decimal>(nullable: true),
                    Zip64VersionMadeBy = table.Column<int>(nullable: true),
                    Zip64VersionToExtract = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageArchives", x => x.PackageKey);
                    table.ForeignKey(
                        name: "FK_PackageArchives_Packages_PackageKey",
                        column: x => x.PackageKey,
                        principalTable: "Packages",
                        principalColumn: "PackageKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackageDependencies",
                columns: table => new
                {
                    PackageDependencyKey = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ParentPackageKey = table.Column<long>(nullable: false),
                    DependencyPackageRegistrationKey = table.Column<long>(nullable: false),
                    FrameworkKey = table.Column<long>(nullable: true),
                    VersionRange = table.Column<string>(nullable: true),
                    OriginalVersionRange = table.Column<string>(nullable: true),
                    MinimumDependencyPackageKey = table.Column<long>(nullable: true),
                    BestDependencyPackageKey = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageDependencies", x => x.PackageDependencyKey);
                    table.ForeignKey(
                        name: "FK_PackageDependencies_Packages_BestDependencyPackageKey",
                        column: x => x.BestDependencyPackageKey,
                        principalTable: "Packages",
                        principalColumn: "PackageKey",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PackageDependencies_PackageRegistrations_DependencyPackageRegistrationKey",
                        column: x => x.DependencyPackageRegistrationKey,
                        principalTable: "PackageRegistrations",
                        principalColumn: "PackageRegistrationKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PackageDependencies_Frameworks_FrameworkKey",
                        column: x => x.FrameworkKey,
                        principalTable: "Frameworks",
                        principalColumn: "FrameworkKey",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PackageDependencies_Packages_MinimumDependencyPackageKey",
                        column: x => x.MinimumDependencyPackageKey,
                        principalTable: "Packages",
                        principalColumn: "PackageKey",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PackageDependencies_Packages_ParentPackageKey",
                        column: x => x.ParentPackageKey,
                        principalTable: "Packages",
                        principalColumn: "PackageKey",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PackageDownloads",
                columns: table => new
                {
                    PackageKey = table.Column<long>(nullable: false),
                    Downloads = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageDownloads", x => x.PackageKey);
                    table.ForeignKey(
                        name: "FK_PackageDownloads_Packages_PackageKey",
                        column: x => x.PackageKey,
                        principalTable: "Packages",
                        principalColumn: "PackageKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackageQueryMatches",
                columns: table => new
                {
                    PackageQueryMatchKey = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PackageKey = table.Column<long>(nullable: false),
                    PackageQueryKey = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageQueryMatches", x => x.PackageQueryMatchKey);
                    table.ForeignKey(
                        name: "FK_PackageQueryMatches_Packages_PackageKey",
                        column: x => x.PackageKey,
                        principalTable: "Packages",
                        principalColumn: "PackageKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PackageQueryMatches_PackageQueries_PackageQueryKey",
                        column: x => x.PackageQueryKey,
                        principalTable: "PackageQueries",
                        principalColumn: "PackageQueryKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "V2Packages",
                columns: table => new
                {
                    PackageKey = table.Column<long>(nullable: false),
                    CreatedTimestamp = table.Column<long>(nullable: false),
                    LastEditedTimestamp = table.Column<long>(nullable: true),
                    PublishedTimestamp = table.Column<long>(nullable: false),
                    LastUpdatedTimestamp = table.Column<long>(nullable: false),
                    Listed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_V2Packages", x => x.PackageKey);
                    table.ForeignKey(
                        name: "FK_V2Packages_Packages_PackageKey",
                        column: x => x.PackageKey,
                        principalTable: "Packages",
                        principalColumn: "PackageKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatalogLeaves",
                columns: table => new
                {
                    CatalogLeafKey = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CatalogCommitKey = table.Column<long>(nullable: false),
                    PackageKey = table.Column<long>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    RelativePath = table.Column<string>(nullable: true),
                    IsListed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogLeaves", x => x.CatalogLeafKey);
                    table.ForeignKey(
                        name: "FK_CatalogLeaves_CatalogCommits_CatalogCommitKey",
                        column: x => x.CatalogCommitKey,
                        principalTable: "CatalogCommits",
                        principalColumn: "CatalogCommitKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogLeaves_CatalogPackages_PackageKey",
                        column: x => x.PackageKey,
                        principalTable: "CatalogPackages",
                        principalColumn: "PackageKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackageEntries",
                columns: table => new
                {
                    PackageEntryKey = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PackageKey = table.Column<long>(nullable: false),
                    Index = table.Column<decimal>(nullable: false),
                    Comment = table.Column<byte[]>(nullable: false),
                    ExtraField = table.Column<byte[]>(nullable: false),
                    Name = table.Column<byte[]>(nullable: false),
                    LocalHeaderOffset = table.Column<long>(nullable: false),
                    ExternalAttributes = table.Column<long>(nullable: false),
                    InternalAttributes = table.Column<int>(nullable: false),
                    DiskNumberStart = table.Column<int>(nullable: false),
                    CommentSize = table.Column<int>(nullable: false),
                    ExtraFieldSize = table.Column<int>(nullable: false),
                    NameSize = table.Column<int>(nullable: false),
                    UncompressedSize = table.Column<long>(nullable: false),
                    CompressedSize = table.Column<long>(nullable: false),
                    Crc32 = table.Column<long>(nullable: false),
                    LastModifiedDate = table.Column<int>(nullable: false),
                    LastModifiedTime = table.Column<int>(nullable: false),
                    CompressionMethod = table.Column<int>(nullable: false),
                    Flags = table.Column<int>(nullable: false),
                    VersionToExtract = table.Column<int>(nullable: false),
                    VersionMadeBy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageEntries", x => x.PackageEntryKey);
                    table.ForeignKey(
                        name: "FK_PackageEntries_PackageArchives_PackageKey",
                        column: x => x.PackageKey,
                        principalTable: "PackageArchives",
                        principalColumn: "PackageKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogCommits_CatalogPageKey",
                table: "CatalogCommits",
                column: "CatalogPageKey");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogCommits_CommitId",
                table: "CatalogCommits",
                column: "CommitId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogCommits_CommitTimestamp",
                table: "CatalogCommits",
                column: "CommitTimestamp",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogLeaves_CatalogCommitKey",
                table: "CatalogLeaves",
                column: "CatalogCommitKey");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogLeaves_PackageKey",
                table: "CatalogLeaves",
                column: "PackageKey");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogPackages_LastCommitTimestamp",
                table: "CatalogPackages",
                column: "LastCommitTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogPages_Url",
                table: "CatalogPages",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cursors_Name",
                table: "Cursors",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ETags_Name",
                table: "ETags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Frameworks_OriginalValue",
                table: "Frameworks",
                column: "OriginalValue",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PackageDependencies_BestDependencyPackageKey",
                table: "PackageDependencies",
                column: "BestDependencyPackageKey");

            migrationBuilder.CreateIndex(
                name: "IX_PackageDependencies_DependencyPackageRegistrationKey",
                table: "PackageDependencies",
                column: "DependencyPackageRegistrationKey");

            migrationBuilder.CreateIndex(
                name: "IX_PackageDependencies_FrameworkKey",
                table: "PackageDependencies",
                column: "FrameworkKey");

            migrationBuilder.CreateIndex(
                name: "IX_PackageDependencies_MinimumDependencyPackageKey",
                table: "PackageDependencies",
                column: "MinimumDependencyPackageKey");

            migrationBuilder.CreateIndex(
                name: "IX_PackageDependencies_ParentPackageKey_DependencyPackageRegistrationKey_FrameworkKey",
                table: "PackageDependencies",
                columns: new[] { "ParentPackageKey", "DependencyPackageRegistrationKey", "FrameworkKey" },
                unique: true,
                filter: "[FrameworkKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PackageEntries_PackageKey_Index",
                table: "PackageEntries",
                columns: new[] { "PackageKey", "Index" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PackageQueries_CursorKey",
                table: "PackageQueries",
                column: "CursorKey");

            migrationBuilder.CreateIndex(
                name: "IX_PackageQueries_Name",
                table: "PackageQueries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PackageQueryMatches_PackageKey",
                table: "PackageQueryMatches",
                column: "PackageKey");

            migrationBuilder.CreateIndex(
                name: "IX_PackageQueryMatches_PackageQueryKey_PackageKey",
                table: "PackageQueryMatches",
                columns: new[] { "PackageQueryKey", "PackageKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PackageRegistrations_Id",
                table: "PackageRegistrations",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_Identity",
                table: "Packages",
                column: "Identity",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_PackageRegistrationKey_Version",
                table: "Packages",
                columns: new[] { "PackageRegistrationKey", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_V2Packages_CreatedTimestamp",
                table: "V2Packages",
                column: "CreatedTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_V2Packages_LastEditedTimestamp",
                table: "V2Packages",
                column: "LastEditedTimestamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogLeaves");

            migrationBuilder.DropTable(
                name: "ETags");

            migrationBuilder.DropTable(
                name: "PackageDependencies");

            migrationBuilder.DropTable(
                name: "PackageDownloads");

            migrationBuilder.DropTable(
                name: "PackageEntries");

            migrationBuilder.DropTable(
                name: "PackageQueryMatches");

            migrationBuilder.DropTable(
                name: "V2Packages");

            migrationBuilder.DropTable(
                name: "CatalogCommits");

            migrationBuilder.DropTable(
                name: "CatalogPackages");

            migrationBuilder.DropTable(
                name: "Frameworks");

            migrationBuilder.DropTable(
                name: "PackageArchives");

            migrationBuilder.DropTable(
                name: "PackageQueries");

            migrationBuilder.DropTable(
                name: "CatalogPages");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "Cursors");

            migrationBuilder.DropTable(
                name: "PackageRegistrations");
        }
    }
}
