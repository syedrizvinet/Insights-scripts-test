﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Knapcode.ExplorePackages.Logic;
using Knapcode.ExplorePackages.Support;
using Knapcode.MiniZip;
using Microsoft.Extensions.DependencyInjection;
using NuGet.CatalogReader;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Knapcode.ExplorePackages
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddExplorePackages(this IServiceCollection serviceCollection, ExplorePackagesSettings settings)
        {
            serviceCollection.AddSingleton<UrlReportProvider>();
            serviceCollection.AddTransient<UrlReportHandler>();
            serviceCollection.AddTransient<LoggingHandler>();
            serviceCollection.AddSingleton(
                x => new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                });
            serviceCollection.AddTransient(
                x => new InitializeServicePointHandler(
                    connectionLeaseTimeout: TimeSpan.FromMinutes(1)));
            serviceCollection.AddTransient<HttpMessageHandler>(
                x =>
                {
                    var httpClientHandler = x.GetRequiredService<HttpClientHandler>();
                    var initializeServicePointerHander = x.GetRequiredService<InitializeServicePointHandler>();
                    var urlReportHandler = x.GetRequiredService<UrlReportHandler>();

                    initializeServicePointerHander.InnerHandler = httpClientHandler;
                    urlReportHandler.InnerHandler = initializeServicePointerHander;

                    return urlReportHandler;
                });
            serviceCollection.AddSingleton(
                x =>
                {
                    var httpMessageHandler = x.GetRequiredService<HttpMessageHandler>();
                    var loggingHandler = x.GetRequiredService<LoggingHandler>();
                    loggingHandler.InnerHandler = httpMessageHandler;
                    var httpClient = new HttpClient(loggingHandler);
                    UserAgent.SetUserAgent(httpClient);
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-ms-version", "2017-04-17");
                    return httpClient;
                });
            serviceCollection.AddSingleton(
                x => new HttpSource(
                    new PackageSource(settings.V3ServiceIndex),
                    () =>
                    {
                        var httpClientHandler = x.GetRequiredService<HttpClientHandler>();
                        var httpMessageHandler = x.GetRequiredService<HttpMessageHandler>();
                        return Task.FromResult<HttpHandlerResource>(new HttpHandlerResourceV3(
                            httpClientHandler,
                            httpMessageHandler));
                    },
                    NullThrottle.Instance));
            serviceCollection.AddTransient(
                x => new PackagePathProvider(settings.PackagePath));

            var searchServiceUrlCache = new SearchServiceUrlCache();
            serviceCollection.AddSingleton(searchServiceUrlCache);
            serviceCollection.AddSingleton<ISearchServiceUrlCacheInvalidator>(searchServiceUrlCache);
            serviceCollection.AddSingleton(
                x => new CatalogReader(
                    new Uri(x.GetRequiredService<ExplorePackagesSettings>().V3ServiceIndex, UriKind.Absolute),
                    x.GetRequiredService<HttpSource>(),
                    cacheContext: null,
                    cacheTimeout: TimeSpan.Zero,
                    log: x.GetRequiredService<ILogger>()));

            serviceCollection.AddTransient(
                x => new HttpZipProvider(
                    x.GetRequiredService<HttpClient>())
                {
                    FirstBufferSize = 4096,
                    SecondBufferSize = 4096,
                    BufferGrowthExponent = 2,
                });
            serviceCollection.AddTransient<MZipFormat>();

            serviceCollection.AddTransient(x => settings.Clone());
            serviceCollection.AddTransient<PackageQueryProcessor>();
            serviceCollection.AddTransient<CatalogToDatabaseProcessor>();
            serviceCollection.AddTransient<CatalogToNuspecsProcessor>();
            serviceCollection.AddTransient<NuspecDownloader>();
            serviceCollection.AddTransient<MZipStore>();
            serviceCollection.AddTransient<RemoteCursorService>();
            serviceCollection.AddTransient<IPortTester, PortTester>();
            serviceCollection.AddTransient<IPortDiscoverer, SimplePortDiscoverer>();
            serviceCollection.AddTransient<SearchServiceUrlDiscoverer>();
            serviceCollection.AddTransient<SearchServiceCursorReader>();
            serviceCollection.AddTransient<PackageQueryContextBuilder>();
            serviceCollection.AddTransient<IProgressReport, NullProgressReport>();
            serviceCollection.AddTransient<LatestV2PackageFetcher>();
            serviceCollection.AddTransient<LatestCatalogCommitFetcher>();
            serviceCollection.AddTransient<V2ToDatabaseProcessor>();
            serviceCollection.AddTransient<PackageDownloadsToDatabaseProcessor>();
            serviceCollection.AddTransient<MZipCollector>();

            serviceCollection.AddTransient<PackageCommitEnumerator>();
            serviceCollection.AddTransient<CursorService>();
            serviceCollection.AddTransient<IETagService, ETagService>();
            serviceCollection.AddTransient<PackageService>();
            serviceCollection.AddTransient<IPackageService, PackageService>();
            serviceCollection.AddTransient<PackageQueryService>();
            serviceCollection.AddTransient<CatalogService>();
            serviceCollection.AddTransient<PackageDependencyService>();
            serviceCollection.AddTransient<NuspecProvider>();

            serviceCollection.AddTransient<V2Parser>();
            serviceCollection.AddSingleton<ServiceIndexCache>();
            serviceCollection.AddTransient<GalleryClient>();
            serviceCollection.AddTransient<V2Client>();
            serviceCollection.AddTransient<PackagesContainerClient>();
            serviceCollection.AddTransient<FlatContainerClient>();
            serviceCollection.AddTransient<RegistrationClient>();
            serviceCollection.AddTransient<SearchClient>();
            serviceCollection.AddTransient<AutocompleteClient>();
            serviceCollection.AddTransient<IPackageDownloadsClient, PackageDownloadsClient>();

            serviceCollection.AddTransient<GalleryConsistencyService>();
            serviceCollection.AddTransient<V2ConsistencyService>();
            serviceCollection.AddTransient<FlatContainerConsistencyService>();
            serviceCollection.AddTransient<PackagesContainerConsistencyService>();
            serviceCollection.AddTransient<RegistrationOriginalConsistencyService>();
            serviceCollection.AddTransient<RegistrationGzippedConsistencyService>();
            serviceCollection.AddTransient<RegistrationSemVer2ConsistencyService>();
            serviceCollection.AddTransient<SearchLoadBalancerConsistencyService>();
            serviceCollection.AddTransient<SearchSpecificInstancesConsistencyService>();
            serviceCollection.AddTransient<PackageConsistencyService>();
            serviceCollection.AddTransient<CrossCheckConsistencyService>();

            serviceCollection.AddTransient<FindIdsEndingInDotNumberNuspecQuery>();
            serviceCollection.AddTransient<FindRepositoriesNuspecQuery>();
            serviceCollection.AddTransient<FindInvalidDependencyVersionsNuspecQuery>();
            serviceCollection.AddTransient<FindMissingDependencyIdsNuspecQuery>();
            serviceCollection.AddTransient<FindPackageTypesNuspecQuery>();
            serviceCollection.AddTransient<FindSemVer2PackageVersionsNuspecQuery>();
            serviceCollection.AddTransient<FindSemVer2DependencyVersionsNuspecQuery>();
            serviceCollection.AddTransient<FindFloatingDependencyVersionsNuspecQuery>();
            serviceCollection.AddTransient<FindNonAsciiIdsNuspecQuery>();
            serviceCollection.AddTransient<FindInvalidPackageIdsNuspecQuery>();
            serviceCollection.AddTransient<FindInvalidPackageVersionsNuspecQuery>();
            serviceCollection.AddTransient<FindPackageVersionsContainingWhitespaceNuspecQuery>();
            serviceCollection.AddTransient<FindInvalidDependencyIdNuspecQuery>();
            serviceCollection.AddTransient<FindInvalidDependencyTargetFrameworkNuspecQuery>();
            serviceCollection.AddTransient<FindMixedDependencyGroupStylesNuspecQuery>();
            serviceCollection.AddTransient<FindWhitespaceDependencyTargetFrameworkNuspecQuery>();
            serviceCollection.AddTransient<FindUnsupportedDependencyTargetFrameworkNuspecQuery>();
            serviceCollection.AddTransient<FindDuplicateDependencyTargetFrameworksNuspecQuery>();
            serviceCollection.AddTransient<FindDuplicateNormalizedDependencyTargetFrameworksNuspecQuery>();
            serviceCollection.AddTransient<FindEmptyDependencyIdsNuspecQuery>();
            serviceCollection.AddTransient<FindWhitespaceDependencyIdsNuspecQuery>();
            serviceCollection.AddTransient<FindWhitespaceDependencyVersionsNuspecQuery>();
            serviceCollection.AddTransient<FindDuplicateDependenciesNuspecQuery>();
            serviceCollection.AddTransient<FindCaseSensitiveDuplicateMetadataElementsNuspecQuery>();
            serviceCollection.AddTransient<FindCaseInsensitiveDuplicateMetadataElementsNuspecQuery>();
            serviceCollection.AddTransient<FindNonAlphabetMetadataElementsNuspecQuery>();
            serviceCollection.AddTransient<FindCollidingMetadataElementsNuspecQuery>();
            serviceCollection.AddTransient<FindUnexpectedValuesForBooleanMetadataNuspecQuery>();

            if (settings.RunBoringQueries)
            {
                serviceCollection.AddTransient<FindNonNormalizedPackageVersionsNuspecQuery>();
                serviceCollection.AddTransient<FindMissingDependencyVersionsNuspecQuery>();
                serviceCollection.AddTransient<FindEmptyDependencyVersionsNuspecQuery>();
            }

            if (settings.RunConsistencyChecks)
            {
                serviceCollection.AddTransient<IPackageQuery, HasV2DiscrepancyPackageQuery>();
                serviceCollection.AddTransient<IPackageQuery, HasPackagesContainerDiscrepancyPackageQuery>();
                serviceCollection.AddTransient<IPackageQuery, HasFlatContainerDiscrepancyPackageQuery>();
                serviceCollection.AddTransient<IPackageQuery, HasRegistrationDiscrepancyInOriginalHivePackageQuery>();
                serviceCollection.AddTransient<IPackageQuery, HasRegistrationDiscrepancyInGzippedHivePackageQuery>();
                serviceCollection.AddTransient<IPackageQuery, HasRegistrationDiscrepancyInSemVer2HivePackageQuery>();
                serviceCollection.AddTransient<IPackageQuery, HasSearchDiscrepancyPackageQuery>();
                serviceCollection.AddTransient<IPackageQuery, HasCrossCheckDiscrepancyPackageQuery>();
            }

            serviceCollection.AddTransient<IPackageQuery, HasMissingNuspecPackageQuery>();

            // Add all of the .nuspec queries as package queries.
            var nuspecQueryDescriptors = serviceCollection
                .Where(x => typeof(INuspecQuery).IsAssignableFrom(x.ServiceType))
                .ToList();
            foreach (var nuspecQueryDescriptor in nuspecQueryDescriptors)
            {
                serviceCollection.AddTransient<IPackageQuery>(x =>
                {
                    var nuspecQuery = (INuspecQuery)x.GetRequiredService(nuspecQueryDescriptor.ImplementationType);
                    return new NuspecPackageQuery(nuspecQuery);
                });
            }

            return serviceCollection;
        }
    }
}
