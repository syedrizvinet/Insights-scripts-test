﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Knapcode.ExplorePackages.Logic;
using NuGet.CatalogReader;
using NuGet.Common;

namespace Knapcode.ExplorePackages.Commands
{
    public class CatalogToNuspecsCommand : ICommand
    {
        private readonly CatalogReader _catalogReader;
        private readonly CatalogToNuspecsProcessor _processor;
        private readonly ILogger _log;

        public CatalogToNuspecsCommand(
            CatalogReader catalogReader,
            CatalogToNuspecsProcessor processor,
            ILogger log)
        {
            _catalogReader = catalogReader;
            _processor = processor;
            _log = log;
        }

        public async Task ExecuteAsync(IReadOnlyList<string> args, CancellationToken token)
        {
            var catalogProcessor = new CatalogProcessorQueue(_catalogReader, _processor, _log);

            await catalogProcessor.ProcessAsync(token);
        }

        public bool IsDatabaseRequired(IReadOnlyList<string> args)
        {
            return true;
        }
    }
}
