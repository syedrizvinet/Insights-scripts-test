﻿using System.Threading;
using System.Threading.Tasks;
using Knapcode.ExplorePackages.Logic;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using NuGet.CatalogReader;

namespace Knapcode.ExplorePackages.Commands
{
    public class CatalogToNuspecsCommand : ICommand
    {
        private readonly CatalogReader _catalogReader;
        private readonly CatalogToNuspecsProcessor _processor;

        public CatalogToNuspecsCommand(
            CatalogReader catalogReader,
            CatalogToNuspecsProcessor processor)
        {
            _catalogReader = catalogReader;
            _processor = processor;
        }

        public void Configure(CommandLineApplication app)
        {
        }

        public async Task ExecuteAsync(CancellationToken token)
        {
            var catalogProcessor = new CatalogProcessorQueue(_catalogReader, _processor);
            await catalogProcessor.ProcessAsync(token);
        }

        public bool IsDatabaseRequired()
        {
            return true;
        }
    }
}
