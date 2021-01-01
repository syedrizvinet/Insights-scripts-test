﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Knapcode.ExplorePackages.Worker
{
    public class TableScanMessageProcessor<T> : IMessageProcessor<TableScanMessage<T>> where T : ITableEntity, new()
    {
        private readonly TaskStateStorageService _taskStateStorageService;
        private readonly ServiceClientFactory _serviceClientFactory;
        private readonly MessageEnqueuer _enqueuer;
        private readonly SchemaSerializer _serializer;
        private readonly TablePrefixScanner _prefixScanner;
        private readonly TableScanDriverFactory<T> _driverFactory;
        private readonly ITelemetryClient _telemetryClient;

        public TableScanMessageProcessor(
            TaskStateStorageService taskStateStorageService,
            ServiceClientFactory serviceClientFactory,
            MessageEnqueuer enqueuer,
            SchemaSerializer serializer,
            TablePrefixScanner prefixScanner,
            TableScanDriverFactory<T> driverFactory,
            ITelemetryClient telemetryClient)
        {
            _taskStateStorageService = taskStateStorageService;
            _serviceClientFactory = serviceClientFactory;
            _enqueuer = enqueuer;
            _serializer = serializer;
            _prefixScanner = prefixScanner;
            _driverFactory = driverFactory;
            _telemetryClient = telemetryClient;
        }

        public async Task ProcessAsync(TableScanMessage<T> message, int dequeueCount)
        {
            var taskState = await _taskStateStorageService.GetAsync(message.TaskStateKey);
            if (taskState == null)
            {
                return;
            }

            switch (message.Strategy)
            {
                case TableScanStrategy.Serial:
                    await ProcessSerialAsync(message);
                    break;
                case TableScanStrategy.PrefixScan:
                    await ProcessPrefixScanAsync(message);
                    break;
                default:
                    throw new NotImplementedException();
            }

            await _taskStateStorageService.DeleteAsync(taskState);
        }

        private async Task ProcessSerialAsync(TableScanMessage<T> message)
        {
            if (message.PartitionKeyPrefix != string.Empty)
            {
                throw new NotImplementedException();
            }

            using var metrics = _telemetryClient.NewQueryLoopMetrics();

            var sourceTable = GetTable(message.TableName);
            var driver = _driverFactory.Create(message.DriverType);
            await driver.InitializeAsync(message.DriverParameters);

            var tableQuery = new TableQuery<T>
            {
                SelectColumns = driver.SelectColumns,
                TakeCount = message.TakeCount,
            };

            TableContinuationToken continuationToken = null;
            do
            {
                TableQuerySegment<T> segment;
                using (metrics.TrackQuery())
                {
                    segment = await sourceTable.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);
                }

                await driver.ProcessEntitySegmentAsync(message.TableName, message.DriverParameters, segment.Results);

                continuationToken = segment.ContinuationToken;
            }
            while (continuationToken != null);
        }

        private async Task ProcessPrefixScanAsync(TableScanMessage<T> message)
        {
            var driver = _driverFactory.Create(message.DriverType);

            var tableQueryParameters = new TableQueryParameters(
                GetTable(message.TableName),
                driver.SelectColumns,
                message.TakeCount);

            TablePrefixScanStep currentStep;
            if (message.ScanParameters == null)
            {
                currentStep = new TablePrefixScanStart(
                    tableQueryParameters,
                    message.PartitionKeyPrefix);
            }
            else
            {
                switch (_serializer.Deserialize(message.ScanParameters).Data)
                {
                    case TablePrefixScanPartitionKeyQueryParameters partitionKeyQueryParameters:
                        currentStep = new TablePrefixScanPartitionKeyQuery(
                            tableQueryParameters,
                            partitionKeyQueryParameters.Depth,
                            partitionKeyQueryParameters.PartitionKey,
                            partitionKeyQueryParameters.RowKeySkip);
                        break;

                    case TablePrefixScanPrefixQueryParameters prefixQueryParameters:
                        currentStep = new TablePrefixScanPrefixQuery(
                            tableQueryParameters,
                            prefixQueryParameters.Depth,
                            prefixQueryParameters.PartitionKeyPrefix,
                            prefixQueryParameters.PartitionKeyLowerBound);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            // Run as many non-async steps as possible to save needless enqueues but only perform on batch of
            // asynchronous steps to reduce runtime.
            var currentSteps = new List<TablePrefixScanStep> { currentStep };
            var enqueueSteps = new List<TablePrefixScanStep>();
            while (currentSteps.Any())
            {
                var step = currentSteps.Last();
                currentSteps.RemoveAt(currentSteps.Count - 1);
                switch (step)
                {
                    case TablePrefixScanStart start:
                        await driver.InitializeAsync(message.DriverParameters);
                        currentSteps.AddRange(_prefixScanner.Start(start));
                        break;
                    case TablePrefixScanEntitySegment<T> entitySegment:
                        enqueueSteps.Add(entitySegment);
                        break;
                    case TablePrefixScanPartitionKeyQuery partitionKeyQuery:
                        enqueueSteps.AddRange(await _prefixScanner.ExecutePartitionKeyQueryAsync<T>(partitionKeyQuery));
                        break;
                    case TablePrefixScanPrefixQuery prefixQuery:
                        enqueueSteps.AddRange(await _prefixScanner.ExecutePrefixQueryAsync<T>(prefixQuery));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            await EnqueuePrefixScanStepsAsync(message, driver, enqueueSteps);
        }

        private async Task EnqueuePrefixScanStepsAsync(TableScanMessage<T> originalMessage, ITableScanDriver<T> driver, List<TablePrefixScanStep> nextSteps)
        {
            // Two types of messages can be enqueued here:
            //   1. Table row copy messages (the actual work to be done)
            //   2. Table copy messages (recursion)

            var entities = new List<T>();
            var tableCopyMessages = new List<TableScanMessage<T>>();
            var taskStates = new List<TaskState>();

            foreach (var nextStep in nextSteps)
            {
                switch (nextStep)
                {
                    case TablePrefixScanEntitySegment<T> segment:
                        entities.AddRange(segment.Entities);
                        break;
                    case TablePrefixScanPartitionKeyQuery partitionKeyQuery:
                        tableCopyMessages.Add(GetPrefixScanMessage(
                            originalMessage,
                            new TablePrefixScanPartitionKeyQueryParameters
                            {
                                Depth = partitionKeyQuery.Depth,
                                PartitionKey = partitionKeyQuery.PartitionKey,
                                RowKeySkip = partitionKeyQuery.RowKeySkip,
                            },
                            taskStates));
                        break;
                    case TablePrefixScanPrefixQuery prefixQuery:
                        tableCopyMessages.Add(GetPrefixScanMessage(
                            originalMessage,
                            new TablePrefixScanPrefixQueryParameters
                            {
                                Depth = prefixQuery.Depth,
                                PartitionKeyPrefix = prefixQuery.PartitionKeyPrefix,
                                PartitionKeyLowerBound = prefixQuery.PartitionKeyLowerBound,
                            },
                            taskStates));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            if (entities.Any())
            {
                await driver.ProcessEntitySegmentAsync(originalMessage.TableName, originalMessage.DriverParameters, entities);
            }

            if (tableCopyMessages.Any())
            {
                await _taskStateStorageService.AddAllAsync(
                    originalMessage.TaskStateKey.StorageSuffix,
                    originalMessage.TaskStateKey.PartitionKey,
                    taskStates);

                await _enqueuer.EnqueueAsync(tableCopyMessages);
            }
        }

        private TableScanMessage<T> GetPrefixScanMessage<TParameters>(TableScanMessage<T> originalMessage, TParameters scanParameters, List<TaskState> addedTaskStates)
        {
            var serializedParameters = _serializer.Serialize(scanParameters);

            string rowKey;
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(serializedParameters.AsString());
                rowKey = sha256.ComputeHash(bytes).ToTrimmedBase32();
            }

            var taskState = new TaskState(
                originalMessage.TaskStateKey.StorageSuffix,
                originalMessage.TaskStateKey.PartitionKey,
                rowKey)
            {
                Parameters = serializedParameters.AsString(),
            };

            addedTaskStates.Add(taskState);

            return new TableScanMessage<T>
            {
                TaskStateKey = taskState.Key,
                DriverType = originalMessage.DriverType,
                TableName = originalMessage.TableName,
                Strategy = TableScanStrategy.PrefixScan,
                TakeCount = originalMessage.TakeCount,
                PartitionKeyPrefix = originalMessage.PartitionKeyPrefix,
                ScanParameters = serializedParameters.AsJToken(),
                DriverParameters = originalMessage.DriverParameters,
            };
        }

        private CloudTable GetTable(string name)
        {
            return _serviceClientFactory
                .GetStorageAccount()
                .CreateCloudTableClient()
                .GetTableReference(name);
        }
    }
}
