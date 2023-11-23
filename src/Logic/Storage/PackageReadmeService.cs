// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuGet.Protocol;
using NuGet.Versioning;

#nullable enable

namespace NuGet.Insights
{
    public class PackageReadmeService
    {
        private readonly PackageWideEntityService _wideEntityService;
        private readonly FlatContainerClient _flatContainerClient;
        private readonly HttpSource _httpSource;
        private readonly ITelemetryClient _telemetryClient;
        private readonly IOptions<NuGetInsightsSettings> _options;
        private readonly ILogger<PackageReadmeService> _logger;

        public PackageReadmeService(
            PackageWideEntityService wideEntityService,
            FlatContainerClient flatContainerClient,
            HttpSource httpSource,
            ITelemetryClient telemetryClient,
            IOptions<NuGetInsightsSettings> options,
            ILogger<PackageReadmeService> logger)
        {
            _wideEntityService = wideEntityService;
            _flatContainerClient = flatContainerClient;
            _httpSource = httpSource;
            _telemetryClient = telemetryClient;
            _options = options;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            await _wideEntityService.InitializeAsync(_options.Value.PackageReadmeTableName);
        }

        public async Task DestroyAsync()
        {
            await _wideEntityService.DeleteTableAsync(_options.Value.PackageReadmeTableName);
        }

        public async Task<IReadOnlyDictionary<IPackageIdentityCommit, PackageReadmeInfoV1>> UpdateBatchFromLeafItemsAsync(
            string id,
            IReadOnlyCollection<IPackageIdentityCommit> leafItems)
        {
            return await _wideEntityService.UpdateBatchAsync(
                _options.Value.PackageReadmeTableName,
                id,
                leafItems,
                GetInfoFromLeafItemAsync,
                OutputToData,
                DataToOutput);
        }

        public async Task<PackageReadmeInfoV1> GetOrUpdateInfoFromLeafItemAsync(IPackageIdentityCommit leafItem)
        {
            return await _wideEntityService.GetOrUpdateInfoAsync(
                _options.Value.PackageReadmeTableName,
                leafItem,
                GetInfoFromLeafItemAsync,
                OutputToData,
                DataToOutput);
        }

        private async Task<PackageReadmeInfoV1> GetInfoFromLeafItemAsync(IPackageIdentityCommit leafItem)
        {
            if (leafItem.LeafType == CatalogLeafType.PackageDelete)
            {
                return MakeUnavailableInfo(leafItem);
            }

            return await GetInfoAsync(leafItem);
        }

        private async Task<PackageReadmeInfoV1> GetInfoAsync(IPackageIdentityCommit item)
        {
            var embeddedUrl = await _flatContainerClient.GetPackageReadmeUrlAsync(item.PackageId, item.PackageVersion);

            var urls = new List<(ReadmeType, string)> { (ReadmeType.Embedded, embeddedUrl) };

            if (_options.Value.LegacyReadmeUrlPattern != null)
            {
                var lowerId = item.PackageId.ToLowerInvariant();
                var lowerVersion = NuGetVersion.Parse(item.PackageVersion).ToNormalizedString().ToLowerInvariant();
                var legacyUrl = string.Format(CultureInfo.InvariantCulture, _options.Value.LegacyReadmeUrlPattern, lowerId, lowerVersion);

                urls.Add((ReadmeType.Legacy, legacyUrl));
            }

            var metric = _telemetryClient.GetMetric($"{nameof(PackageReadmeService)}.{nameof(GetInfoAsync)}.DurationMs");
            var sw = Stopwatch.StartNew();
            var nuGetLog = _logger.ToNuGetLogger();

            try
            {
                foreach ((var readmeType, var url) in urls)
                {
                    var result = await _httpSource.ProcessResponseWithRetryAsync(
                        new HttpSourceRequest(() => HttpRequestMessageFactory.Create(HttpMethod.Get, url, nuGetLog))
                        {
                            IgnoreNotFounds = true
                        },
                        async response =>
                        {
                            if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                return null;
                            }

                            response.EnsureSuccessStatusCode();

                            using var destStream = new MemoryStream();
                            using var responseStream = await response.Content.ReadAsStreamAsync();
                            await responseStream.CopyToAsync(destStream);

                            var headers = Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>()
                                .Concat(response.Headers)
                                .Concat(response.Content.Headers)
                                .SelectMany(x => x.Value.Select(y => new { x.Key, Value = y }))
                                .ToLookup(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);

                            return new PackageReadmeInfoV1
                            {
                                CommitTimestamp = item.CommitTimestamp,
                                ReadmeType = readmeType,
                                HttpHeaders = headers,
                                ReadmeBytes = new Memory<byte>(destStream.GetBuffer(), 0, (int)destStream.Length),
                            };
                        },
                        _logger,
                        token: CancellationToken.None);

                    if (result is not null)
                    {
                        return result;
                    }
                }

                return MakeUnavailableInfo(item);
            }
            finally
            {
                metric.TrackValue(sw.ElapsedMilliseconds);
            }
        }

        private static PackageReadmeInfoV1 MakeUnavailableInfo(IPackageIdentityCommit item)
        {
            return new PackageReadmeInfoV1
            {
                CommitTimestamp = item.CommitTimestamp,
                ReadmeType = ReadmeType.None,
            };
        }

        private static PackageReadmeInfoV1 DataToOutput(PackageReadmeInfoVersions data)
        {
            return data.V1;
        }

        private static PackageReadmeInfoVersions OutputToData(PackageReadmeInfoV1 output)
        {
            return new PackageReadmeInfoVersions(output);
        }

        [MessagePackObject]
        public class PackageReadmeInfoVersions : PackageWideEntityService.IPackageWideEntity
        {
            [SerializationConstructor]
            public PackageReadmeInfoVersions(PackageReadmeInfoV1 v1)
            {
                V1 = v1;
            }

            [Key(0)]
            public PackageReadmeInfoV1 V1 { get; set; }

            DateTimeOffset? PackageWideEntityService.IPackageWideEntity.CommitTimestamp => V1.CommitTimestamp;
        }

        [MessagePackObject]
        public class PackageReadmeInfoV1
        {
            [Key(0)]
            public DateTimeOffset? CommitTimestamp { get; set; }

            [Key(1)]
            public ReadmeType ReadmeType { get; set; }

            [Key(2)]
            public ILookup<string, string>? HttpHeaders { get; set; }

            [Key(3)]
            public Memory<byte> ReadmeBytes { get; set; }
        }
    }
}
