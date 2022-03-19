﻿// <auto-generated />

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace NuGet.Insights
{
    static partial class KustoDDL
    {
        public const string CertificateRecordDefaultTableName = "Certificates";

        public static readonly IReadOnlyList<string> CertificateRecordDDL = new[]
        {
            ".drop table __TABLENAME__ ifexists",

            @".create table __TABLENAME__ (
    ResultType: string,
    Fingerprint: string,
    FingerprintSHA256Hex: string,
    FingerprintSHA1Hex: string,
    Subject: string,
    Issuer: string,
    NotBefore: datetime,
    NotAfter: datetime,
    SerialNumber: string,
    SignatureAlgorithmOid: string,
    Version: int,
    Extensions: dynamic,
    PublicKeyOid: string,
    RawDataLength: int,
    RawData: string,
    IssuerFingerprint: string,
    RootFingerprint: string,
    ChainLength: int,
    CodeSigningCommitTimestamp: datetime,
    CodeSigningStatus: string,
    CodeSigningStatusFlags: string,
    CodeSigningStatusUpdateTime: datetime,
    CodeSigningRevocationTime: datetime,
    TimestampingCommitTimestamp: datetime,
    TimestampingStatus: string,
    TimestampingStatusFlags: string,
    TimestampingStatusUpdateTime: datetime,
    TimestampingRevocationTime: datetime
)",

            ".alter-merge table __TABLENAME__ policy retention softdelete = 30d",

            @".create table __TABLENAME__ ingestion csv mapping 'BlobStorageMapping'
'['
    '{""Column"":""ResultType"",""DataType"":""string"",""Properties"":{""Ordinal"":2}},'
    '{""Column"":""Fingerprint"",""DataType"":""string"",""Properties"":{""Ordinal"":3}},'
    '{""Column"":""FingerprintSHA256Hex"",""DataType"":""string"",""Properties"":{""Ordinal"":4}},'
    '{""Column"":""FingerprintSHA1Hex"",""DataType"":""string"",""Properties"":{""Ordinal"":5}},'
    '{""Column"":""Subject"",""DataType"":""string"",""Properties"":{""Ordinal"":6}},'
    '{""Column"":""Issuer"",""DataType"":""string"",""Properties"":{""Ordinal"":7}},'
    '{""Column"":""NotBefore"",""DataType"":""datetime"",""Properties"":{""Ordinal"":8}},'
    '{""Column"":""NotAfter"",""DataType"":""datetime"",""Properties"":{""Ordinal"":9}},'
    '{""Column"":""SerialNumber"",""DataType"":""string"",""Properties"":{""Ordinal"":10}},'
    '{""Column"":""SignatureAlgorithmOid"",""DataType"":""string"",""Properties"":{""Ordinal"":11}},'
    '{""Column"":""Version"",""DataType"":""int"",""Properties"":{""Ordinal"":12}},'
    '{""Column"":""Extensions"",""DataType"":""dynamic"",""Properties"":{""Ordinal"":13}},'
    '{""Column"":""PublicKeyOid"",""DataType"":""string"",""Properties"":{""Ordinal"":14}},'
    '{""Column"":""RawDataLength"",""DataType"":""int"",""Properties"":{""Ordinal"":15}},'
    '{""Column"":""RawData"",""DataType"":""string"",""Properties"":{""Ordinal"":16}},'
    '{""Column"":""IssuerFingerprint"",""DataType"":""string"",""Properties"":{""Ordinal"":17}},'
    '{""Column"":""RootFingerprint"",""DataType"":""string"",""Properties"":{""Ordinal"":18}},'
    '{""Column"":""ChainLength"",""DataType"":""int"",""Properties"":{""Ordinal"":19}},'
    '{""Column"":""CodeSigningCommitTimestamp"",""DataType"":""datetime"",""Properties"":{""Ordinal"":20}},'
    '{""Column"":""CodeSigningStatus"",""DataType"":""string"",""Properties"":{""Ordinal"":21}},'
    '{""Column"":""CodeSigningStatusFlags"",""DataType"":""string"",""Properties"":{""Ordinal"":22}},'
    '{""Column"":""CodeSigningStatusUpdateTime"",""DataType"":""datetime"",""Properties"":{""Ordinal"":23}},'
    '{""Column"":""CodeSigningRevocationTime"",""DataType"":""datetime"",""Properties"":{""Ordinal"":24}},'
    '{""Column"":""TimestampingCommitTimestamp"",""DataType"":""datetime"",""Properties"":{""Ordinal"":25}},'
    '{""Column"":""TimestampingStatus"",""DataType"":""string"",""Properties"":{""Ordinal"":26}},'
    '{""Column"":""TimestampingStatusFlags"",""DataType"":""string"",""Properties"":{""Ordinal"":27}},'
    '{""Column"":""TimestampingStatusUpdateTime"",""DataType"":""datetime"",""Properties"":{""Ordinal"":28}},'
    '{""Column"":""TimestampingRevocationTime"",""DataType"":""datetime"",""Properties"":{""Ordinal"":29}}'
']'",
        };

        public const string CertificateRecordPartitioningPolicy = @".alter table __TABLENAME__ policy partitioning '{'
  '""PartitionKeys"": ['
    '{'
      '""ColumnName"": ""Fingerprint"",'
      '""Kind"": ""Hash"",'
      '""Properties"": {'
        '""Function"": ""XxHash64"",'
        '""MaxPartitionCount"": 256'
      '}'
    '}'
  ']'
'}'";

        private static readonly bool CertificateRecordAddTypeToDefaultTableName = AddTypeToDefaultTableName(typeof(NuGet.Insights.Worker.PackageCertificateToCsv.CertificateRecord), CertificateRecordDefaultTableName);

        private static readonly bool CertificateRecordAddTypeToDDL = AddTypeToDDL(typeof(NuGet.Insights.Worker.PackageCertificateToCsv.CertificateRecord), CertificateRecordDDL);

        private static readonly bool CertificateRecordAddTypeToPartitioningPolicy = AddTypeToPartitioningPolicy(typeof(NuGet.Insights.Worker.PackageCertificateToCsv.CertificateRecord), CertificateRecordPartitioningPolicy);
    }
}