﻿// <auto-generated />

using System.Collections.Generic;

namespace NuGet.Insights
{
    static partial class KustoDDL
    {
        public const string PackageAssetDefaultTableName = "PackageAssets";

        public static readonly IReadOnlyList<string> PackageAssetDDL = new[]
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
    PatternSet: string,
    PropertyAnyValue: string,
    PropertyCodeLanguage: string,
    PropertyTargetFrameworkMoniker: string,
    PropertyLocale: string,
    PropertyManagedAssembly: string,
    PropertyMSBuild: string,
    PropertyRuntimeIdentifier: string,
    PropertySatelliteAssembly: string,
    Path: string,
    FileName: string,
    FileExtension: string,
    TopLevelFolder: string,
    RoundTripTargetFrameworkMoniker: string,
    FrameworkName: string,
    FrameworkVersion: string,
    FrameworkProfile: string,
    PlatformName: string,
    PlatformVersion: string
)",

            ".alter-merge table __TABLENAME__ policy retention softdelete = 30d",

            @".alter table __TABLENAME__ policy partitioning '{'
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
'}'",

            @".create table __TABLENAME__ ingestion csv mapping 'BlobStorageMapping'
'['
    '{""Column"":""LowerId"",""DataType"":""string"",""Properties"":{""Ordinal"":2}},'
    '{""Column"":""Identity"",""DataType"":""string"",""Properties"":{""Ordinal"":3}},'
    '{""Column"":""Id"",""DataType"":""string"",""Properties"":{""Ordinal"":4}},'
    '{""Column"":""Version"",""DataType"":""string"",""Properties"":{""Ordinal"":5}},'
    '{""Column"":""CatalogCommitTimestamp"",""DataType"":""datetime"",""Properties"":{""Ordinal"":6}},'
    '{""Column"":""Created"",""DataType"":""datetime"",""Properties"":{""Ordinal"":7}},'
    '{""Column"":""ResultType"",""DataType"":""string"",""Properties"":{""Ordinal"":8}},'
    '{""Column"":""PatternSet"",""DataType"":""string"",""Properties"":{""Ordinal"":9}},'
    '{""Column"":""PropertyAnyValue"",""DataType"":""string"",""Properties"":{""Ordinal"":10}},'
    '{""Column"":""PropertyCodeLanguage"",""DataType"":""string"",""Properties"":{""Ordinal"":11}},'
    '{""Column"":""PropertyTargetFrameworkMoniker"",""DataType"":""string"",""Properties"":{""Ordinal"":12}},'
    '{""Column"":""PropertyLocale"",""DataType"":""string"",""Properties"":{""Ordinal"":13}},'
    '{""Column"":""PropertyManagedAssembly"",""DataType"":""string"",""Properties"":{""Ordinal"":14}},'
    '{""Column"":""PropertyMSBuild"",""DataType"":""string"",""Properties"":{""Ordinal"":15}},'
    '{""Column"":""PropertyRuntimeIdentifier"",""DataType"":""string"",""Properties"":{""Ordinal"":16}},'
    '{""Column"":""PropertySatelliteAssembly"",""DataType"":""string"",""Properties"":{""Ordinal"":17}},'
    '{""Column"":""Path"",""DataType"":""string"",""Properties"":{""Ordinal"":18}},'
    '{""Column"":""FileName"",""DataType"":""string"",""Properties"":{""Ordinal"":19}},'
    '{""Column"":""FileExtension"",""DataType"":""string"",""Properties"":{""Ordinal"":20}},'
    '{""Column"":""TopLevelFolder"",""DataType"":""string"",""Properties"":{""Ordinal"":21}},'
    '{""Column"":""RoundTripTargetFrameworkMoniker"",""DataType"":""string"",""Properties"":{""Ordinal"":22}},'
    '{""Column"":""FrameworkName"",""DataType"":""string"",""Properties"":{""Ordinal"":23}},'
    '{""Column"":""FrameworkVersion"",""DataType"":""string"",""Properties"":{""Ordinal"":24}},'
    '{""Column"":""FrameworkProfile"",""DataType"":""string"",""Properties"":{""Ordinal"":25}},'
    '{""Column"":""PlatformName"",""DataType"":""string"",""Properties"":{""Ordinal"":26}},'
    '{""Column"":""PlatformVersion"",""DataType"":""string"",""Properties"":{""Ordinal"":27}}'
']'",
        };

        private static readonly bool PackageAssetAddTypeToDefaultTableName = AddTypeToDefaultTableName(typeof(NuGet.Insights.Worker.PackageAssetToCsv.PackageAsset), PackageAssetDefaultTableName);

        private static readonly bool PackageAssetAddTypeToDDL = AddTypeToDDL(typeof(NuGet.Insights.Worker.PackageAssetToCsv.PackageAsset), PackageAssetDDL);
    }
}