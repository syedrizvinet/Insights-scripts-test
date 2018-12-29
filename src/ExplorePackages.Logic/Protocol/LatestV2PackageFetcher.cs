﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Knapcode.ExplorePackages.Logic
{
    public class LatestV2PackageFetcher
    {
        private readonly V2Client _client;
        private readonly IOptionsSnapshot<ExplorePackagesSettings> _options;

        public LatestV2PackageFetcher(
            V2Client client,
            IOptionsSnapshot<ExplorePackagesSettings> options)
        {
            _client = client;
            _options = options;
        }

        public async Task<V2Package> GetLatestPackageAsync(IProgressReporter progressReporter)
        {
            var packages = await _client.GetPackagesAsync(
                _options.Value.V2BaseUrl,
                filter: null,
                orderBy: "Created desc",
                top: 1);
            var package = packages.FirstOrDefault();

            if (package == null)
            {
                await progressReporter.ReportProgressAsync(1m, $"Found no latest packages.");
            }
            else
            {
                await progressReporter.ReportProgressAsync(1m, $"Found one latest package {package.Id} {package.Version}, created at {package.Created:O}.");
            }

            return package;
        }
    }
}
