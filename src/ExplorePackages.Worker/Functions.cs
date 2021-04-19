﻿using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Queue;
using static Knapcode.ExplorePackages.Worker.CustomNameResolver;
using static Knapcode.ExplorePackages.Worker.CustomStorageAccountProvider;

namespace Knapcode.ExplorePackages.Worker
{
    public class Functions
    {
        private readonly TempStreamLeaseScope _tempStreamLeaseScope;
        private readonly TimerExecutionService _timerExecutionService;
        private readonly IGenericMessageProcessor _messageProcessor;

        public Functions(
            TempStreamLeaseScope tempStreamLeaseScope,
            TimerExecutionService timerExecutionService,
            IGenericMessageProcessor messageProcessor)
        {
            _tempStreamLeaseScope = tempStreamLeaseScope;
            _timerExecutionService = timerExecutionService;
            _messageProcessor = messageProcessor;
        }

        [FunctionName("TimerFunction")]
        public async Task TimerAsync(
            [TimerTrigger("0 * * * * *")] TimerInfo timerInfo)
        {
            await using var scopeOwnership = _tempStreamLeaseScope.TakeOwnership();
            await _timerExecutionService.InitializeAsync();
            await _timerExecutionService.ExecuteAsync();
        }

        [FunctionName("WorkQueueFunction")]
        public async Task WorkQueueAsync(
            [QueueTrigger(WorkQueueVariable, Connection = ConnectionName)] CloudQueueMessage message)
        {
            await ProcessMessageAsync(message);
        }

        [FunctionName("ExpandQueueFunction")]
        public async Task ExpandQueueAsync(
            [QueueTrigger(ExpandQueueVariable, Connection = ConnectionName)] CloudQueueMessage message)
        {
            await ProcessMessageAsync(message);
        }

        private async Task ProcessMessageAsync(CloudQueueMessage message)
        {
            await using var scopeOwnership = _tempStreamLeaseScope.TakeOwnership();
            await _messageProcessor.ProcessSingleAsync(message.AsString, message.DequeueCount);
        }
    }
}
