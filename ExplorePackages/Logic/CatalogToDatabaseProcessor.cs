﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NuGet.CatalogReader;

namespace Knapcode.ExplorePackages.Logic
{
    public class CatalogToDatabaseProcessor : ICatalogEntriesProcessor
    {
        private readonly PackageService _packageService;

        public CatalogToDatabaseProcessor(PackageService packageService)
        {
            _packageService = packageService;
        }

        public IReadOnlyList<string> DependencyCursorNames => new List<string>();

        public string CursorName => CursorNames.CatalogToDatabase;

        public async Task ProcessAsync(CatalogPageEntry page, IReadOnlyList<CatalogEntry> leaves)
        {
            await _packageService.AddOrUpdatePackagesAsync(leaves);
        }
    }
}
