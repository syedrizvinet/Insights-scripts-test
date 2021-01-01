﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Knapcode.ExplorePackages.Worker.FindPackageAssets
{
    public class FindPackageAssetsIntegrationTest : BaseCatalogLeafScanToCsvIntegrationTest
    {
        private const string FindPackageAssetsDir = nameof(FindPackageAssets);
        private const string FindPackageAssets_WithDeleteDir = nameof(FindPackageAssets_WithDelete);
        private const string FindPackageAssets_WithDuplicatesDir = nameof(FindPackageAssets_WithDuplicates);

        public FindPackageAssetsIntegrationTest(ITestOutputHelper output, DefaultWebApplicationFactory<StaticFilesStartup> factory)
            : base(output, factory)
        {
        }

        protected override string DestinationContainerName => Options.Value.FindPackageAssetsContainerName;
        protected override CatalogScanDriverType DriverType => CatalogScanDriverType.FindPackageAssets;

        public class FindPackageAssets : FindPackageAssetsIntegrationTest
        {
            public FindPackageAssets(ITestOutputHelper output, DefaultWebApplicationFactory<StaticFilesStartup> factory)
                : base(output, factory)
            {
            }

            [Fact]
            public async Task Execute()
            {
                Logger.LogInformation("Settings: " + Environment.NewLine + JsonConvert.SerializeObject(Options.Value, Formatting.Indented));

                // Arrange
                var min0 = DateTimeOffset.Parse("2020-11-27T19:34:24.4257168Z");
                var max1 = DateTimeOffset.Parse("2020-11-27T19:35:06.0046046Z");
                var max2 = DateTimeOffset.Parse("2020-11-27T19:36:50.4909042Z");

                await CatalogScanService.InitializeAsync();
                await SetCursorAsync(min0);

                // Act
                await UpdateAsync(max1);

                // Assert
                await AssertOutputAsync(FindPackageAssetsDir, Step1, 0);
                await AssertOutputAsync(FindPackageAssetsDir, Step1, 1);
                await AssertOutputAsync(FindPackageAssetsDir, Step1, 2);

                // Act
                await UpdateAsync(max2);

                // Assert
                await AssertOutputAsync(FindPackageAssetsDir, Step2, 0);
                await AssertOutputAsync(FindPackageAssetsDir, Step1, 1); // This file is unchanged.
                await AssertOutputAsync(FindPackageAssetsDir, Step2, 2);

                await VerifyExpectedStorageAsync();
            }
        }

        public class FindPackageAssets_WithoutBatching : FindPackageAssetsIntegrationTest
        {
            public FindPackageAssets_WithoutBatching(ITestOutputHelper output, DefaultWebApplicationFactory<StaticFilesStartup> factory)
                : base(output, factory)
            {
            }

            [Fact]
            public async Task Execute()
            {
                ConfigureWorkerSettings = x => x.AllowBatching = false;

                Logger.LogInformation("Settings: " + Environment.NewLine + JsonConvert.SerializeObject(Options.Value, Formatting.Indented));

                // Arrange
                var min0 = DateTimeOffset.Parse("2020-11-27T19:34:24.4257168Z");
                var max1 = DateTimeOffset.Parse("2020-11-27T19:35:06.0046046Z");

                await CatalogScanService.InitializeAsync();
                await SetCursorAsync(min0);

                // Act
                await UpdateAsync(max1);

                // Assert
                await AssertOutputAsync(FindPackageAssetsDir, Step1, 0);
                await AssertOutputAsync(FindPackageAssetsDir, Step1, 1);
                await AssertOutputAsync(FindPackageAssetsDir, Step1, 2);

                await VerifyExpectedStorageAsync();
            }
        }

        public class FindPackageAssets_WithDelete : FindPackageAssetsIntegrationTest
        {
            public FindPackageAssets_WithDelete(ITestOutputHelper output, DefaultWebApplicationFactory<StaticFilesStartup> factory)
                : base(output, factory)
            {
            }

            [Fact]
            public async Task Execute()
            {
                Logger.LogInformation("Settings: " + Environment.NewLine + JsonConvert.SerializeObject(Options.Value, Formatting.Indented));

                // Arrange
                HttpMessageHandlerFactory.OnSendAsync = async req =>
                {
                    if (req.RequestUri.AbsolutePath.EndsWith("/behaviorsample.1.0.0.nupkg"))
                    {
                        var newReq = Clone(req);
                        newReq.RequestUri = new Uri($"http://localhost/{TestData}/behaviorsample.1.0.0.nupkg");
                        return await TestDataHttpClient.SendAsync(newReq);
                    }

                    return null;
                };
                var min0 = DateTimeOffset.Parse("2020-12-20T02:37:31.5269913Z");
                var max1 = DateTimeOffset.Parse("2020-12-20T03:01:57.2082154Z");
                var max2 = DateTimeOffset.Parse("2020-12-20T03:03:53.7885893Z");

                await CatalogScanService.InitializeAsync();
                await SetCursorAsync(min0);

                // Act
                await UpdateAsync(max1);

                // Assert
                await AssertOutputAsync(FindPackageAssets_WithDeleteDir, Step1, 0);
                await AssertOutputAsync(FindPackageAssets_WithDeleteDir, Step1, 1);
                await AssertOutputAsync(FindPackageAssets_WithDeleteDir, Step1, 2);

                // Act
                await UpdateAsync(max2);

                // Assert
                await AssertOutputAsync(FindPackageAssets_WithDeleteDir, Step1, 0); // This file is unchanged.
                await AssertOutputAsync(FindPackageAssets_WithDeleteDir, Step1, 1); // This file is unchanged.
                await AssertOutputAsync(FindPackageAssets_WithDeleteDir, Step2, 2);

                await VerifyExpectedStorageAsync();
            }
        }

        public class FindPackageAssets_WithDuplicates_OnlyLatestLeaves : FindPackageAssetsIntegrationTest
        {
            public FindPackageAssets_WithDuplicates_OnlyLatestLeaves(ITestOutputHelper output, DefaultWebApplicationFactory<StaticFilesStartup> factory)
                : base(output, factory)
            {
            }

            public override bool OnlyLatestLeaves => true;

            [Fact]
            public Task Execute() => FindPackageAssets_WithDuplicates();
        }

        public class FindPackageAssets_WithDuplicates_AllLeaves : FindPackageAssetsIntegrationTest
        {
            public FindPackageAssets_WithDuplicates_AllLeaves(ITestOutputHelper output, DefaultWebApplicationFactory<StaticFilesStartup> factory)
                : base(output, factory)
            {
            }

            public override bool OnlyLatestLeaves => false;

            [Fact]
            public Task Execute() => FindPackageAssets_WithDuplicates();
        }

        private async Task FindPackageAssets_WithDuplicates()
        {
            ConfigureWorkerSettings = x => x.AppendResultStorageBucketCount = 1;

            Logger.LogInformation("Settings: " + Environment.NewLine + JsonConvert.SerializeObject(Options.Value, Formatting.Indented));

            // Arrange
            var min0 = DateTimeOffset.Parse("2020-11-27T21:58:12.5094058Z");
            var max1 = DateTimeOffset.Parse("2020-11-27T22:09:56.3587144Z");

            await CatalogScanService.InitializeAsync();
            await SetCursorAsync(min0);

            // Act
            await UpdateAsync(max1);

            // Assert
            await AssertOutputAsync(FindPackageAssets_WithDuplicatesDir, Step1, 0);

            var duplicatePackageRequests = HttpMessageHandlerFactory
                .Requests
                .Where(x => x.RequestUri.AbsolutePath.EndsWith("/gosms.ge-sms-api.1.0.1.nupkg"))
                .ToList();
            Assert.Equal(OnlyLatestLeaves ? 1 : 2, duplicatePackageRequests.Where(x => x.Method == HttpMethod.Head).Count());
            Assert.Equal(OnlyLatestLeaves ? 1 : 2, duplicatePackageRequests.Where(x => x.Method == HttpMethod.Get).Count());

            await VerifyExpectedStorageAsync();
        }
    }
}