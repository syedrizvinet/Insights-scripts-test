﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Knapcode.ExplorePackages.Commands;
using Knapcode.ExplorePackages.Logic;
using Knapcode.ExplorePackages.Support;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Knapcode.ExplorePackages
{
    public static class ServiceCollectionExtensions
    {
        public static ServiceCollection AddExplorePackages(this ServiceCollection serviceCollection, ExplorePackagesSettings settings)
        {
            serviceCollection.AddSingleton(
                x => new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                });
            serviceCollection.AddSingleton(
                x =>
                {
                    var httpClient = new HttpClient(x.GetRequiredService<HttpClientHandler>());
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", UserAgent.UserAgentString);
                    return httpClient;
                });
            serviceCollection.AddSingleton(
                x => new HttpSource(
                    new PackageSource(settings.V3ServiceIndex),
                    () =>
                    {
                        var httpClientHandler = x.GetRequiredService<HttpClientHandler>();
                        var httpMessageHandler = new PassThroughHandler
                        {
                            InnerHandler = httpClientHandler
                        };
                        return Task.FromResult<HttpHandlerResource>(new HttpHandlerResourceV3(
                            httpClientHandler,
                            httpMessageHandler));
                    },
                    NullThrottle.Instance));
            serviceCollection.AddTransient(
                x => new PackagePathProvider(settings.PackagePath));

            serviceCollection.AddTransient(x => settings.Clone());
            serviceCollection.AddTransient<PackageQueryProcessor>();
            serviceCollection.AddTransient<CatalogToDatabaseProcessor>();
            serviceCollection.AddTransient<CatalogToNuspecsProcessor>();
            serviceCollection.AddTransient<NuspecDownloader>();
            serviceCollection.AddTransient<RemoteCursorReader>();
            serviceCollection.AddTransient<PortDiscoverer>();
            serviceCollection.AddTransient<SearchServiceUrlDiscoverer>();
            serviceCollection.AddTransient<SearchServiceCursorReader>();
            serviceCollection.AddTransient<PackageQueryContextBuilder>();

            serviceCollection.AddSingleton<ServiceIndexCache>();
            serviceCollection.AddTransient<GalleryClient>();
            serviceCollection.AddTransient<V2Client>();
            serviceCollection.AddTransient<PackagesContainerClient>();
            serviceCollection.AddTransient<FlatContainerClient>();
            serviceCollection.AddTransient<RegistrationClient>();
            serviceCollection.AddTransient<SearchClient>();

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
            serviceCollection.AddTransient<FindEmptyDependencyVersionsNuspecQuery>();
            serviceCollection.AddTransient<FindRepositoriesNuspecQuery>();
            serviceCollection.AddTransient<FindInvalidDependencyVersionsNuspecQuery>();
            serviceCollection.AddTransient<FindMissingDependencyVersionsNuspecQuery>();
            serviceCollection.AddTransient<FindMissingDependencyIdsNuspecQuery>();
            serviceCollection.AddTransient<FindPackageTypesNuspecQuery>();
            serviceCollection.AddTransient<FindSemVer2PackageVersionsNuspecQuery>();
            serviceCollection.AddTransient<FindSemVer2DependencyVersionsNuspecQuery>();
            serviceCollection.AddTransient<FindFloatingDependencyVersionsNuspecQuery>();

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