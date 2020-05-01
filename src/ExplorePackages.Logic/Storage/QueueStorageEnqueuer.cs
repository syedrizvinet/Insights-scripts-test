﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Knapcode.ExplorePackages.Logic.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Knapcode.ExplorePackages.Logic
{
    public class QueueStorageEnqueuer : IRawMessageEnqueuer
    {
        private readonly IWorkerQueueFactory _workerQueueFactory;
        private readonly IOptionsSnapshot<ExplorePackagesSettings> _options;
        private readonly ILogger<QueueStorageEnqueuer> _logger;

        public QueueStorageEnqueuer(
            IWorkerQueueFactory workerQueueFactory,
            IOptionsSnapshot<ExplorePackagesSettings> options,
            ILogger<QueueStorageEnqueuer> logger)
        {
            _workerQueueFactory = workerQueueFactory;
            _options = options;
            _logger = logger;
            BulkEnqueueStrategy = _options.Value.UseBulkEnqueueStrategy
                ? BulkEnqueueStrategy.Enabled(_options.Value.BulkEnqueueThreshold, maxSize: 65536)
                : BulkEnqueueStrategy.Disabled();
        }

        public BulkEnqueueStrategy BulkEnqueueStrategy { get; }

        public async Task AddAsync(IReadOnlyList<string> messages)
        {
            await AddAsync(messages, TimeSpan.Zero);
        }

        public async Task AddAsync(IReadOnlyList<string> messages, TimeSpan visibilityDelay)
        {
            if (messages.Count == 0)
            {
                return;
            }

            var workers = Math.Min(messages.Count, _options.Value.EnqueueWorkers);
            if (workers < 2)
            {
                _logger.LogInformation("Enqueueing {Count} individual messages.", messages.Count);
                var queue = _workerQueueFactory.GetQueue();
                foreach (var message in messages)
                {
                    await AddMessageAsync(queue, message, visibilityDelay);
                }
            }
            else
            {
                var work = new ConcurrentQueue<string>(messages);

                _logger.LogInformation(
                    "Enqueueing {MessageCount} individual messages with {WorkerCount} workers.",
                    messages.Count,
                    workers);

                var tasks = Enumerable
                    .Range(0, workers)
                    .Select(async i =>
                    {
                        var queue = _workerQueueFactory.GetQueue();
                        while (work.TryDequeue(out var message))
                        {
                            await AddMessageAsync(queue, message, visibilityDelay);
                        }
                    })
                    .ToList();

                await Task.WhenAll(tasks);
            }
        }

        private async Task AddMessageAsync(CloudQueue queue, string message, TimeSpan initialVisibilityDelay)
        {
            await queue.AddMessageAsync(
                new CloudQueueMessage(message),
                timeToLive: null,
                initialVisibilityDelay: initialVisibilityDelay > TimeSpan.Zero ? (TimeSpan?)initialVisibilityDelay : null,
                options: null,
                operationContext: null);
        }
    }
}