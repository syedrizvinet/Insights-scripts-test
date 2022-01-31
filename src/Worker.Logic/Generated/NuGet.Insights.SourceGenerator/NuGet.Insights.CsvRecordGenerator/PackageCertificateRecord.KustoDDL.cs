﻿// <auto-generated />

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace NuGet.Insights
{
    static partial class KustoDDL
    {
        public const string PackageCertificateRecordDefaultTableName = "PackageCertificates";

        public static readonly IReadOnlyList<string> PackageCertificateRecordDDL = new[]
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
    Fingerprint: string,
    RelationshipTypes: string
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
    '{""Column"":""Fingerprint"",""DataType"":""string"",""Properties"":{""Ordinal"":9}},'
    '{""Column"":""RelationshipTypes"",""DataType"":""string"",""Properties"":{""Ordinal"":10}}'
']'",
        };

        public const string PackageCertificateRecordPartitioningPolicy = @".alter table __TABLENAME__ policy partitioning '{'
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

        private static readonly bool PackageCertificateRecordAddTypeToDefaultTableName = AddTypeToDefaultTableName(typeof(NuGet.Insights.Worker.PackageCertificateToCsv.PackageCertificateRecord), PackageCertificateRecordDefaultTableName);

        private static readonly bool PackageCertificateRecordAddTypeToDDL = AddTypeToDDL(typeof(NuGet.Insights.Worker.PackageCertificateToCsv.PackageCertificateRecord), PackageCertificateRecordDDL);

        private static readonly bool PackageCertificateRecordAddTypeToPartitioningPolicy = AddTypeToPartitioningPolicy(typeof(NuGet.Insights.Worker.PackageCertificateToCsv.PackageCertificateRecord), PackageCertificateRecordPartitioningPolicy);
    }
}
