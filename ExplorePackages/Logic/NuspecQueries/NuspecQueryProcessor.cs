﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Knapcode.ExplorePackages.Entities;
using NuGet.Common;

namespace Knapcode.ExplorePackages.Logic
{
    public class NuspecQueryProcessor
    {
        private readonly PackagePathProvider _pathProvider;
        private readonly PackageQueryService _queryService;
        private readonly IReadOnlyList<INuspecQuery> _queries;
        private readonly ILogger _log;

        public NuspecQueryProcessor(
            PackagePathProvider pathProvider,
            IReadOnlyList<INuspecQuery> queries,
            ILogger log)
        {
            _pathProvider = pathProvider;
            _queryService = new PackageQueryService(log);
            _queries = queries;
            _log = log;
        }

        public async Task ProcessAsync(CancellationToken token)
        {
            var cursorService = new CursorService();

            var cursorStarts = new Dictionary<string, DateTimeOffset>();
            var start = await GetMinimumQueryStartAsync(cursorService, cursorStarts);
            var end = await cursorService.GetMinimumAsync(new[]
{
                CursorNames.CatalogToDatabase,
                CursorNames.CatalogToNuspecs,
            });

            var complete = 0;
            var stopwatch = Stopwatch.StartNew();

            int commitCount;
            var packageService = new PackageService(_log);

            do
            {
                var commits = await packageService.GetPackageCommitsAsync(start, end);
                commitCount = commits.Count;

                var allQueryMatches = _queries.ToDictionary(
                    x => x.Name,
                    x => new List<PackageIdentity>());

                foreach (var commit in commits)
                {
                    foreach (var package in commit.Packages)
                    {
                        var nuspec = GetNuspecAndMetadata(package);

                        foreach (var query in _queries)
                        {
                            if (commit.CommitTimestamp <= cursorStarts[query.CursorName])
                            {
                                continue;
                            }

                            if (package.Deleted)
                            {
                                continue;
                            }

                            var isMatch = false;
                            try
                            {
                                isMatch = await query.IsMatchAsync(nuspec);
                            }
                            catch (Exception e)
                            {
                                _log.LogError($"Could not query .nuspec for {nuspec.Id} {nuspec.Version}: {nuspec.Path}"
                                    + Environment.NewLine
                                    + "  "
                                    + e.Message);
                                throw;
                            }

                            if (isMatch)
                            {
                                _log.LogInformation($"Query match {query.Name}: {package.Id} {package.Version}");
                                allQueryMatches[query.Name].Add(new PackageIdentity(package.Id, package.Version));
                            }
                        }

                        start = commit.CommitTimestamp;
                    }
                }

                complete += commits.Sum(x => x.Packages.Count);
                _log.LogInformation($"{complete} completed ({Math.Round(complete / stopwatch.Elapsed.TotalSeconds)} per second).");

                await PersistResultsAndCursorsAsync(cursorService, cursorStarts, start, allQueryMatches);
            }
            while (commitCount > 0);
        }

        private async Task<DateTimeOffset> GetMinimumQueryStartAsync(
            CursorService cursorService,
            Dictionary<string, DateTimeOffset> cursorStarts)
        {
            var start = DateTimeOffset.MaxValue;
            foreach (var query in _queries)
            {
                if (!cursorStarts.ContainsKey(query.CursorName))
                {
                    var cursorStart = await cursorService.GetAsync(query.CursorName);
                    cursorStarts[query.CursorName] = cursorStart;

                    if (cursorStart < start)
                    {
                        start = cursorStart;
                    }
                }
            }

            return start;
        }

        private async Task PersistResultsAndCursorsAsync(
            CursorService cursorService,
            Dictionary<string, DateTimeOffset> cursorStarts,
            DateTimeOffset start,
            Dictionary<string, List<PackageIdentity>> allQueryMatches)
        {
            foreach (var query in _queries)
            {
                if (allQueryMatches[query.Name].Any())
                {
                    await _queryService.AddQueryAsync(query.Name, query.CursorName);
                    await _queryService.AddMatchesAsync(query.Name, allQueryMatches[query.Name]);
                }

                if (cursorStarts[query.CursorName] < start)
                {
                    await cursorService.SetAsync(query.CursorName, start);
                    cursorStarts[query.CursorName] = start;
                }
            }
        }

        private NuspecAndMetadata GetNuspecAndMetadata(Package package)
        {
            var path = _pathProvider.GetLatestNuspecPath(package.Id, package.Version);
            var exists = false;
            XDocument document = null;
            try
            {
                if (File.Exists(path))
                {
                    exists = true;
                    using (var stream = File.OpenRead(path))
                    {
                        document = NuspecUtility.LoadXml(stream);
                    }
                }
                else
                {
                    _log.LogWarning($"Could not find .nuspec for {package.Id} {package.Version}: {path}");
                }
            }
            catch (Exception e)
            {
                _log.LogError($"Could not parse .nuspec for {package.Id} {package.Version}: {path}"
                    + Environment.NewLine
                    + "  "
                    + e.Message);
            }

            return new NuspecAndMetadata(
                package.Id,
                package.Version,
                path,
                exists,
                document);
        }
    }
}
