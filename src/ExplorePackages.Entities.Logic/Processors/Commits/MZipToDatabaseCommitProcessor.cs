﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Knapcode.ExplorePackages.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Knapcode.ExplorePackages.Logic
{
    public class MZipToDatabaseCommitProcessor : ICommitProcessor<PackageEntity, PackageArchiveMetadata, object>
    {
        private readonly MZipStore _mZipStore;
        private readonly IPackageService _packageService;
        private readonly IBatchSizeProvider _batchSizeProvider;
        private readonly IOptionsSnapshot<ExplorePackagesEntitiesSettings> _options;

        public MZipToDatabaseCommitProcessor(
            MZipStore mZipStore,
            IPackageService packageService,
            IBatchSizeProvider batchSizeProvider,
            IOptionsSnapshot<ExplorePackagesEntitiesSettings> options)
        {
            _mZipStore = mZipStore;
            _packageService = packageService;
            _batchSizeProvider = batchSizeProvider;
            _options = options;
        }

        public string CursorName => CursorNames.MZipToDatabase;

        public IReadOnlyList<string> DependencyCursorNames { get; } = new[]
        {
            CursorNames.MZips,
        };

        public ProcessMode ProcessMode => ProcessMode.Sequentially;
        public int BatchSize => _batchSizeProvider.Get(BatchSizeType.MZipToDatabase);
        public string SerializeProgressToken(object progressToken) => null;
        public object DeserializeProgressToken(string serializedProgressToken) => null;

        public async Task<ItemBatch<PackageArchiveMetadata, object>> InitializeItemsAsync(
            IReadOnlyList<PackageEntity> packages,
            object progressToken,
            CancellationToken token)
        {
            var output = await TaskProcessor.ExecuteAsync(
                packages,
                InitializeItemAsync,
                workerCount: _options.Value.WorkerCount,
                token: token);

            var list = output.Where(x => x != null).ToList();

            return new ItemBatch<PackageArchiveMetadata, object>(list);
        }

        private async Task<PackageArchiveMetadata> InitializeItemAsync(PackageEntity package)
        {
            // Read the .zip directory.
            var context = await _mZipStore.GetMZipContextAsync(
                package.PackageRegistration.Id,
                package.Version);

            if (!context.Exists)
            {
                if (!package.CatalogPackage.Deleted)
                {
                    throw new InvalidOperationException($"Could not find .mzip for {package.PackageRegistration.Id} {package.Version}.");
                }

                return null;
            }

            // Gather the metadata.
            var metadata = new PackageArchiveMetadata
            {
                Id = package.PackageRegistration.Id,
                Version = package.Version,
                Size = context.Size.Value,
                ZipDirectory = context.ZipDirectory,
            };

            return metadata;
        }

        public async Task ProcessBatchAsync(IReadOnlyList<PackageArchiveMetadata> batch)
        {
            await _packageService.AddOrUpdatePackagesAsync(batch);
        }

        public class Collector : CommitCollector<PackageEntity, PackageArchiveMetadata, object>
        {
            public Collector(
                CursorService cursorService,
                ICommitEnumerator<PackageEntity> enumerator,
                MZipToDatabaseCommitProcessor processor,
                CommitCollectorSequentialProgressService sequentialProgressService,
                ISingletonService singletonService,
                IOptionsSnapshot<ExplorePackagesEntitiesSettings> options,
                ILogger<Collector> logger) : base(
                    cursorService,
                    enumerator,
                    processor,
                    sequentialProgressService,
                    singletonService,
                    options,
                    logger)
            {
            }
        }
    }
}
