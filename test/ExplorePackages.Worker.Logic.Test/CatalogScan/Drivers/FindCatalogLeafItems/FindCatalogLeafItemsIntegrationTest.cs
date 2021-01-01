﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Knapcode.ExplorePackages.Worker.FindCatalogLeafItems
{
    public class FindCatalogLeafItemsIntegrationTest : BaseCatalogScanToCsvIntegrationTest
    {
        private const string FindCatalogLeafItemsDir = nameof(FindCatalogLeafItems);
        private const string FindCatalogLeafItems_WithDuplicatesDir = nameof(FindCatalogLeafItems_WithDuplicates);

        public FindCatalogLeafItemsIntegrationTest(ITestOutputHelper output, DefaultWebApplicationFactory<StaticFilesStartup> factory)
            : base(output, factory)
        {
        }

        protected override string DestinationContainerName => Options.Value.FindCatalogLeafItemsContainerName;
        protected override CatalogScanDriverType DriverType => CatalogScanDriverType.FindCatalogLeafItems;

        public class FindCatalogLeafItems : FindCatalogLeafItemsIntegrationTest
        {
            public FindCatalogLeafItems(ITestOutputHelper output, DefaultWebApplicationFactory<StaticFilesStartup> factory)
                : base(output, factory)
            {
            }

            [Fact]
            public async Task Execute()
            {
                ConfigureWorkerSettings = x => x.AppendResultStorageBucketCount = 1;

                Logger.LogInformation("Settings: " + Environment.NewLine + JsonConvert.SerializeObject(Options.Value, Formatting.Indented));

                // Arrange
                var min0 = DateTimeOffset.Parse("2020-12-27T05:06:30.4180312Z");
                var max1 = DateTimeOffset.Parse("2020-12-27T05:07:45.7628472Z");

                await CatalogScanService.InitializeAsync();
                await SetCursorAsync(min0);

                // Act
                await UpdateAsync(max1);

                // Assert
                await AssertOutputAsync(FindCatalogLeafItemsDir, Step1, 0);

                await VerifyExpectedStorageAsync();
            }
        }

        public class FindCatalogLeafItems_WithDuplicates : FindCatalogLeafItemsIntegrationTest
        {
            public FindCatalogLeafItems_WithDuplicates(ITestOutputHelper output, DefaultWebApplicationFactory<StaticFilesStartup> factory)
                : base(output, factory)
            {
            }

            [Fact]
            public async Task Execute()
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
                await AssertOutputAsync(FindCatalogLeafItems_WithDuplicatesDir, Step1, 0);

                await VerifyExpectedStorageAsync();
            }
        }
    }
}
