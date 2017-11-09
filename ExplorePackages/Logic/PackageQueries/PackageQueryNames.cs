﻿namespace Knapcode.ExplorePackages.Logic
{
    public static class PackageQueryNames
    {
        public const string FindMissingDependencyIdsNuspecQuery = CursorNames.FindMissingDependencyIdsNuspecQuery;
        public const string FindRepositoriesNuspecQuery = CursorNames.FindRepositoriesNuspecQuery;
        public const string FindPackageTypesNuspecQuery = CursorNames.FindPackageTypesNuspecQuery;
        public const string FindInvalidDependencyVersionsNuspecQuery = CursorNames.FindInvalidDependencyVersionsNuspecQuery;
        public const string FindMissingDependencyVersionsNuspecQuery = CursorNames.FindMissingDependencyVersionsNuspecQuery;
        public const string FindEmptyDependencyVersionsNuspecQuery = CursorNames.FindEmptyDependencyVersionsNuspecQuery;
        public const string FindIdsEndingInDotNumberNuspecQuery = CursorNames.FindIdsEndingInDotNumberNuspecQuery;
        public const string FindSemVer2PackageVersionsNuspecQuery = CursorNames.FindSemVer2PackageVersionsNuspecQuery;
        public const string FindSemVer2DependencyVersionsNuspecQuery = CursorNames.FindSemVer2DependencyVersionsNuspecQuery;
        public const string FindFloatingDependencyVersionsNuspecQuery = CursorNames.FindFloatingDependencyVersionsNuspecQuery;

        public const string HasRegistrationDiscrepancyInOriginalHivePackageQuery = CursorNames.HasRegistrationDiscrepancyInOriginalHivePackageQuery;
        public const string HasRegistrationDiscrepancyInGzippedHivePackageQuery = CursorNames.HasRegistrationDiscrepancyInGzippedHivePackageQuery;
        public const string HasRegistrationDiscrepancyInSemVer2HivePackageQuery = CursorNames.HasRegistrationDiscrepancyInSemVer2HivePackageQuery;
        public const string HasPackagesContainerDiscrepancyPackageQuery = CursorNames.HasPackagesContainerDiscrepancyPackageQuery;
    }
}
