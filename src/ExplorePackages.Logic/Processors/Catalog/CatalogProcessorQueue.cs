﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Knapcode.ExplorePackages.Logic
{
    public class CatalogProcessorQueue
    {
        private readonly CatalogClient _catalogClient;
        private readonly CursorService _cursorService;
        private readonly ICatalogEntriesProcessor _processor;
        private readonly ISingletonService _singletonService;
        private readonly ILogger<CatalogProcessorQueue> _logger;

        public CatalogProcessorQueue(
            CatalogClient catalogClient,
            CursorService cursorService,
            ICatalogEntriesProcessor processor,
            ISingletonService singletonService,
            ILogger<CatalogProcessorQueue> logger)
        {
            _catalogClient = catalogClient;
            _cursorService = cursorService;
            _processor = processor;
            _singletonService = singletonService;
            _logger = logger;
        }

        public async Task ProcessAsync(CancellationToken token)
        {
            var start = await _cursorService.GetValueAsync(_processor.CursorName);
            DateTimeOffset end;
            var dependencyCursorNames = _processor.DependencyCursorNames;
            if (dependencyCursorNames.Any())
            {
                end = await _cursorService.GetMinimumAsync(dependencyCursorNames);
            }
            else
            {
                end = DateTimeOffset.UtcNow;
            }

            var taskQueue = new TaskQueue<Work>(
                workerCount: 1,
                workAsync: WorkAsync,
                logger: _logger);

            taskQueue.Start();

            await taskQueue.ProduceThenCompleteAsync(
                t => ProduceAsync(taskQueue, start, end, t));
        }

        private async Task WorkAsync(Work work, CancellationToken token)
        {
            if (!work.Leaves.Any())
            {
                return;
            }

            await _processor.ProcessAsync(work.Page, work.Leaves);

            var newCursorValue = work.Leaves.Max(x => x.CommitTimestamp);
            _logger.LogInformation("Cursor {CursorName} moving to {Start:O}.", _processor.CursorName, newCursorValue);
            await _cursorService.SetValueAsync(_processor.CursorName, newCursorValue);
        }

        private async Task ProduceAsync(TaskQueue<Work> taskQueue, DateTimeOffset start, DateTimeOffset end, CancellationToken token)
        {
            var remainingPages = new Queue<CatalogPageItem>(await _catalogClient.GetCatalogPageItemsAsync(start, end));

            while (remainingPages.Any())
            {
                token.ThrowIfCancellationRequested();

                await _singletonService.RenewAsync();

                var currentPage = remainingPages.Dequeue();
                var currentPages = new[] { currentPage };

                var entries = await _catalogClient.GetCatalogLeafItemsAsync(currentPages, start, end, token);

                taskQueue.Enqueue(new Work(currentPage, entries));

                await taskQueue.WaitForCountToBeLessThanAsync(10, token);
            }
        }

        private class Work
        {
            public Work(CatalogPageItem page, IReadOnlyList<CatalogLeafItem> leaves)
            {
                Page = page;
                Leaves = leaves;
            }

            public CatalogPageItem Page { get; }
            public IReadOnlyList<CatalogLeafItem> Leaves { get; }
        }
    }
}
