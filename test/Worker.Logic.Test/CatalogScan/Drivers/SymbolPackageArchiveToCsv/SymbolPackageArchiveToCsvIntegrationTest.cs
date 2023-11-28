// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace NuGet.Insights.Worker.SymbolPackageArchiveToCsv
{
    public class SymbolPackageArchiveToCsvIntegrationTest : BaseCatalogLeafScanToCsvIntegrationTest<SymbolPackageArchiveRecord, SymbolPackageArchiveEntry>
    {
        private const string SymbolPackageArchiveToCsvDir = nameof(SymbolPackageArchiveToCsv);
        private const string SymbolPackageArchiveToCsv_WithDeleteDir = nameof(SymbolPackageArchiveToCsv_WithDelete);

        [Fact]
        public async Task SymbolPackageArchiveToCsv()
        {
            // Arrange
            var min0 = DateTimeOffset.Parse("2021-03-22T20:13:00.3409860Z", CultureInfo.InvariantCulture);
            var max1 = DateTimeOffset.Parse("2021-03-22T20:13:54.6075418Z", CultureInfo.InvariantCulture);
            var max2 = DateTimeOffset.Parse("2021-03-22T20:15:23.6403188Z", CultureInfo.InvariantCulture);

            await CatalogScanService.InitializeAsync();
            await SetCursorAsync(CatalogScanDriverType.LoadSymbolPackageArchive, max2);
            await SetCursorAsync(min0);

            // Act
            await UpdateAsync(max1);

            // Assert
            await AssertOutputAsync(SymbolPackageArchiveToCsvDir, Step1, 0);
            await AssertCsvCountAsync(1);

            // Act
            await UpdateAsync(max2);

            // Assert
            await AssertOutputAsync(SymbolPackageArchiveToCsvDir, Step2, 0);
            await AssertOutputAsync(SymbolPackageArchiveToCsvDir, Step2, 2);
            await AssertCsvCountAsync(2);
        }

        [Fact]
        public async Task SymbolPackageArchiveToCsv_WithDelete()
        {
            // Arrange
            MakeDeletedPackageAvailable();
            var min0 = DateTimeOffset.Parse("2020-12-20T02:37:31.5269913Z", CultureInfo.InvariantCulture);
            var max1 = DateTimeOffset.Parse("2020-12-20T03:01:57.2082154Z", CultureInfo.InvariantCulture);
            var max2 = DateTimeOffset.Parse("2020-12-20T03:03:53.7885893Z", CultureInfo.InvariantCulture);

            await CatalogScanService.InitializeAsync();
            await SetCursorAsync(CatalogScanDriverType.LoadSymbolPackageArchive, max2);
            await SetCursorAsync(min0);

            // Act
            await UpdateAsync(max1);

            // Assert
            await AssertOutputAsync(SymbolPackageArchiveToCsv_WithDeleteDir, Step1, 0);
            await AssertOutputAsync(SymbolPackageArchiveToCsv_WithDeleteDir, Step1, 1);
            await AssertOutputAsync(SymbolPackageArchiveToCsv_WithDeleteDir, Step1, 2);

            // Act
            await UpdateAsync(max2);

            // Assert
            await AssertOutputAsync(SymbolPackageArchiveToCsv_WithDeleteDir, Step1, 0); // This file is unchanged.
            await AssertOutputAsync(SymbolPackageArchiveToCsv_WithDeleteDir, Step1, 1); // This file is unchanged.
            await AssertOutputAsync(SymbolPackageArchiveToCsv_WithDeleteDir, Step2, 2);
        }

        public SymbolPackageArchiveToCsvIntegrationTest(ITestOutputHelper output, DefaultWebApplicationFactory<StaticFilesStartup> factory)
            : base(output, factory)
        {
        }

        protected override string DestinationContainerName1 => Options.Value.SymbolPackageArchiveContainerName;
        protected override string DestinationContainerName2 => Options.Value.SymbolPackageArchiveEntryContainerName;
        protected override CatalogScanDriverType DriverType => CatalogScanDriverType.SymbolPackageArchiveToCsv;
        public override IEnumerable<CatalogScanDriverType> LatestLeavesTypes => new[] { DriverType };
        public override IEnumerable<CatalogScanDriverType> LatestLeavesPerIdTypes => Enumerable.Empty<CatalogScanDriverType>();

        protected override IEnumerable<string> GetExpectedCursorNames()
        {
            return base.GetExpectedCursorNames().Concat(new[]
            {
                "CatalogScan-" + CatalogScanDriverType.LoadSymbolPackageArchive,
            });
        }

        protected override IEnumerable<string> GetExpectedTableNames()
        {
            return base.GetExpectedTableNames().Concat(new[]
            {
                Options.Value.SymbolPackageArchiveTableName,
            });
        }
    }
}
