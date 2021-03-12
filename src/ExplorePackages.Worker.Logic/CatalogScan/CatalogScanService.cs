﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Knapcode.ExplorePackages.Worker
{
    public class CatalogScanService
    {
        private readonly CatalogScanCursorService _cursorService;
        private readonly IMessageEnqueuer _messageEnqueuer;
        private readonly SchemaSerializer _serializer;
        private readonly CatalogScanStorageService _storageService;
        private readonly AutoRenewingStorageLeaseService _leaseService;
        private readonly IOptions<ExplorePackagesWorkerSettings> _options;
        private readonly ILogger<CatalogScanService> _logger;

        public CatalogScanService(
            CatalogScanCursorService cursorService,
            IMessageEnqueuer messageEnqueuer,
            SchemaSerializer serializer,
            CatalogScanStorageService catalogScanStorageService,
            AutoRenewingStorageLeaseService leaseService,
            IOptions<ExplorePackagesWorkerSettings> options,
            ILogger<CatalogScanService> logger)
        {
            _cursorService = cursorService;
            _messageEnqueuer = messageEnqueuer;
            _serializer = serializer;
            _storageService = catalogScanStorageService;
            _leaseService = leaseService;
            _options = options;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            await _cursorService.InitializeAsync();
            await _storageService.InitializeAsync();
            await _messageEnqueuer.InitializeAsync();
            await _leaseService.InitializeAsync();
        }

        public async Task RequeueAsync(CatalogScanDriverType driverType, string scanId)
        {
            var cursorName = _cursorService.GetCursorName(driverType);
            var indexScan = await _storageService.GetIndexScanAsync(cursorName, scanId);
            if (indexScan.ParsedState != CatalogIndexScanState.Working)
            {
                return;
            }

            var pageScans = await _storageService.GetPageScansAsync(indexScan.StorageSuffix, indexScan.ScanId);
            var leafScans = new List<CatalogLeafScan>();
            foreach (var pageScan in pageScans)
            {
                var pageLeafScans = await _storageService.GetLeafScansAsync(pageScan.StorageSuffix, pageScan.ScanId, pageScan.PageId);
                leafScans.AddRange(pageLeafScans);
            }

            await _messageEnqueuer.EnqueueAsync(leafScans
                .Select(x => new CatalogLeafScanMessage
                {
                    StorageSuffix = x.StorageSuffix,
                    ScanId = x.ScanId,
                    PageId = x.PageId,
                    LeafId = x.LeafId,
                })
                .ToList());

            await _messageEnqueuer.EnqueueAsync(pageScans
                .Select(x => new CatalogPageScanMessage
                {
                    StorageSuffix = x.StorageSuffix,
                    ScanId = x.ScanId,
                    PageId = x.PageId,
                })
                .ToList());

            await _messageEnqueuer.EnqueueAsync(new[]
            {
                new CatalogIndexScanMessage
                {
                    CursorName = indexScan.CursorName,
                    ScanId = indexScan.ScanId,
                },
            });
        }

        public bool SupportsReprocess(CatalogScanDriverType driverType)
        {
            switch (driverType)
            {
                case CatalogScanDriverType.NuGetPackageExplorerToCsv:
                    return true;

                default:
                    return false;
            }
        }

        public bool? GetOnlyLatestLeavesSupport(CatalogScanDriverType driverType)
        {
            switch (driverType)
            {
                case CatalogScanDriverType.CatalogLeafItemToCsv:
                    return false;

                case CatalogScanDriverType.LoadLatestPackageLeaf:
                    return true;

                case CatalogScanDriverType.PackageArchiveEntryToCsv:
                case CatalogScanDriverType.PackageAssemblyToCsv:
                case CatalogScanDriverType.PackageAssetToCsv:
                case CatalogScanDriverType.PackageSignatureToCsv:
                case CatalogScanDriverType.PackageManifestToCsv:
                case CatalogScanDriverType.PackageVersionToCsv:
                case CatalogScanDriverType.NuGetPackageExplorerToCsv:
                    return null;

                case CatalogScanDriverType.LoadPackageArchive:
                case CatalogScanDriverType.LoadPackageManifest:
                case CatalogScanDriverType.LoadPackageVersion:
                    return true;

                default:
                    throw new NotSupportedException();
            }
        }

        public async Task<CatalogScanServiceResult> ReprocessAsync(CatalogScanDriverType driverType)
        {
            if (!SupportsReprocess(driverType))
            {
                throw new ArgumentException("Reprocessing is not support for this driver type.", nameof(driverType));
            }

            switch (driverType)
            {
                case CatalogScanDriverType.NuGetPackageExplorerToCsv:
                    return await ReprocessCatalogLeafToCsvAsync(driverType);

                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<IReadOnlyDictionary<CatalogScanDriverType, CatalogScanServiceResult>> UpdateAllAsync(DateTimeOffset max)
        {
            var results = new Dictionary<CatalogScanDriverType, CatalogScanServiceResult>();
            foreach (var driverType in _cursorService.StartableDriverTypes)
            {
                var result = await UpdateAsync(driverType, max, onlyLatestLeaves: null, continueWithDependents: true);
                results.Add(driverType, result);
            }

            return results;
        }

        public async Task<CatalogScanServiceResult> UpdateAsync(CatalogScanDriverType driverType)
        {
            return await UpdateAsync(driverType, max: null);
        }

        public async Task<CatalogScanServiceResult> UpdateAsync(CatalogScanDriverType driverType, DateTimeOffset? max)
        {
            return await UpdateAsync(driverType, max, onlyLatestLeaves: null);
        }

        public async Task<CatalogScanServiceResult> UpdateAsync(CatalogScanDriverType driverType, DateTimeOffset? max, bool? onlyLatestLeaves)
        {
            return await UpdateAsync(driverType, max, onlyLatestLeaves, continueWithDependents: false);
        }

        private async Task<CatalogScanServiceResult> UpdateAsync(CatalogScanDriverType driverType, DateTimeOffset? max, bool? onlyLatestLeaves, bool continueWithDependents)
        {
            var onlyLatestLeavesSupport = GetOnlyLatestLeavesSupport(driverType);
            if (onlyLatestLeavesSupport.HasValue
                && onlyLatestLeaves.HasValue
                && onlyLatestLeaves != onlyLatestLeavesSupport)
            {
                if (onlyLatestLeavesSupport.Value)
                {
                    throw new ArgumentException("Only using latest leaves is supported for this driver type.", nameof(onlyLatestLeaves));
                }
                else
                {
                    throw new ArgumentException("Only using all leaves is supported for this driver type.", nameof(onlyLatestLeaves));
                }
            }

            switch (driverType)
            {
                case CatalogScanDriverType.CatalogLeafItemToCsv:
                    return await UpdateAsync(
                        driverType,
                        parameters: null,
                        CatalogClient.NuGetOrgMin,
                        max,
                        continueWithDependents);

                case CatalogScanDriverType.PackageArchiveEntryToCsv:
                case CatalogScanDriverType.PackageAssemblyToCsv:
                case CatalogScanDriverType.PackageAssetToCsv:
                case CatalogScanDriverType.PackageSignatureToCsv:
                case CatalogScanDriverType.PackageManifestToCsv:
                case CatalogScanDriverType.PackageVersionToCsv:
                case CatalogScanDriverType.NuGetPackageExplorerToCsv:
                    return await UpdateCatalogLeafToCsvAsync(
                        driverType,
                        onlyLatestLeaves.GetValueOrDefault(true),
                        max,
                        continueWithDependents);

                case CatalogScanDriverType.LoadLatestPackageLeaf:
                case CatalogScanDriverType.LoadPackageArchive:
                case CatalogScanDriverType.LoadPackageManifest:
                case CatalogScanDriverType.LoadPackageVersion:
                    return await UpdateAsync(
                        driverType,
                        parameters: null,
                        min: CatalogClient.NuGetOrgMinDeleted,
                        max,
                        continueWithDependents);

                default:
                    throw new NotSupportedException();
            }
        }

        public async Task<CatalogIndexScan> GetOrStartFindLatestCatalogLeafScanAsync(
            string scanId,
            string storageSuffix,
            CatalogIndexScanMessage parentScanMessage,
            DateTimeOffset min,
            DateTimeOffset max)
        {
            return await GetOrStartCursorlessAsync(
                scanId,
                storageSuffix,
                CatalogScanDriverType.Internal_FindLatestCatalogLeafScan,
                parameters: _serializer.Serialize(parentScanMessage).AsString(),
                min,
                max);
        }

        public async Task<CatalogIndexScan> GetOrStartFindLatestCatalogLeafScanPerIdAsync(
            string scanId,
            string storageSuffix,
            CatalogIndexScanMessage parentScanMessage,
            DateTimeOffset min,
            DateTimeOffset max)
        {
            return await GetOrStartCursorlessAsync(
                scanId,
                storageSuffix,
                CatalogScanDriverType.Internal_FindLatestCatalogLeafScanPerId,
                parameters: _serializer.Serialize(parentScanMessage).AsString(),
                min,
                max);
        }

        private async Task<CatalogScanServiceResult> ReprocessCatalogLeafToCsvAsync(CatalogScanDriverType driverType)
        {
            var parameters = new CatalogLeafToCsvParameters
            {
                BucketCount = _options.Value.AppendResultStorageBucketCount,
                Mode = CatalogLeafToCsvMode.Reprocess,
            };

            var cursorValue = await _cursorService.GetCursorValueAsync(driverType);
            var scanId = StorageUtility.GenerateDescendingId();
            var scan = await GetOrStartCursorlessAsync(
                scanId.ToString(),
                scanId.Unique,
                driverType,
                parameters: _serializer.Serialize(parameters).AsString(),
                CatalogClient.NuGetOrgMinAvailable,
                cursorValue);

            return new CatalogScanServiceResult(
                CatalogScanServiceResultType.NewStarted,
                dependencyName: null,
                scan);
        }

        private async Task<CatalogScanServiceResult> UpdateCatalogLeafToCsvAsync(
            CatalogScanDriverType driverType,
            bool onlyLatestLeaves,
            DateTimeOffset? max,
            bool continueWithDependents)
        {
            var parameters = new CatalogLeafToCsvParameters
            {
                BucketCount = _options.Value.AppendResultStorageBucketCount,
                Mode = onlyLatestLeaves ? CatalogLeafToCsvMode.LatestLeaves : CatalogLeafToCsvMode.AllLeaves,
            };

            return await UpdateAsync(
                driverType,
                parameters: _serializer.Serialize(parameters).AsString(),
                onlyLatestLeaves ? CatalogClient.NuGetOrgMinDeleted : CatalogClient.NuGetOrgMinAvailable,
                max,
                continueWithDependents);
        }

        private async Task<CatalogScanServiceResult> UpdateAsync(
            CatalogScanDriverType driverType,
            string parameters,
            DateTimeOffset min,
            DateTimeOffset? max,
            bool continueWithDependents)
        {
            // Check if a scan is already running, outside the lease.
            var cursor = await _cursorService.GetCursorAsync(driverType);
            var incompleteScan = await GetLatestIncompleteScanAsync(cursor.Name);
            if (incompleteScan != null)
            {
                return new CatalogScanServiceResult(CatalogScanServiceResultType.AlreadyRunning, dependencyName: null, incompleteScan);
            }

            if (cursor.Value > CursorTableEntity.Min)
            {
                // Use the cursor value as the min if it's greater than the provided min. We don't want to process leaves
                // that have already been scanned.
                min = cursor.Value;
            }

            (var dependencyName, var dependencyMax) = await _cursorService.GetDependencyMaxAsync(driverType);

            if (dependencyMax <= CursorTableEntity.Min)
            {
                return new CatalogScanServiceResult(CatalogScanServiceResultType.BlockedByDependency, dependencyName, scan: null);
            }

            var tookDependencyMax = false;
            if (!max.HasValue)
            {
                max = dependencyMax;
                tookDependencyMax = true;
            }
            else
            {
                if (max > dependencyMax)
                {
                    return new CatalogScanServiceResult(CatalogScanServiceResultType.BlockedByDependency, dependencyName, scan: null);
                }
            }

            if (max < min)
            {
                // If the provided max is less than the smart min, revert the min to the absolute min. This allows
                // very short test runs from the beginning of the catalog.
                min = CursorTableEntity.Min;
            }

            if (min > max)
            {
                return new CatalogScanServiceResult(CatalogScanServiceResultType.MinAfterMax, dependencyName: null, scan: null);
            }

            if (!tookDependencyMax && min == max)
            {
                return new CatalogScanServiceResult(CatalogScanServiceResultType.FullyCaughtUpWithMax, dependencyName: null, scan: null);
            }

            if (min == dependencyMax)
            {
                return new CatalogScanServiceResult(CatalogScanServiceResultType.FullyCaughtUpWithDependency, dependencyName, scan: null);
            }

            await using (var lease = await _leaseService.TryAcquireAsync($"Start-{cursor.Name}"))
            {
                if (!lease.Acquired)
                {
                    return new CatalogScanServiceResult(CatalogScanServiceResultType.UnavailableLease, dependencyName: null, scan: null);
                }

                // Check if a scan is already running, inside the lease.
                incompleteScan = await GetLatestIncompleteScanAsync(cursor.Name);
                if (incompleteScan != null)
                {
                    return new CatalogScanServiceResult(CatalogScanServiceResultType.AlreadyRunning, dependencyName: null, incompleteScan);
                }

                var descendingId = StorageUtility.GenerateDescendingId();
                var newScan = await StartWithoutLeaseAsync(
                    cursor.Name,
                    descendingId.ToString(),
                    descendingId.Unique,
                    driverType,
                    parameters,
                    min,
                    max.Value,
                    continueWithDependents);

                return new CatalogScanServiceResult(CatalogScanServiceResultType.NewStarted, dependencyName: null, newScan);
            }
        }

        private async Task<CatalogIndexScan> GetOrStartCursorlessAsync(
            string scanId,
            string storageSuffix,
            CatalogScanDriverType driverType,
            string parameters,
            DateTimeOffset min,
            DateTimeOffset max)
        {
            var cursorName = string.Empty;

            // Check if a scan is already running, outside the lease.
            var incompleteScan = await _storageService.GetIndexScanAsync(cursorName, scanId);
            if (incompleteScan != null)
            {
                return incompleteScan;
            }

            // Use a rather generic lease, to simplify clean-up.
            await using (var lease = await _leaseService.TryAcquireAsync($"Start-{_cursorService.GetCursorName(driverType)}"))
            {
                if (!lease.Acquired)
                {
                    throw new InvalidOperationException("Another thread is already starting the scan.");
                }

                // Check if a scan is already running, inside the lease.
                incompleteScan = await _storageService.GetIndexScanAsync(cursorName, scanId);
                if (incompleteScan != null)
                {
                    return incompleteScan;
                }

                return await StartWithoutLeaseAsync(
                    cursorName,
                    scanId,
                    storageSuffix,
                    driverType,
                    parameters,
                    min,
                    max,
                    continueWithDependents: false);
            }
        }

        private async Task<CatalogIndexScan> StartWithoutLeaseAsync(
            string cursorName,
            string scanId,
            string storageSuffix,
            CatalogScanDriverType driverType,
            string parameters,
            DateTimeOffset min,
            DateTimeOffset max,
            bool continueWithDependents)
        {
            // Start a new scan.
            _logger.LogInformation("Attempting to start a {DriverType} catalog index scan from ({Min}, {Max}].", driverType, min, max);

            var catalogIndexScanMessage = new CatalogIndexScanMessage
            {
                CursorName = cursorName,
                ScanId = scanId,
            };
            await _messageEnqueuer.EnqueueAsync(new[] { catalogIndexScanMessage });
            var catalogIndexScan = new CatalogIndexScan(cursorName, scanId, storageSuffix)
            {
                ParsedDriverType = driverType,
                DriverParameters = parameters,
                ParsedState = CatalogIndexScanState.Created,
                Min = min,
                Max = max,
                ContinueUpdate = continueWithDependents,
            };
            await _storageService.InsertAsync(catalogIndexScan);

            return catalogIndexScan;
        }

        private async Task<CatalogIndexScan> GetLatestIncompleteScanAsync(string cursorName)
        {
            var latestScans = await _storageService.GetLatestIndexScans(cursorName);
            var incompleteScans = latestScans.Where(x => x.ParsedState != CatalogIndexScanState.Complete);
            if (incompleteScans.Any())
            {
                return incompleteScans.First();
            }

            return null;
        }
    }
}
