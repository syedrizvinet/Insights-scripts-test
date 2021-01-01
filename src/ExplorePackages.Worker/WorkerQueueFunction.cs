﻿using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Knapcode.ExplorePackages.Worker
{
    public class WorkerQueueFunction
    {
        private const string WorkerQueue = ExplorePackagesSettings.DefaultSectionName + ":" + nameof(ExplorePackagesWorkerSettings.WorkerQueueName);
        private const string WorkerQueueVariable = "%" + WorkerQueue + "%";
        private const string StorageConnection = ExplorePackagesSettings.DefaultSectionName + ":" + nameof(ExplorePackagesSettings.StorageConnectionString);

        private readonly TempStreamLeaseScope _tempStreamLeaseScope;
        private readonly GenericMessageProcessor _messageProcessor;
        private readonly ILogger<WorkerQueueFunction> _logger;

        public WorkerQueueFunction(
            TempStreamLeaseScope tempStreamLeaseScope,
            GenericMessageProcessor messageProcessor,
            ILogger<WorkerQueueFunction> logger)
        {
            _tempStreamLeaseScope = tempStreamLeaseScope;
            _messageProcessor = messageProcessor;
            _logger = logger;
        }

        [FunctionName("WorkerQueueFunction")]
        public async Task ProcessAsync(
            [QueueTrigger(WorkerQueueVariable, Connection = StorageConnection)] CloudQueueMessage message)
        {
            await using var scopeOwnership = _tempStreamLeaseScope.TakeOwnership();
            await _messageProcessor.ProcessAsync(message.AsString, message.DequeueCount);
        }
    }
}
