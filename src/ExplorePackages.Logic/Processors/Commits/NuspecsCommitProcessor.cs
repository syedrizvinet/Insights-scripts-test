﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Knapcode.ExplorePackages.Entities;
using Microsoft.Extensions.Logging;

namespace Knapcode.ExplorePackages.Logic
{
    public class NuspecsCommitProcessor : ICommitProcessor<PackageEntity, PackageEntity, object>
    {
        private readonly IBatchSizeProvider _batchSizeProvider;
        private readonly NuspecStore _nuspecStore;
        private readonly ILogger<NuspecsCommitProcessor> _logger;

        public NuspecsCommitProcessor(
            IBatchSizeProvider batchSizeProvider,
            NuspecStore nuspecStore,
            ILogger<NuspecsCommitProcessor> logger)
        {
            _batchSizeProvider = batchSizeProvider;
            _nuspecStore = nuspecStore;
            _logger = logger;
        }

        public string CursorName => CursorNames.Nuspecs;

        public IReadOnlyList<string> DependencyCursorNames { get; } = new[]
        {
            CursorNames.NuGetOrg.FlatContainer,
        };

        public ProcessMode ProcessMode => ProcessMode.TaskQueue;
        public int BatchSize => _batchSizeProvider.Get(BatchSizeType.Nuspecs);
        public object DeserializeProgressToken(string serializedProgressToken) => null;
        public string SerializeProgressToken(object progressToken) => null;

        public Task<ItemBatch<PackageEntity, object>> InitializeItemsAsync(
            IReadOnlyList<PackageEntity> entities,
            object progressToken,
            CancellationToken token)
        {
            return Task.FromResult(new ItemBatch<PackageEntity, object>(entities));
        }

        public async Task ProcessBatchAsync(IReadOnlyList<PackageEntity> batch)
        {
            foreach (var package in batch)
            {
                var success = await _nuspecStore.StoreNuspecAsync(
                   package.PackageRegistration.Id,
                   package.Version,
                   CancellationToken.None);

                if (!success)
                {
                    _logger.LogWarning(
                        "The .nuspec for package {Id} {Version} could not be found.",
                        package.PackageRegistration.Id,
                        package.Version);
                }
            }
        }
    }
}