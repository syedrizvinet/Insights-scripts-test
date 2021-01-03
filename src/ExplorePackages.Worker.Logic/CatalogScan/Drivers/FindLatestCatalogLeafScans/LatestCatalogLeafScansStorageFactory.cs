﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Knapcode.ExplorePackages.Worker.FindLatestCatalogLeafScans
{
    public class LatestCatalogLeafScansStorageFactory : ILatestPackageLeafStorageFactory<CatalogLeafScan>
    {
        private readonly SchemaSerializer _serializer;
        private readonly CatalogScanStorageService _storageService;

        public LatestCatalogLeafScansStorageFactory(
            SchemaSerializer serializer,
            CatalogScanStorageService storageService)
        {
            _serializer = serializer;
            _storageService = storageService;
        }

        public Task InitializeAsync(CatalogIndexScan indexScan) => Task.CompletedTask;

        public async Task<ILatestPackageLeafStorage<CatalogLeafScan>> CreateAsync(CatalogPageScan pageScan, IReadOnlyDictionary<CatalogLeafItem, int> leafItemToRank)
        {
            var parameters = (CatalogIndexScanMessage)_serializer.Deserialize(pageScan.DriverParameters).Data;
            var indexScan = await _storageService.GetIndexScanAsync(parameters.CursorName, parameters.ScanId);
            var table = _storageService.GetLeafScanTable(indexScan.StorageSuffix);
            return new LatestCatalogLeafScansStorage(table, indexScan);
        }
    }
}