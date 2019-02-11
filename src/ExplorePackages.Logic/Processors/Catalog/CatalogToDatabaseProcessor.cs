﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Knapcode.ExplorePackages.Logic
{
    public class CatalogToDatabaseProcessor : ICatalogEntriesProcessor
    {
        private readonly IPackageService _packageService;
        private readonly CatalogClient _catalogClient;
        private readonly CatalogService _catalogService;
        private readonly V2Client _v2Client;

        public CatalogToDatabaseProcessor(
            IPackageService packageService,
            CatalogClient catalogClient,
            CatalogService catalogService,
            V2Client v2Client)
        {
            _packageService = packageService;
            _catalogClient = catalogClient;
            _catalogService = catalogService;
            _v2Client = v2Client;
        }

        public IReadOnlyList<string> DependencyCursorNames => new List<string>();

        public string CursorName => CursorNames.CatalogToDatabase;

        public async Task ProcessAsync(CatalogPageItem page, IReadOnlyList<CatalogLeafItem> leaves)
        {
            // Determine the listed status of all of the packages.
            var entryToVisibilityState = (await TaskProcessor.ExecuteAsync(
                leaves,
                async x =>
                {
                    PackageVisibilityState visiblityState;
                    if (x.IsPackageDelete())
                    {
                        visiblityState = new PackageVisibilityState(listed: null, semVer2: null);
                    }
                    else
                    {
                        visiblityState = await DetermineVisibilityStateAsync(x);
                    }

                    return KeyValuePairFactory.Create(x, visiblityState);
                },
                workerCount: 32,
                token: CancellationToken.None))
                .ToDictionary(x => x.Key, x => x.Value, new CatalogLeafItemComparer());

            // Only add Package entities based on the latest commit timestamp.
            var latestLeaves = leaves
                .GroupBy(x => new PackageIdentity(x.PackageId, x.ParsePackageVersion().ToNormalizedString()))
                .Select(x => x
                    .OrderByDescending(y => y.CommitTimestamp)
                    .First())
                .ToList();

            var identityToPackageKey = await _packageService.AddOrUpdatePackagesAsync(latestLeaves, entryToVisibilityState);
            
            await _catalogService.AddOrUpdateAsync(page, leaves, identityToPackageKey, entryToVisibilityState);

            await _packageService.SetDeletedPackagesAsUnlistedInV2Async(latestLeaves);
        }

        private async Task<PackageVisibilityState> DetermineVisibilityStateAsync(CatalogLeafItem entry)
        {
            var leaf = (PackageDetailsCatalogLeaf)await _catalogClient.GetCatalogLeafAsync(entry);

            var listed = leaf.IsListed();
            var semVer2 = leaf.IsSemVer2();

            return new PackageVisibilityState(listed, semVer2);
        }

        private class CatalogLeafItemComparer : IEqualityComparer<CatalogLeafItem>
        {
            public bool Equals(CatalogLeafItem x, CatalogLeafItem y)
            {
                return x?.Url == y?.Url;
            }

            public int GetHashCode(CatalogLeafItem obj)
            {
                return obj?.Url.GetHashCode() ?? 0;
            }
        }
    }
}
