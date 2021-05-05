﻿using System;
using System.IO;
using System.Threading.Tasks;
using Knapcode.ExplorePackages.Worker.StreamWriterUpdater;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
using Xunit.Abstractions;

namespace Knapcode.ExplorePackages.Worker.DownloadsToCsv
{
    public class DownloadsToCsvIntegrationTest : BaseWorkerLogicIntegrationTest
    {
        private const string DownloadsToCsvDir = nameof(DownloadsToCsv);
        private const string DownloadsToCsv_NonExistentVersionDir = nameof(DownloadsToCsv_NonExistentVersion);
        private const string DownloadsToCsv_UncheckedIdAndVersionDir = nameof(DownloadsToCsv_UncheckedIdAndVersion);

        public DownloadsToCsvIntegrationTest(ITestOutputHelper output, DefaultWebApplicationFactory<StaticFilesStartup> factory) : base(output, factory)
        {
            MockVersionSet.SetReturnsDefault(true);
        }

        protected override void ConfigureHostBuilder(IHostBuilder hostBuilder)
        {
            base.ConfigureHostBuilder(hostBuilder);

            hostBuilder.ConfigureServices(serviceCollection =>
            {
                serviceCollection.AddTransient(s => MockVersionSetProvider.Object);
            });
        }

        public class DownloadsToCsv : DownloadsToCsvIntegrationTest
        {
            public DownloadsToCsv(ITestOutputHelper output, DefaultWebApplicationFactory<StaticFilesStartup> factory) : base(output, factory)
            {
            }

            [Fact]
            public async Task ExecuteAsync()
            {
                // Arrange
                ConfigureWorkerSettings = x => x.OnlyKeepLatestInStreamWriterUpdater = false;
                ConfigureAndSetLastModified();
                var service = Host.Services.GetRequiredService<IStreamWriterUpdaterService<PackageDownloadSet>>();
                await service.InitializeAsync();
                await service.StartAsync(loop: false, notBefore: TimeSpan.Zero);

                // Act
                await ProcessQueueAsync(service);

                // Assert
                await AssertCsvBlobAsync(DownloadsToCsvDir, Step1, "downloads_08585909596854775807.csv.gz");
                await AssertCsvBlobAsync(DownloadsToCsvDir, Step1, "latest_downloads.csv.gz");

                // Arrange
                SetData(Step2);
                await service.StartAsync(loop: false, notBefore: TimeSpan.Zero);

                // Act
                await ProcessQueueAsync(service);

                // Assert
                await AssertBlobCountAsync(Options.Value.PackageDownloadsContainerName, 3);
                await AssertCsvBlobAsync(DownloadsToCsvDir, Step1, "downloads_08585909596854775807.csv.gz");
                await AssertCsvBlobAsync(DownloadsToCsvDir, Step2, "downloads_08585908696854775807.csv.gz");
                await AssertCsvBlobAsync(DownloadsToCsvDir, Step2, "latest_downloads.csv.gz");
            }
        }

        public class DownloadsToCsv_NonExistentVersion : DownloadsToCsvIntegrationTest
        {
            public DownloadsToCsv_NonExistentVersion(ITestOutputHelper output, DefaultWebApplicationFactory<StaticFilesStartup> factory) : base(output, factory)
            {
            }

            [Fact]
            public async Task ExecuteAsync()
            {
                // Arrange
                ConfigureWorkerSettings = x => x.OnlyKeepLatestInStreamWriterUpdater = false;
                ConfigureAndSetLastModified();
                var service = Host.Services.GetRequiredService<IStreamWriterUpdaterService<PackageDownloadSet>>();
                await service.InitializeAsync();
                await service.StartAsync(loop: false, notBefore: TimeSpan.Zero);
                MockVersionSet.Setup(x => x.DidVersionEverExist("Knapcode.TorSharp", "2.0.7")).Returns(false);
                MockVersionSet.Setup(x => x.DidVersionEverExist("Newtonsoft.Json", "10.5.0")).Returns(false);

                // Act
                await ProcessQueueAsync(service);

                // Assert
                await AssertCsvBlobAsync(DownloadsToCsv_NonExistentVersionDir, Step1, "downloads_08585909596854775807.csv.gz");
                await AssertCsvBlobAsync(DownloadsToCsv_NonExistentVersionDir, Step1, "latest_downloads.csv.gz");

                // Arrange
                SetData(Step2);
                await service.StartAsync(loop: false, notBefore: TimeSpan.Zero);
                MockVersionSet.Setup(x => x.DidVersionEverExist("Knapcode.TorSharp", "2.0.7")).Returns(true);

                // Act
                await ProcessQueueAsync(service);

                // Assert
                await AssertBlobCountAsync(Options.Value.PackageDownloadsContainerName, 3);
                await AssertCsvBlobAsync(DownloadsToCsv_NonExistentVersionDir, Step1, "downloads_08585909596854775807.csv.gz");
                await AssertCsvBlobAsync(DownloadsToCsv_NonExistentVersionDir, Step2, "downloads_08585908696854775807.csv.gz");
                await AssertCsvBlobAsync(DownloadsToCsv_NonExistentVersionDir, Step2, "latest_downloads.csv.gz");
            }
        }

        public class DownloadsToCsv_UncheckedIdAndVersion : DownloadsToCsvIntegrationTest
        {
            public DownloadsToCsv_UncheckedIdAndVersion(ITestOutputHelper output, DefaultWebApplicationFactory<StaticFilesStartup> factory) : base(output, factory)
            {
            }

            [Fact]
            public async Task ExecuteAsync()
            {
                // Arrange
                ConfigureWorkerSettings = x => x.OnlyKeepLatestInStreamWriterUpdater = false;
                ConfigureAndSetLastModified();
                var service = Host.Services.GetRequiredService<IStreamWriterUpdaterService<PackageDownloadSet>>();
                await service.InitializeAsync();
                await service.StartAsync(loop: false, notBefore: TimeSpan.Zero);
                MockVersionSet.Setup(x => x.DidVersionEverExist("Knapcode.TorSharp", "2.0.7")).Returns(false);
                MockVersionSet.Setup(x => x.GetUncheckedIds()).Returns(new[] { "UncheckedB", "UncheckedA", "Knapcode.TorSharp" });
                MockVersionSet.Setup(x => x.GetUncheckedVersions("UncheckedA")).Returns(new[] { "2.0.0", "1.0.0" });
                MockVersionSet.Setup(x => x.GetUncheckedVersions("UncheckedB")).Returns(new[] { "3.0.0" });
                MockVersionSet.Setup(x => x.GetUncheckedVersions("Knapcode.TorSharp")).Returns(new[] { "0.0.1" });

                // Act
                await ProcessQueueAsync(service);

                // Assert
                await AssertCsvBlobAsync(DownloadsToCsv_UncheckedIdAndVersionDir, Step1, "downloads_08585909596854775807.csv.gz");
                await AssertCsvBlobAsync(DownloadsToCsv_UncheckedIdAndVersionDir, Step1, "latest_downloads.csv.gz");
            }
        }

        public class DownloadsToCsv_JustLatest : DownloadsToCsvIntegrationTest
        {
            public DownloadsToCsv_JustLatest(ITestOutputHelper output, DefaultWebApplicationFactory<StaticFilesStartup> factory) : base(output, factory)
            {
            }

            [Fact]
            public async Task ExecuteAsync()
            {
                // Arrange
                ConfigureAndSetLastModified();
                var service = Host.Services.GetRequiredService<IStreamWriterUpdaterService<PackageDownloadSet>>();
                await service.InitializeAsync();
                await service.StartAsync(loop: false, notBefore: TimeSpan.Zero);

                // Act
                await ProcessQueueAsync(service);

                // Assert
                await AssertCsvBlobAsync(DownloadsToCsvDir, Step1, "latest_downloads.csv.gz");

                // Arrange
                SetData(Step2);
                await service.StartAsync(loop: false, notBefore: TimeSpan.Zero);

                // Act
                await ProcessQueueAsync(service);

                // Assert
                await AssertBlobCountAsync(Options.Value.PackageDownloadsContainerName, 1);
                await AssertCsvBlobAsync(DownloadsToCsvDir, Step2, "latest_downloads.csv.gz");
            }
        }

        private async Task ProcessQueueAsync(IStreamWriterUpdaterService<PackageDownloadSet> service)
        {
            await ProcessQueueAsync(() => { }, async () => !await service.IsRunningAsync());
        }

        private void ConfigureAndSetLastModified()
        {
            ConfigureSettings = x => x.DownloadsV1Url = $"http://localhost/{TestData}/{DownloadsToCsvDir}/downloads.v1.json";

            // Set the Last-Modified date
            var fileA = new FileInfo(Path.Combine(TestData, DownloadsToCsvDir, Step1, "downloads.v1.json"))
            {
                LastWriteTimeUtc = DateTime.Parse("2021-01-14T18:00:00Z")
            };
            var fileB = new FileInfo(Path.Combine(TestData, DownloadsToCsvDir, Step2, "downloads.v1.json"))
            {
                LastWriteTimeUtc = DateTime.Parse("2021-01-15T19:00:00Z")
            };

            SetData(Step1);
        }

        private void SetData(string stepName)
        {
            HttpMessageHandlerFactory.OnSendAsync = async req =>
            {
                if (req.RequestUri.AbsolutePath.EndsWith("/downloads.v1.json"))
                {
                    var newReq = Clone(req);
                    newReq.RequestUri = new Uri($"http://localhost/{TestData}/{DownloadsToCsvDir}/{stepName}/downloads.v1.json");
                    return await TestDataHttpClient.SendAsync(newReq);
                }

                return null;
            };
        }

        private Task AssertCsvBlobAsync(string testName, string stepName, string blobName)
        {
            return AssertCsvBlobAsync<PackageDownloadRecord>(Options.Value.PackageDownloadsContainerName, testName, stepName, "latest_downloads.csv", blobName);
        }
    }
}