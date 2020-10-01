﻿using CsvHelper.Configuration.Attributes;
using NuGet.ProjectModel;
using System;
using NuGetPackageIdentity = NuGet.Packaging.Core.PackageIdentity;

namespace Knapcode.ExplorePackages.Logic.Worker.RunRealRestore
{
    public class RealRestoreResult
    {
        public RealRestoreResult()
        {
        }

        public RealRestoreResult(DateTimeOffset timestamp, string dotnetVersion, TimeSpan duration, NuGetPackageIdentity package, ProjectProfile projectProfile)
        {
            Timestamp = timestamp;
            DotnetVersion = dotnetVersion;
            Duration = duration;
            Id = package.Id;
            Version = package.Version.ToNormalizedString();
            Framework = projectProfile.Framework.GetShortFolderName();
            Template = projectProfile.TemplateName;
        }

        public RealRestoreResult(
            DateTimeOffset timestamp,
            string dotnetVersion,
            TimeSpan duration,
            NuGetPackageIdentity package,
            ProjectProfile projectProfile,
            LockFile assetsFile,
            LockFileTarget target,
            LockFileTargetLibrary library)
            : this(timestamp, dotnetVersion, duration, package, projectProfile)
        {
            TargetCount = assetsFile.Targets.Count;
            LibraryCount = target.Libraries.Count;

            DependencyCount = library.Dependencies.Count;
            FrameworkAssemblyCount = library.FrameworkAssemblies.Count;
            FrameworkReferenceCount = library.FrameworkReferences.Count;
            RuntimeAssemblyCount = library.RuntimeAssemblies.Count;
            ResourceAssemblyCount = library.ResourceAssemblies.Count;
            CompileTimeAssemblyCount = library.CompileTimeAssemblies.Count;
            NativeLibraryCount = library.NativeLibraries.Count;
            BuildCount = library.Build.Count;
            BuildMultiTargetingCount = library.BuildMultiTargeting.Count;
            ContentFileCount = library.ContentFiles.Count;
            RuntimeTargetCount = library.RuntimeTargets.Count;
            ToolAssemblyCount = library.ToolsAssemblies.Count;
            EmbedAssemblyCount = library.EmbedAssemblies.Count;
        }

        [Index(0)] public DateTimeOffset Timestamp { get; }
        [Index(1)] public string DotnetVersion { get; }
        [Index(2)] public TimeSpan Duration { get; }

        [Index(3)] public string Id { get; set; }
        [Index(4)] public string Version { get; set; }
        [Index(5)] public string Framework { get; set; }
        [Index(6)] public string Template { get; }

        [Index(7)] public int DependencyCount { get; set; }
        [Index(8)] public int FrameworkAssemblyCount { get; set; }
        [Index(9)] public int FrameworkReferenceCount { get; set; }
        [Index(10)] public int RuntimeAssemblyCount { get; set; }
        [Index(11)] public int ResourceAssemblyCount { get; set; }
        [Index(12)] public int CompileTimeAssemblyCount { get; set; }
        [Index(13)] public int NativeLibraryCount { get; set; }
        [Index(14)] public int BuildCount { get; set; }
        [Index(15)] public int BuildMultiTargetingCount { get; set; }
        [Index(16)] public int ContentFileCount { get; set; }
        [Index(17)] public int RuntimeTargetCount { get; set; }
        [Index(18)] public int ToolAssemblyCount { get; set; }
        [Index(19)] public int EmbedAssemblyCount { get; set; }

        [Index(20)] public int TargetCount { get; set; }
        [Index(21)] public int LibraryCount { get; set; }

        [Index(22)] public bool RestoreSucceeded { get; set; }
        [Index(23)] public bool BuildSucceeded { get; set; }

        [Index(24)] public string ErrorBlobPath { get; set; }
    }
}