﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Knapcode.ExplorePackages.Entities
{
    public interface IEntityContext : IDisposable
    {
        DbSet<CatalogCommitEntity> CatalogCommits { get; }
        DbSet<CatalogLeafEntity> CatalogLeaves { get; }
        DbSet<CatalogPackageEntity> CatalogPackages { get; }
        DbSet<CatalogPageEntity> CatalogPages { get; }
        DbSet<CursorEntity> Cursors { get; }
        DbSet<ETagEntity> ETags { get; }
        DbSet<FrameworkEntity> Frameworks { get; }
        DbSet<PackageArchiveEntity> PackageArchives { get; }
        DbSet<PackageDependencyEntity> PackageDependencies { get; }
        DbSet<PackageDownloadsEntity> PackageDownloads { get; }
        DbSet<PackageEntryEntity> PackageEntries { get; }
        DbSet<PackageQueryEntity> PackageQueries { get; }
        DbSet<PackageQueryMatchEntity> PackageQueryMatches { get; }
        DbSet<PackageRegistrationEntity> PackageRegistrations { get; }
        DbSet<PackageEntity> Packages { get; set; }
        DbSet<V2PackageEntity> V2PackageEntities { get; set; }

        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default(CancellationToken));
    }
}