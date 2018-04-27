﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Knapcode.ExplorePackages.Entities;

namespace Knapcode.ExplorePackages.Logic
{
    public class DependenciesToDatabaseCommitProcessor : ICommitProcessor<PackageEntity, PackageDependencyGroups>
    {
        private readonly NuspecProvider _nuspecProvider;
        private readonly PackageDependencyService _packageDependencyService;

        public DependenciesToDatabaseCommitProcessor(
            NuspecProvider nuspecProvider,
            PackageDependencyService packageDependencyService)
        {
            _nuspecProvider = nuspecProvider;
            _packageDependencyService = packageDependencyService;
        }

        public string CursorName => CursorNames.DependenciesToDatabase;

        public IReadOnlyList<string> DependencyCursorNames { get; } = new[]
        {
            CursorNames.CatalogToNuspecs,
            CursorNames.CatalogToDatabase,
        };

        public int BatchSize => 5000;

        public Task<IReadOnlyList<PackageDependencyGroups>> InitializeItemsAsync(IReadOnlyList<PackageEntity> packages, CancellationToken token)
        {
            var output = new List<PackageDependencyGroups>();

            foreach (var package in packages)
            {
                InitializeItem(output, package);
            }

            return Task.FromResult<IReadOnlyList<PackageDependencyGroups>>(output);
        }

        private void InitializeItem(List<PackageDependencyGroups> output, PackageEntity package)
        {
            var nuspec = _nuspecProvider.GetNuspec(package.PackageRegistration.Id, package.Version);
            if (nuspec.Document == null)
            {
                return;
            }

            var identity = new PackageIdentity(package.PackageRegistration.Id, package.Version);
            var dependencyGroups = NuspecUtility.GetParsedDependencyGroups(nuspec.Document);
            var packageDependencyGroups = new PackageDependencyGroups(identity, dependencyGroups);

            output.Add(packageDependencyGroups);
        }

        public async Task ProcessBatchAsync(IReadOnlyList<PackageDependencyGroups> batch)
        {
            await _packageDependencyService.AddDependenciesAsync(batch);
        }
    }
}
