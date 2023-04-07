// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Sas;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Kusto.Data.Common;
using Kusto.Data.Net.Client;
using Kusto.Ingest;
using MessagePack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using NuGet.Insights.ReferenceTracking;
using NuGet.Insights.WideEntities;
using NuGet.Insights.Worker.BuildVersionSet;
using NuGet.Insights.Worker.KustoIngestion;
using NuGet.Insights.Worker.Workflow;
using NuGet.Versioning;
using Xunit;
using Xunit.Abstractions;

namespace NuGet.Insights.Worker
{
    public abstract class BaseWorkerLogicIntegrationTest : BaseLogicIntegrationTest
    {
        public delegate void TryGetId(string id, out string outId);
        public delegate void TryGetVersion(string id, string version, out string outVersion);

        protected BaseWorkerLogicIntegrationTest(ITestOutputHelper output, DefaultWebApplicationFactory<StaticFilesStartup> factory) : base(output, factory)
        {
            // Version set
            MockVersionSet.Setup(x => x.GetUncheckedIds()).Returns(Array.Empty<string>());
            MockVersionSet.Setup(x => x.GetUncheckedVersions(It.IsAny<string>())).Returns(Array.Empty<string>());
            MockVersionSetProvider.Setup(x => x.GetAsync()).ReturnsAsync(() => EntityHandle.Create(MockVersionSet.Object));

            // Kusto SDK
            MockCslAdminProvider = new Mock<ICslAdminProvider>();
            MockKustoQueueIngestClient = new Mock<IKustoQueuedIngestClient>();
            MockKustoQueueIngestClient
                .Setup(x => x.IngestFromStorageAsync(
                    It.IsAny<string>(),
                    It.IsAny<KustoIngestionProperties>(),
                    It.IsAny<StorageSourceOptions>()))
                .Returns<string, KustoIngestionProperties, StorageSourceOptions>(async (u, p, o) =>
                {
                    return await MakeTableReportIngestionResultAsync(o, Status.Succeeded);
                });
            MockCslQueryProvider = new Mock<ICslQueryProvider>();
            MockCslQueryProvider
                .Setup(x => x.ExecuteQueryAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ClientRequestProperties>()))
                .ReturnsAsync(() =>
                {
                    var mockReader = new Mock<IDataReader>();
                    mockReader.SetupSequence(x => x.Read()).Returns(true).Returns(false);
                    mockReader.Setup(x => x.GetInt64(It.IsAny<int>())).Returns(0);
                    mockReader.Setup(x => x.GetValue(It.IsAny<int>())).Returns(new JValue((object)null));
                    return mockReader.Object;
                });
        }

        public Action<NuGetInsightsWorkerSettings> ConfigureWorkerSettings { get; set; }
        public IOptions<NuGetInsightsWorkerSettings> Options => Host.Services.GetRequiredService<IOptions<NuGetInsightsWorkerSettings>>();
        public CatalogScanService CatalogScanService => Host.Services.GetRequiredService<CatalogScanService>();
        public CatalogScanCursorService CatalogScanCursorService => Host.Services.GetRequiredService<CatalogScanCursorService>();
        public CursorStorageService CursorStorageService => Host.Services.GetRequiredService<CursorStorageService>();
        public CatalogScanStorageService CatalogScanStorageService => Host.Services.GetRequiredService<CatalogScanStorageService>();
        public TaskStateStorageService TaskStateStorageService => Host.Services.GetRequiredService<TaskStateStorageService>();
        public KustoIngestionService KustoIngestionService => Host.Services.GetRequiredService<KustoIngestionService>();
        public KustoIngestionStorageService KustoIngestionStorageService => Host.Services.GetRequiredService<KustoIngestionStorageService>();
        public WorkflowService WorkflowService => Host.Services.GetRequiredService<WorkflowService>();
        public WorkflowStorageService WorkflowStorageService => Host.Services.GetRequiredService<WorkflowStorageService>();
        public IMessageEnqueuer MessageEnqueuer => Host.Services.GetRequiredService<IMessageEnqueuer>();
        public IWorkerQueueFactory WorkerQueueFactory => Host.Services.GetRequiredService<IWorkerQueueFactory>();

        public Mock<IVersionSetProvider> MockVersionSetProvider { get; } = new Mock<IVersionSetProvider>();
        public Mock<IVersionSet> MockVersionSet { get; } = new Mock<IVersionSet>();
        public Mock<ICslAdminProvider> MockCslAdminProvider { get; }
        public Mock<IKustoQueuedIngestClient> MockKustoQueueIngestClient { get; }
        public Mock<ICslQueryProvider> MockCslQueryProvider { get; }

        protected override void ConfigureHostBuilder(IHostBuilder hostBuilder)
        {
            base.ConfigureHostBuilder(hostBuilder);

            hostBuilder.ConfigureServices(serviceCollection =>
            {
                serviceCollection.AddNuGetInsightsWorker();
                serviceCollection.Configure((Action<NuGetInsightsWorkerSettings>)ConfigureWorkerDefaultsAndSettings);
            });
        }

        protected void ConfigureWorkerDefaultsAndSettings(NuGetInsightsWorkerSettings x)
        {
            x.DisableMessageDelay = true;
            x.AppendResultStorageBucketCount = 3;
            x.KustoDatabaseName = "TestKustoDb";
            x.PackageContentFileExtensions = new List<string> { ".txt" };

            x.WorkQueueName = $"{StoragePrefix}1wq1";
            x.ExpandQueueName = $"{StoragePrefix}1eq1";
            x.CursorTableName = $"{StoragePrefix}1c1";
            x.CatalogIndexScanTableName = $"{StoragePrefix}1cis1";
            x.CatalogPageScanTableName = $"{StoragePrefix}1cps1";
            x.CatalogLeafScanTableName = $"{StoragePrefix}1cls1";
            x.TaskStateTableName = $"{StoragePrefix}1ts1";
            x.CsvRecordTableName = $"{StoragePrefix}1cr1";
            x.VersionSetAggregateTableName = $"{StoragePrefix}1vsa1";
            x.VersionSetContainerName = $"{StoragePrefix}1vs1";
            x.KustoIngestionTableName = $"{StoragePrefix}1ki1";
            x.LatestPackageLeafTableName = $"{StoragePrefix}1lpl1";
            x.PackageVersionTableName = $"{StoragePrefix}1pv1";
            x.WorkflowRunTableName = $"{StoragePrefix}1wr1";
            x.PackageVersionContainerName = $"{StoragePrefix}1pvc1";
            x.PackageAssetContainerName = $"{StoragePrefix}1fpa1";
            x.PackageAssemblyContainerName = $"{StoragePrefix}1fpi1";
            x.PackageManifestContainerName = $"{StoragePrefix}1pm2c1";
            x.PackageReadmeContainerName = $"{StoragePrefix}1pmd2c1";
            x.PackageSignatureContainerName = $"{StoragePrefix}1fps1";
            x.CatalogLeafItemContainerName = $"{StoragePrefix}1fcli1";
            x.PackageDownloadContainerName = $"{StoragePrefix}1pd1";
            x.PackageOwnerContainerName = $"{StoragePrefix}1po1";
            x.VerifiedPackageContainerName = $"{StoragePrefix}1vp1";
            x.PackageArchiveContainerName = $"{StoragePrefix}1pa2c1";
            x.PackageArchiveEntryContainerName = $"{StoragePrefix}1pae2c1";
            x.SymbolPackageArchiveContainerName = $"{StoragePrefix}1sa2c1";
            x.SymbolPackageArchiveEntryContainerName = $"{StoragePrefix}1sae2c1";
            x.NuGetPackageExplorerContainerName = $"{StoragePrefix}1npe2c1";
            x.NuGetPackageExplorerFileContainerName = $"{StoragePrefix}1npef2c1";
            x.PackageDeprecationContainerName = $"{StoragePrefix}1pe1";
            x.PackageVulnerabilityContainerName = $"{StoragePrefix}1pu1";
            x.PackageIconContainerName = $"{StoragePrefix}1pi1";
            x.PackageCompatibilityContainerName = $"{StoragePrefix}1pc1";
            x.PackageCertificateContainerName = $"{StoragePrefix}1pr1";
            x.CertificateContainerName = $"{StoragePrefix}1r1";
            x.PackageContentContainerName = $"{StoragePrefix}1pco1";

            ConfigureDefaultsAndSettings(x);

            if (ConfigureWorkerSettings != null)
            {
                ConfigureWorkerSettings(x);
            }

            AssertStoragePrefix(x);
        }

        protected void SetupDefaultMockVersionSet()
        {
            string anyOutId;
            string anyOutVersion;
            MockVersionSet
                .Setup(x => x.TryGetId(It.IsAny<string>(), out anyOutId))
                .Returns(true)
                .Callback(new TryGetId((string id, out string outId) => outId = id));
            MockVersionSet
                .Setup(x => x.TryGetVersion(It.IsAny<string>(), It.IsAny<string>(), out anyOutVersion))
                .Returns(true)
                .Callback(new TryGetVersion((string id, string version, out string outVersion) => outVersion = version));
        }

        protected async Task SetCursorAsync(CatalogScanDriverType driverType, DateTimeOffset min)
        {
            var cursor = await CatalogScanCursorService.GetCursorAsync(driverType);
            cursor.Value = min;
            await CursorStorageService.UpdateAsync(cursor);
        }

        public ConcurrentBag<CatalogIndexScan> ExpectedCatalogIndexScans { get; } = new ConcurrentBag<CatalogIndexScan>();

        protected async Task<CatalogIndexScan> UpdateAsync(CatalogScanDriverType driverType, DateTimeOffset max)
        {
            return await UpdateAsync(driverType, null, max);
        }

        protected async Task<CatalogIndexScan> UpdateAsync(CatalogScanDriverType driverType, bool? onlyLatestLeaves, DateTimeOffset max)
        {
            var result = await CatalogScanService.UpdateAsync(driverType, max, onlyLatestLeaves);
            return await UpdateAsync(result.Scan);
        }

        protected async Task<WorkflowRun> UpdateAsync(WorkflowRun run)
        {
            Assert.NotNull(run);
            await ProcessQueueAsync(async () =>
            {
                run = await WorkflowStorageService.GetRunAsync(run.GetRunId());

                if (run.State != WorkflowRunState.Complete)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    return false;
                }

                return true;
            });

            return run;
        }

        protected async Task<KustoIngestionEntity> UpdateAsync(KustoIngestionEntity ingestion)
        {
            Assert.NotNull(ingestion);
            await ProcessQueueAsync(async () =>
            {
                ingestion = await KustoIngestionStorageService.GetIngestionAsync(ingestion.GetIngestionId());

                if (ingestion.State != KustoIngestionState.Complete && ingestion.State != KustoIngestionState.FailedValidation)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    return false;
                }

                return true;
            });

            return ingestion;
        }

        protected async Task<CatalogIndexScan> UpdateAsync(CatalogIndexScan indexScan)
        {
            Assert.NotNull(indexScan);
            await ProcessQueueAsync(async () =>
            {
                indexScan = await CatalogScanStorageService.GetIndexScanAsync(indexScan.GetCursorName(), indexScan.GetScanId());

                if (indexScan.State != CatalogIndexScanState.Complete)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    return false;
                }

                return true;
            });

            ExpectedCatalogIndexScans.Add(indexScan);

            return indexScan;
        }

        protected async Task UpdateAsync(TaskStateKey taskStateKey)
        {
            await ProcessQueueAsync(async () =>
            {
                var countLowerBound = await TaskStateStorageService.GetCountLowerBoundAsync(taskStateKey.StorageSuffix, taskStateKey.PartitionKey);
                if (countLowerBound > 0)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    return false;
                }

                return true;
            });
        }

        protected async Task ProcessQueueAsync(Func<Task<bool>> isCompleteAsync, int workerCount = 1)
        {
            await ProcessQueueAsync(() => { }, isCompleteAsync, workerCount);
        }

        protected async Task ProcessQueueAsync(Action foundMessage, Func<Task<bool>> isCompleteAsync, int workerCount = 1)
        {
            var expandQueue = await WorkerQueueFactory.GetQueueAsync(QueueType.Expand);
            var workerQueue = await WorkerQueueFactory.GetQueueAsync(QueueType.Work);

            async Task<(QueueType queueType, QueueClient queue, QueueMessage message)> ReceiveMessageAsync()
            {
                QueueMessage message = await expandQueue.ReceiveMessageAsync();
                if (message != null)
                {
                    return (QueueType.Expand, expandQueue, message);
                }

                message = await workerQueue.ReceiveMessageAsync();
                if (message != null)
                {
                    return (QueueType.Work, workerQueue, message);
                }

                return (QueueType.Work, null, null);
            };

            bool isComplete;
            do
            {
                await Task.WhenAll(Enumerable
                    .Range(0, workerCount)
                    .Select(async x =>
                    {
                        while (true)
                        {
                            (var queueType, var queue, var message) = await ReceiveMessageAsync();
                            if (message != null)
                            {
                                foundMessage();
                                using (var scope = Host.Services.CreateScope())
                                {
                                    await ProcessMessageAsync(scope.ServiceProvider, queueType, message);
                                }

                                try
                                {
                                    await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                                }
                                catch (RequestFailedException ex) when (ex.Status == (int)HttpStatusCode.NotFound)
                                {
                                    // Ignore, some other thread processed the message and completed it first.
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }));

                isComplete = await isCompleteAsync();
            }
            while (!isComplete);
        }

        protected virtual async Task ProcessMessageAsync(IServiceProvider serviceProvider, QueueType queue, QueueMessage message)
        {
            var leaseScope = serviceProvider.GetRequiredService<TempStreamLeaseScope>();
            await using var scopeOwnership = leaseScope.TakeOwnership();
            var messageProcessor = serviceProvider.GetRequiredService<IGenericMessageProcessor>();
            await messageProcessor.ProcessSingleAsync(queue, message.Body.ToMemory(), message.DequeueCount);
        }

        protected async Task AssertCompactAsync<T>(string containerName, string testName, string stepName, int bucket, string fileName = null) where T : ICsvRecord
        {
            await AssertCsvBlobAsync<T>(containerName, testName, stepName, fileName, $"compact_{bucket}.csv.gz");
        }

        public async Task<IKustoIngestionResult> MakeTableReportIngestionResultAsync(StorageSourceOptions options, Status status)
        {
            var tableServiceClient = await ServiceClientFactory.GetTableServiceClientAsync();
            var writeTable = tableServiceClient.GetTableClient(StoragePrefix + "1kir1");
            await writeTable.CreateIfNotExistsAsync();
            await writeTable.AddEntityAsync(new IngestionStatus(options.SourceId)
            {
                Status = status,
                UpdatedOn = DateTime.UtcNow,
            });

            Uri tableSasUri;
            if (TestSettings.StorageSharedAccessSignature is not null)
            {
                tableSasUri = new UriBuilder(writeTable.Uri) { Query = TestSettings.StorageSharedAccessSignature }.Uri;
            }
            else
            {
                tableSasUri = writeTable.GenerateSasUri(TableSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(6));
            }

            return new TableReportIngestionResult(new AzureCloudTable(tableSasUri.AbsoluteUri));
        }

        protected static SortedDictionary<string, List<string>> NormalizeHeaders(ILookup<string, string> headers, IEnumerable<string> ignore)
        {
            // These headers are unstable
            var ignoredHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Accept-Ranges",
                "Access-Control-Allow-Origin",
                "Access-Control-Expose-Headers",
                "Age",
                "Cache-Control",
                "Connection",
                "Date",
                "Expires",
                "Server",
                "Strict-Transport-Security",
                "X-Azure-Ref",
                "X-Azure-Ref-OriginShield",
                "X-Cache",
                "X-CDN-Rewrite",
                "X-Content-Type-Options",
                "x-ms-lease-state",
                "x-ms-request-id",
                "x-ms-version",
                "x-ms-copy-completion-time",
                "x-ms-copy-id",
                "x-ms-copy-progress",
                "x-ms-copy-source",
                "x-ms-copy-status",
            };

            foreach (var header in ignore)
            {
                ignoredHeaders.Add(header);
            }

            return new SortedDictionary<string, List<string>>(headers
                .Where(x => !ignoredHeaders.Contains(x.Key))
                .Select(grouping =>
                {
                    if (grouping.Key == "ETag")
                    {
                        var values = new List<string>();
                        foreach (var value in grouping)
                        {
                            if (!value.StartsWith("\""))
                            {
                                values.Add("\"" + value + "\"");
                            }
                            else
                            {
                                values.Add(value);
                            }
                        }

                        return values.GroupBy(x => grouping.Key).Single();
                    }
                    else
                    {
                        return grouping;
                    }
                })
                .ToDictionary(x => x.Key, x => x.ToList()));
        }

        protected async Task AssertEntityOutputAsync<T>(
            TableClient table,
            string dir,
            Action<T> cleanEntity = null,
            string fileName = "entities.json") where T : class, ITableEntity, new()
        {
            var entities = await table.QueryAsync<T>().ToListAsync();

            // Workaround: https://github.com/Azure/azure-sdk-for-net/issues/21023
            var setTimestamp = typeof(T).GetProperty(nameof(ITableEntity.Timestamp));

            foreach (var entity in entities)
            {
                entity.ETag = default;
                setTimestamp.SetValue(entity, DateTimeOffset.MinValue);
                cleanEntity?.Invoke(entity);
            }

            var actual = SerializeTestJson(entities);
            var testDataFile = Path.Combine(TestData, dir, fileName);
            AssertEqualWithDiff(testDataFile, actual);
        }

        protected void AssertEqualWithDiff(string expectedPath, string actual)
        {
            if (OverwriteTestData)
            {
                OverwriteTestDataAndCopyToSource(expectedPath, actual);
            }

            var expected = File.ReadAllText(expectedPath);

            if (expected != actual && expected.Length > 0 && actual.Length > 0)
            {
                // Source: https://github.com/mmanela/diffplex/blob/2dda9db84569cf3c8413acdfc0ed440973632817/DiffPlex.ConsoleRunner/Program.cs
                var inlineBuilder = new InlineDiffBuilder(new Differ());
                var result = inlineBuilder.BuildDiffModel(expected, actual);

                Output.WriteLine("");
                Output.WriteLine("DIFF: ");
                Output.WriteLine(new string('-', 80));

                foreach (var line in result.Lines)
                {
                    switch (line.Type)
                    {
                        case ChangeType.Inserted:
                            Output.WriteLine("+ " + line.Text);
                            break;
                        case ChangeType.Deleted:
                            Output.WriteLine("+ " + line.Text);
                            break;
                        default:
                            Output.WriteLine("  " + line.Text);
                            break;
                    }
                }

                Output.WriteLine(new string('-', 80));
                Output.WriteLine("");
            }

            Assert.Equal(expected, actual);
        }

        protected async Task AssertSymbolPackageArchiveOutputAsync(string testName, string stepName)
        {
            await AssertWideEntityOutputAsync(
                Options.Value.SymbolPackageArchiveTableName,
                Path.Combine(testName, stepName),
                stream =>
                {
                    var entity = MessagePackSerializer.Deserialize<SymbolPackageFileService.SymbolPackageFileInfoVersions>(stream, NuGetInsightsMessagePack.Options);

                    string mzipHash = null;
                    SortedDictionary<string, List<string>> httpHeaders = null;

                    if (entity.V1.Available)
                    {
                        using var algorithm = SHA256.Create();
                        mzipHash = algorithm.ComputeHash(entity.V1.MZipBytes.ToArray()).ToLowerHex();
                        httpHeaders = NormalizeHeaders(entity.V1.HttpHeaders, ignore: Enumerable.Empty<string>());
                    }

                    return new
                    {
                        entity.V1.Available,
                        entity.V1.CommitTimestamp,
                        HttpHeaders = httpHeaders,
                        MZipHash = mzipHash,
                    };
                });
        }

        protected void MakeDeletedPackageAvailable(string id = "BehaviorSample", string version = "1.0.0")
        {
            var lowerId = id.ToLowerInvariant();
            var lowerVersion = NuGetVersion.Parse(version).ToNormalizedString().ToLowerInvariant();

            var nupkgFile = new FileInfo(Path.Combine(TestData, $"{lowerId}.{lowerVersion}.nupkg.testdata"));
            var nuspecFile = new FileInfo(Path.Combine(TestData, $"{lowerId}.{lowerVersion}.nuspec"));
            var readmeFile = new FileInfo(Path.Combine(TestData, $"{lowerId}.{lowerVersion}.md"));
            var snupkgFile = new FileInfo(Path.Combine(TestData, $"{lowerId}.{lowerVersion}.snupkg.testdata"));

            HttpMessageHandlerFactory.OnSendAsync = async (req, _, _) =>
            {
                if (req.RequestUri.AbsolutePath.EndsWith($"/{lowerId}.{lowerVersion}.nupkg"))
                {
                    var newReq = Clone(req);
                    newReq.RequestUri = new Uri($"http://localhost/{TestData}/{lowerId}.{lowerVersion}.nupkg.testdata");
                    var response = await TestDataHttpClient.SendAsync(newReq);
                    response.EnsureSuccessStatusCode();
                    return response;
                }

                if (req.RequestUri.AbsolutePath.EndsWith($"/{lowerId}.nuspec"))
                {
                    var newReq = Clone(req);
                    newReq.RequestUri = new Uri($"http://localhost/{TestData}/{lowerId}.{lowerVersion}.nuspec");
                    var response = await TestDataHttpClient.SendAsync(newReq);
                    response.EnsureSuccessStatusCode();
                    return response;
                }

                if (req.RequestUri.AbsolutePath.EndsWith($"{lowerId}/{lowerVersion}/readme"))
                {
                    var newReq = Clone(req);
                    newReq.RequestUri = new Uri($"http://localhost/{TestData}/{lowerId}.{lowerVersion}.md");
                    var response = await TestDataHttpClient.SendAsync(newReq);
                    response.EnsureSuccessStatusCode();
                    return response;
                }

                if (req.RequestUri.AbsolutePath.EndsWith($"/{lowerId}.{lowerVersion}.snupkg"))
                {
                    var newReq = Clone(req);
                    newReq.RequestUri = new Uri($"http://localhost/{TestData}/{lowerId}.{lowerVersion}.snupkg.testdata");
                    var response = await TestDataHttpClient.SendAsync(newReq);
                    response.EnsureSuccessStatusCode();
                    SetBlobResponseHeaders(response, snupkgFile.FullName);
                    return response;
                }

                return null;
            };

            if (nupkgFile.Exists)
            {
                nupkgFile.LastWriteTimeUtc = DateTime.Parse("2021-01-14T18:00:00Z");
            }

            if (nuspecFile.Exists)
            {
                nuspecFile.LastWriteTimeUtc = DateTime.Parse("2021-01-14T19:00:00Z");
            }

            if (readmeFile.Exists)
            {
                readmeFile.LastWriteTimeUtc = DateTime.Parse("2021-01-14T20:00:00Z");
            }

            if (snupkgFile.Exists)
            {
                snupkgFile.LastWriteTimeUtc = DateTime.Parse("2021-01-14T21:00:00Z");
            }
        }

        private void SetBlobResponseHeaders(HttpResponseMessage response, string sourcePath)
        {
            using (var fileStream = File.OpenRead(sourcePath))
            {
                using var hashes = IncrementalHash.CreateAll();
                var buffer = new byte[1024 * 64];
                int read;
                while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    hashes.TransformBlock(buffer, 0, read);
                }

                hashes.TransformFinalBlock();

                response.Content.Headers.TryAddWithoutValidation("Content-MD5", hashes.Output.MD5.ToBase64());
                response.Content.Headers.TryAddWithoutValidation("x-ms-meta-SHA512", hashes.Output.SHA512.ToBase64());
            }
        }

        protected async Task AssertWideEntityOutputAsync<T>(
            string tableName,
            string dir,
            Func<Stream, T> deserializeEntity,
            string fileName = "entities.json")
        {
            var service = Host.Services.GetRequiredService<WideEntityService>();

            var wideEntities = await service.RetrieveAsync(tableName);
            var entities = new List<(string PartitionKey, string RowKey, T Entity)>();
            foreach (var wideEntity in wideEntities)
            {
                var entity = deserializeEntity(wideEntity.GetStream());
                entities.Add((wideEntity.PartitionKey, wideEntity.RowKey, entity));
            }

            var actual = SerializeTestJson(entities.Select(x => new { x.PartitionKey, x.RowKey, x.Entity }));
            var testDataFile = Path.Combine(TestData, dir, fileName);
            AssertEqualWithDiff(testDataFile, actual);
        }

        protected async Task AssertOwnerToSubjectAsync<T>(
            string testName,
            string stepName,
            Func<byte[], T> deserializeEntity,
            string fileName = null)
        {
            var dir = Path.Combine(testName, stepName);

            await AssertWideEntityOutputAsync(
                Options.Value.OwnerToSubjectReferenceTableName,
                dir,
                stream =>
                {
                    var edges = MessagePackSerializer.Deserialize<OwnerToSubjectEdges>(stream, NuGetInsightsMessagePack.Options);

                    return new
                    {
                        Committed = edges.Committed.Select(x =>
                        {
                            return new
                            {
                                x.PartitionKey,
                                x.RowKey,
                                Data = deserializeEntity(x.Data),
                            };
                        }),
                        edges.ToAdd,
                        edges.ToDelete,
                    };
                },
                fileName: fileName ?? "owner-to-subject.json");
        }

        protected async Task AssertSubjectToOwnerAsync(string testName, string stepName, string fileName = null)
        {
            var dir = Path.Combine(testName, stepName);

            var table = (await ServiceClientFactory.GetTableServiceClientAsync())
                .GetTableClient(Options.Value.SubjectToOwnerReferenceTableName);
            await AssertEntityOutputAsync<TableEntity>(
                table,
                dir,
                fileName: fileName ?? "subject-to-owner.json");
        }

        private ICslAdminProvider GetKustoAdminClient()
        {
            var connectionStringBuilder = ServiceCollectionExtensions.GetKustoConnectionStringBuilder(new NuGetInsightsWorkerSettings
            {
                KustoConnectionString = TestSettings.KustoConnectionString,
                KustoClientCertificateContent = TestSettings.KustoClientCertificateContent,
            });

            return KustoClientFactory.CreateCslAdminProvider(connectionStringBuilder);
        }

        protected async Task CleanUpKustoTablesAsync(Predicate<string> shouldDelete = null)
        {
            var tables = await GetKustoTablesAsync(shouldDelete);

            using var adminClient = GetKustoAdminClient();
            foreach (var table in tables)
            {
                Logger.LogInformation("Deleting Kusto table: {Name}", table);
                using var reader = await adminClient.ExecuteControlCommandAsync(TestSettings.KustoDatabaseName, ".drop table " + table);
            }
        }

        protected async Task<List<string>> GetKustoTablesAsync(Predicate<string> shouldInclude = null)
        {
            if (shouldInclude is null)
            {
                shouldInclude = x => x.StartsWith(StoragePrefix);
            }

            using var adminClient = GetKustoAdminClient();

            var tables = new List<string>();
            using (var reader = await adminClient.ExecuteControlCommandAsync(TestSettings.KustoDatabaseName, ".show tables"))
            {
                while (reader.Read())
                {
                    var tableName = (string)reader["TableName"];
                    if (shouldInclude(tableName))
                    {
                        tables.Add(tableName);
                    }
                }
            }

            return tables;
        }

        private class TableReportIngestionResult : IKustoIngestionResult
        {
            public TableReportIngestionResult(ICloudTable table)
            {
                IngestionStatusTable = table;
            }

            public ICloudTable IngestionStatusTable { get; }

            public IngestionStatus GetIngestionStatusBySourceId(Guid sourceId)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<IngestionStatus> GetIngestionStatusCollection()
            {
                throw new NotImplementedException();
            }
        }

        private interface ICloudTable
        {
            string TableSasUri { get; }
        }

        public class AzureCloudTable : ICloudTable
        {
            public AzureCloudTable(string tableSasUri)
            {
                TableSasUri = tableSasUri;
            }

            public string TableSasUri { get; }
        }
    }
}
