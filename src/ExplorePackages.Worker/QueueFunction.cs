﻿using System.Threading.Tasks;
using Knapcode.ExplorePackages.Logic;
using Knapcode.ExplorePackages.Logic.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Knapcode.ExplorePackages.Worker
{
    public class QueueFunction
    {
        private const string Connection = ExplorePackagesSettings.DefaultSectionName + ":" + nameof(ExplorePackagesSettings.StorageConnectionString);

        private readonly GenericMessageProcessor _messageProcessor;
        private readonly RawMessageEnqueuer _enqueuer;
        private readonly UnencodedQueueStorageEnqueuer _innerEnqueuer;

        public QueueFunction(RawMessageEnqueuer enqueuer, UnencodedQueueStorageEnqueuer innerEnqueuer, GenericMessageProcessor messageProcessor)
        {
            _enqueuer = enqueuer;
            _innerEnqueuer = innerEnqueuer;
            _messageProcessor = messageProcessor;
        }

        [FunctionName("QueueFunction")]
        public async Task ProcessAsync(
            [QueueTrigger("queue", Connection = Connection)] string message,
            [Queue("queue", Connection = Connection)] CloudQueue target)
        {
            _innerEnqueuer.SetTarget(target);
            _enqueuer.SetTarget(_innerEnqueuer);
            await _messageProcessor.ProcessAsync(message);
        }
    }
}
