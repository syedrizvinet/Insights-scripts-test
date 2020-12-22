﻿using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Knapcode.ExplorePackages.Worker
{
    public class CatalogLeafToCsvCompactProcessor<T> : IMessageProcessor<CatalogLeafToCsvCompactMessage<T>> where T : ICsvRecord, new()
    {
        private readonly AppendResultStorageService _storageService;
        private readonly TaskStateStorageService _taskStateStorageService;
        private readonly ICatalogLeafToCsvDriver<T> _driver;
        private readonly ICsvReader _csvReader;
        private readonly ILogger<CatalogLeafToCsvCompactProcessor<T>> _logger;

        public CatalogLeafToCsvCompactProcessor(
            AppendResultStorageService storageService,
            TaskStateStorageService taskStateStorageService,
            ICatalogLeafToCsvDriver<T> driver,
            ICsvReader csvReader,
            ILogger<CatalogLeafToCsvCompactProcessor<T>> logger)
        {
            _storageService = storageService;
            _taskStateStorageService = taskStateStorageService;
            _driver = driver;
            _csvReader = csvReader;
            _logger = logger;
        }

        public async Task ProcessAsync(CatalogLeafToCsvCompactMessage<T> message, int dequeueCount)
        {
            TaskState taskState;
            if (message.Force
                && message.TaskStatePartitionKey == null
                && message.TaskStateRowKey == null
                && message.TaskStateStorageSuffix == null)
            {
                taskState = null;
            }
            else
            {
                taskState = await _taskStateStorageService.GetAsync(
                    message.TaskStateStorageSuffix,
                    message.TaskStatePartitionKey,
                    message.TaskStateRowKey);
            }

            if (!message.Force && taskState == null)
            {
                _logger.LogWarning("No matching task state was found.");
                return;
            }

            await _storageService.CompactAsync<T>(
                message.SourceContainer,
                _driver.ResultsContainerName,
                message.Bucket,
                force: message.Force,
                mergeExisting: true,
                _driver.Prune,
                _csvReader);

            if (taskState != null)
            {
                await _taskStateStorageService.DeleteAsync(taskState);
            }
        }
    }
}
