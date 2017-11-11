﻿using System.Threading.Tasks;

namespace Knapcode.ExplorePackages.Logic
{
    public class GalleryConsistencyService : IConsistencyService<GalleryConsistencyReport>
    {
        private readonly GalleryClient _client;
        private readonly ExplorePackagesSettings _settings;

        public GalleryConsistencyService(
            GalleryClient client,
            ExplorePackagesSettings settings)
        {
            _client = client;
            _settings = settings;
        }

        public async Task<GalleryConsistencyReport> GetReportAsync(PackageQueryContext context, PackageConsistencyState state)
        {
            var shouldExist = !context.Package.Deleted;

            var packageDeletedStatus = await _client.GetPackageDeletedStatusAsync(
                _settings.GalleryBaseUrl,
                context.Package.Id,
                context.Package.Version);

            var isConsistent = shouldExist == (packageDeletedStatus == PackageDeletedStatus.NotDeleted);

            return new GalleryConsistencyReport(
                isConsistent,
                packageDeletedStatus);
        }

        public async Task<bool> IsConsistentAsync(PackageQueryContext context, PackageConsistencyState state)
        {
            var report = await GetReportAsync(context, state);
            return report.IsConsistent;
        }

        public Task PopulateStateAsync(PackageQueryContext context, PackageConsistencyState state)
        {
            return Task.CompletedTask;
        }
    }
}
