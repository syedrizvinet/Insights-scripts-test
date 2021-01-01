﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Knapcode.ExplorePackages.Worker
{
    public class CatalogPageScanMessageProcessor : IMessageProcessor<CatalogPageScanMessage>
    {
        private readonly CatalogClient _catalogClient;
        private readonly CatalogScanDriverFactory _driverFactory;
        private readonly MessageEnqueuer _messageEnqueuer;
        private readonly CatalogScanStorageService _storageService;
        private readonly CatalogScanExpandService _expandService;
        private readonly ILogger<CatalogPageScanMessageProcessor> _logger;

        public CatalogPageScanMessageProcessor(
            CatalogClient catalogClient,
            CatalogScanDriverFactory driverFactory,
            MessageEnqueuer messageEnqueuer,
            CatalogScanStorageService storageService,
            CatalogScanExpandService expandService,
            ILogger<CatalogPageScanMessageProcessor> logger)
        {
            _catalogClient = catalogClient;
            _driverFactory = driverFactory;
            _messageEnqueuer = messageEnqueuer;
            _storageService = storageService;
            _expandService = expandService;
            _logger = logger;
        }

        public async Task ProcessAsync(CatalogPageScanMessage message, int dequeueCount)
        {
            var scan = await _storageService.GetPageScanAsync(message.StorageSuffix, message.ScanId, message.PageId);
            if (scan == null)
            {
                _logger.LogWarning("No matching page scan was found.");
                return;
            }

            var drvier = _driverFactory.Create(scan.ParsedDriverType);

            var result = await drvier.ProcessPageAsync(scan);

            switch (result)
            {
                case CatalogPageScanResult.Processed:
                    await _storageService.DeleteAsync(scan);
                    break;
                case CatalogPageScanResult.ExpandAllowDuplicates:
                    await ExpandAsync(message, scan, excludeRedundantLeaves: false);
                    break;
                case CatalogPageScanResult.ExpandRemoveDuplicates:
                    await ExpandAsync(message, scan, excludeRedundantLeaves: true);
                    break;
                default:
                    throw new NotSupportedException($"Catalog page scan result '{result}' is not supported.");
            }
        }

        private async Task ExpandAsync(CatalogPageScanMessage message, CatalogPageScan scan, bool excludeRedundantLeaves)
        {
            var lazyLeafScansTask = new Lazy<Task<List<CatalogLeafScan>>>(() => InitializeLeavesAsync(scan, excludeRedundantLeaves));

            // Created: no-op
            if (scan.ParsedState == CatalogScanState.Created)
            {
                scan.ParsedState = CatalogScanState.Expanding;
                await _storageService.ReplaceAsync(scan);
            }

            // Expanding: create a record for each leaf
            if (scan.ParsedState == CatalogScanState.Expanding)
            {
                var leafScans = await lazyLeafScansTask.Value;
                await _expandService.InsertLeafScansAsync(scan, leafScans);

                scan.ParsedState = CatalogScanState.Enqueuing;
                await _storageService.ReplaceAsync(scan);
            }

            // Enqueueing: enqueue a message for each leaf
            if (scan.ParsedState == CatalogScanState.Enqueuing)
            {
                var leafScans = await lazyLeafScansTask.Value;
                await _expandService.EnqueueLeafScansAsync(leafScans);

                scan.ParsedState = CatalogScanState.Waiting;
                message.AttemptCount = 0;
                await _storageService.ReplaceAsync(scan);
            }

            // Waiting: check if all of the leaf scans are complete
            if (scan.ParsedState == CatalogScanState.Waiting)
            {
                var countLowerBound = await _storageService.GetLeafScanCountLowerBoundAsync(scan.StorageSuffix, scan.ScanId, scan.PageId);
                if (countLowerBound > 0)
                {
                    _logger.LogInformation("There are at least {Count} leaf scans pending.", countLowerBound);
                    message.AttemptCount++;
                    await _messageEnqueuer.EnqueueAsync(new[] { message }, StorageUtility.GetMessageDelay(message.AttemptCount));
                }
                else
                {
                    _logger.LogInformation("The page scan is complete.");
                    scan.ParsedState = CatalogScanState.Complete;
                    await _storageService.ReplaceAsync(scan);
                }
            }

            // Complete -> Deleted
            if (scan.ParsedState == CatalogScanState.Complete)
            {
                await _storageService.DeleteAsync(scan);
            }
        }

        private async Task<List<CatalogLeafScan>> InitializeLeavesAsync(CatalogPageScan scan, bool excludeRedundantLeaves)
        {
            _logger.LogInformation("Loading catalog page URL: {Url}", scan.Url);
            var page = await _catalogClient.GetCatalogPageAsync(scan.Url);

            var items = page.GetLeavesInBounds(scan.Min, scan.Max, excludeRedundantLeaves);
            var leafItemToRank = page.GetLeafItemToRank();

            _logger.LogInformation("Starting scan of {LeafCount} leaves from ({Min:O}, {Max:O}].", items.Count, scan.Min, scan.Max);

            return _expandService.CreateLeafScans(scan, items, leafItemToRank);
        }
    }
}