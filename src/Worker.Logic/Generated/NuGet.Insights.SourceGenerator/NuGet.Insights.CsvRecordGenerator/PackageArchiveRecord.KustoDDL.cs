﻿// <auto-generated />

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace NuGet.Insights
{
    static partial class KustoDDL
    {
        public const string PackageArchiveRecordDefaultTableName = "PackageArchives";

        public static readonly IReadOnlyList<string> PackageArchiveRecordDDL = new[]
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
    Size: long,
    MD5: string,
    SHA1: string,
    SHA256: string,
    SHA512: string,
    OffsetAfterEndOfCentralDirectory: long,
    CentralDirectorySize: int,
    OffsetOfCentralDirectory: int,
    EntryCount: int,
    Comment: string
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
    '{""Column"":""Size"",""DataType"":""long"",""Properties"":{""Ordinal"":9}},'
    '{""Column"":""MD5"",""DataType"":""string"",""Properties"":{""Ordinal"":10}},'
    '{""Column"":""SHA1"",""DataType"":""string"",""Properties"":{""Ordinal"":11}},'
    '{""Column"":""SHA256"",""DataType"":""string"",""Properties"":{""Ordinal"":12}},'
    '{""Column"":""SHA512"",""DataType"":""string"",""Properties"":{""Ordinal"":13}},'
    '{""Column"":""OffsetAfterEndOfCentralDirectory"",""DataType"":""long"",""Properties"":{""Ordinal"":14}},'
    '{""Column"":""CentralDirectorySize"",""DataType"":""int"",""Properties"":{""Ordinal"":15}},'
    '{""Column"":""OffsetOfCentralDirectory"",""DataType"":""int"",""Properties"":{""Ordinal"":16}},'
    '{""Column"":""EntryCount"",""DataType"":""int"",""Properties"":{""Ordinal"":17}},'
    '{""Column"":""Comment"",""DataType"":""string"",""Properties"":{""Ordinal"":18}}'
']'",
        };

        public const string PackageArchiveRecordPartitioningPolicy = @".alter table __TABLENAME__ policy partitioning '{'
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

        private static readonly bool PackageArchiveRecordAddTypeToDefaultTableName = AddTypeToDefaultTableName(typeof(NuGet.Insights.Worker.PackageArchiveToCsv.PackageArchiveRecord), PackageArchiveRecordDefaultTableName);

        private static readonly bool PackageArchiveRecordAddTypeToDDL = AddTypeToDDL(typeof(NuGet.Insights.Worker.PackageArchiveToCsv.PackageArchiveRecord), PackageArchiveRecordDDL);

        private static readonly bool PackageArchiveRecordAddTypeToPartitioningPolicy = AddTypeToPartitioningPolicy(typeof(NuGet.Insights.Worker.PackageArchiveToCsv.PackageArchiveRecord), PackageArchiveRecordPartitioningPolicy);
    }
}
