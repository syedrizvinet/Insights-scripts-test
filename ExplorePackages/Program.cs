﻿using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Knapcode.ExplorePackages.Commands;
using Knapcode.ExplorePackages.Entities;
using Knapcode.ExplorePackages.Logic;
using Knapcode.ExplorePackages.Support;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Knapcode.ExplorePackages
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = 32;

            args = new[] { "fetchcursors" };
            MainAsync(args, CancellationToken.None).Wait();
            args = new[] { "catalogtodatabase" };
            MainAsync(args, CancellationToken.None).Wait();
            args = new[] { "catalogtonuspecs" };
            MainAsync(args, CancellationToken.None).Wait();
            args = new[] { "nuspecqueries" };
            MainAsync(args, CancellationToken.None).Wait();

            /*
            args = new[] { "showqueryresults", "FindRepositoriesNuspecQuery" };
            MainAsync(args, CancellationToken.None).Wait();
            */
        }

        private static async Task MainAsync(string[] args, CancellationToken token)
        {
            using (var entityContext = new EntityContext())
            {
                await entityContext.Database.EnsureCreatedAsync();
            }

            var log = new ConsoleLogger();
            
            if (args.Length == 0)
            {
                log.LogError("You must provide a parameter.");
                return;
            }

            var httpSource = new HttpSource(
                new PackageSource("https://api.nuget.org/v3/index.json"),
                () =>
                {
                    var httpClientHandler = new HttpClientHandler();
                    var httpMessageHandler = new PassThroughHandler { InnerHandler = httpClientHandler };
                    return Task.FromResult<HttpHandlerResource>(new HttpHandlerResourceV3(
                        httpClientHandler,
                        httpMessageHandler));
                },
                NullThrottle.Instance);

            var packagePathProvider = new PackagePathProvider(@"E:\data\nuget.org\packages");

            var nuspecDownloader = new NuspecDownloader(
                packagePathProvider,
                httpSource,
                log);

            ICommand command;
            switch (args[0])
            {
                case "nuspecqueries":
                    {
                        command = new NuspecQueriesCommand(
                            packagePathProvider,
                            new FindMissingDependencyIdsNuspecQuery(log),
                            new FindRepositoriesNuspecQuery(log),
                            new FindPackageTypesNuspecQuery(log),
                            new FindInvalidDependencyVersionsNuspecQuery(log),
                            new FindMissingDependencyVersionsNuspecQuery(log),
                            new FindEmptyDependencyVersionsNuspecQuery(log),
                            log);
                    }
                    break;
                case "fetchcursors":
                    {
                        command = new FetchCursorsCommand(
                            new RemoteCursorReader(
                                httpSource,
                                log),
                            log);
                    }
                    break;
                case "catalogtodatabase":
                    {
                        command = new CatalogToDatabaseCommand(
                            new CatalogToDatabaseProcessor(
                                log),
                            log);
                    }
                    break;
                case "catalogtonuspecs":
                    {
                        command = new CatalogToNuspecsCommand(
                            new CatalogToNuspecsProcessor(
                                nuspecDownloader,
                                log),
                            log);
                    }
                    break;
                case "showqueryresults":
                    {
                        command = new ShowQueryResultsCommand(
                            log);
                    }
                    break;
                default:
                    log.LogError("Unknown command.");
                    return;
            }
            
            await command.ExecuteAsync(args, token);
        }
    }
}