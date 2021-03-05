﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Knapcode.ExplorePackages.Worker.LoadPackageVersion;
using Microsoft.Extensions.Options;
using NuGet.Versioning;

namespace Knapcode.ExplorePackages.Worker.PackageVersionToCsv
{
    public class PackageVersionToCsvDriver : ICatalogLeafToCsvDriver<PackageVersionRecord>
    {
        private readonly PackageVersionStorageService _storage;
        private readonly IOptions<ExplorePackagesWorkerSettings> _options;

        public PackageVersionToCsvDriver(
            PackageVersionStorageService storage,
            IOptions<ExplorePackagesWorkerSettings> options)
        {
            _storage = storage;
            _options = options;
        }

        public string ResultsContainerName => _options.Value.PackageVersionContainerName;
        public bool SingleMessagePerId => true;

        public List<PackageVersionRecord> Prune(List<PackageVersionRecord> records)
        {
            return PackageRecord.Prune(records);
        }

        public async Task InitializeAsync()
        {
            await _storage.InitializeAsync();
        }

        public async Task<DriverResult<List<PackageVersionRecord>>> ProcessLeafAsync(CatalogLeafItem item, int attemptCount)
        {
            Guid? scanId = null;
            DateTimeOffset? scanTimestamp = null;
            if (_options.Value.AppendResultUniqueIds)
            {
                scanId = Guid.NewGuid();
                scanTimestamp = DateTimeOffset.UtcNow;
            }

            // Fetch all of the known versions for this package ID.
            var entities = await _storage.GetAsync(item.PackageId);

            // Find the set of versions that can possibly the latest.
            var versions = entities
                .Where(x => x.ParsedLeafType != CatalogLeafType.PackageDelete) // Deleted versions can't be the latest
                .Where(x => x.IsListed.Value) // Only listed versions can be latest
                .Select(x => (Entity: x, Version: NuGetVersion.Parse(x.PackageVersion), IsSemVer2: x.ParsedSemVerType.Value.IsSemVer2()))
                .OrderByDescending(x => x.Version)
                .ToList();
            var semVer1Versions = versions.Where(x => !x.IsSemVer2).ToList();

            // Determine the four definitions of "latest". Reminds me of NuGet.org Azure Search implementation...
            var latest = semVer1Versions.FirstOrDefault();
            var latestStable = semVer1Versions.Where(x => !x.Version.IsPrerelease).FirstOrDefault();
            var latestSemVer2 = versions.FirstOrDefault();
            var latestStableSemVer2 = versions.Where(x => !x.Version.IsPrerelease).FirstOrDefault();

            // Map all entities to CSV records.
            var records = entities
                .Select(x => new PackageVersionRecord(scanId, scanTimestamp, x)
                {
                    IsLatest = ReferenceEquals(x, latest.Entity),
                    IsLatestStable = ReferenceEquals(x, latestStable.Entity),
                    IsLatestSemVer2 = ReferenceEquals(x, latestSemVer2.Entity),
                    IsLatestStableSemVer2 = ReferenceEquals(x, latestStableSemVer2.Entity),
                })
                .ToList();

            return new DriverResult<List<PackageVersionRecord>>(DriverResultType.Success, records);
        }

        public string GetBucketKey(CatalogLeafItem item)
        {
            return item.PackageId.ToLowerInvariant();
        }
    }
}