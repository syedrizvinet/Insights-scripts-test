﻿// <auto-generated />

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace NuGet.Insights
{
    static partial class KustoDDL
    {
        public const string PackageCompatibilityDefaultTableName = "PackageCompatibilities";

        public static readonly IReadOnlyList<string> PackageCompatibilityDDL = new[]
        {
            ".drop table __TABLENAME__ ifexists",

            @".create table __TABLENAME__ (
    LowerId: string,
    Identity: string,
    Id: string,
    Version: string,
    CatalogCommitTimestamp: datetime,
    Created: datetime,
    ResultType: string,
    HasError: bool,
    DoesNotRoundTrip: bool,
    HasAny: bool,
    HasUnsupported: bool,
    HasAgnostic: bool,
    BrokenFrameworks: dynamic,
    NuspecReader: dynamic,
    NU1202: dynamic,
    NuGetGallery: dynamic,
    NuGetGalleryEscaped: dynamic,
    NuGetGallerySupported: dynamic,
    NuGetGalleryBadges: dynamic
)",

            ".alter-merge table __TABLENAME__ policy retention softdelete = 30d",

            @".create table __TABLENAME__ ingestion csv mapping 'BlobStorageMapping'
'['
    '{""Column"":""LowerId"",""DataType"":""string"",""Properties"":{""Ordinal"":2}},'
    '{""Column"":""Identity"",""DataType"":""string"",""Properties"":{""Ordinal"":3}},'
    '{""Column"":""Id"",""DataType"":""string"",""Properties"":{""Ordinal"":4}},'
    '{""Column"":""Version"",""DataType"":""string"",""Properties"":{""Ordinal"":5}},'
    '{""Column"":""CatalogCommitTimestamp"",""DataType"":""datetime"",""Properties"":{""Ordinal"":6}},'
    '{""Column"":""Created"",""DataType"":""datetime"",""Properties"":{""Ordinal"":7}},'
    '{""Column"":""ResultType"",""DataType"":""string"",""Properties"":{""Ordinal"":8}},'
    '{""Column"":""HasError"",""DataType"":""bool"",""Properties"":{""Ordinal"":9}},'
    '{""Column"":""DoesNotRoundTrip"",""DataType"":""bool"",""Properties"":{""Ordinal"":10}},'
    '{""Column"":""HasAny"",""DataType"":""bool"",""Properties"":{""Ordinal"":11}},'
    '{""Column"":""HasUnsupported"",""DataType"":""bool"",""Properties"":{""Ordinal"":12}},'
    '{""Column"":""HasAgnostic"",""DataType"":""bool"",""Properties"":{""Ordinal"":13}},'
    '{""Column"":""BrokenFrameworks"",""DataType"":""dynamic"",""Properties"":{""Ordinal"":14}},'
    '{""Column"":""NuspecReader"",""DataType"":""dynamic"",""Properties"":{""Ordinal"":15}},'
    '{""Column"":""NU1202"",""DataType"":""dynamic"",""Properties"":{""Ordinal"":16}},'
    '{""Column"":""NuGetGallery"",""DataType"":""dynamic"",""Properties"":{""Ordinal"":17}},'
    '{""Column"":""NuGetGalleryEscaped"",""DataType"":""dynamic"",""Properties"":{""Ordinal"":18}},'
    '{""Column"":""NuGetGallerySupported"",""DataType"":""dynamic"",""Properties"":{""Ordinal"":19}},'
    '{""Column"":""NuGetGalleryBadges"",""DataType"":""dynamic"",""Properties"":{""Ordinal"":20}}'
']'",
        };

        public const string PackageCompatibilityPartitioningPolicy = @".alter table __TABLENAME__ policy partitioning '{'
  '""PartitionKeys"": ['
    '{'
      '""ColumnName"": ""Identity"",'
      '""Kind"": ""Hash"",'
      '""Properties"": {'
        '""Function"": ""XxHash64"",'
        '""MaxPartitionCount"": 256'
      '}'
    '}'
  ']'
'}'";

        private static readonly bool PackageCompatibilityAddTypeToDefaultTableName = AddTypeToDefaultTableName(typeof(NuGet.Insights.Worker.PackageCompatibilityToCsv.PackageCompatibility), PackageCompatibilityDefaultTableName);

        private static readonly bool PackageCompatibilityAddTypeToDDL = AddTypeToDDL(typeof(NuGet.Insights.Worker.PackageCompatibilityToCsv.PackageCompatibility), PackageCompatibilityDDL);

        private static readonly bool PackageCompatibilityAddTypeToPartitioningPolicy = AddTypeToPartitioningPolicy(typeof(NuGet.Insights.Worker.PackageCompatibilityToCsv.PackageCompatibility), PackageCompatibilityPartitioningPolicy);
    }
}
