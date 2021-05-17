﻿// <auto-generated />

using System.Collections.Generic;

namespace NuGet.Insights
{
    static partial class KustoDDL
    {
        public const string PackageSignatureDefaultTableName = "PackageSignatures";

        public static readonly IReadOnlyList<string> PackageSignatureDDL = new[]
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
    HashAlgorithm: string,
    HashValue: string,
    AuthorSHA1: string,
    AuthorSHA256: string,
    AuthorSubject: string,
    AuthorNotBefore: datetime,
    AuthorNotAfter: datetime,
    AuthorIssuer: string,
    AuthorTimestampSHA1: string,
    AuthorTimestampSHA256: string,
    AuthorTimestampSubject: string,
    AuthorTimestampNotBefore: datetime,
    AuthorTimestampNotAfter: datetime,
    AuthorTimestampIssuer: string,
    AuthorTimestampValue: datetime,
    AuthorTimestampHasASN1Error: bool,
    RepositorySHA1: string,
    RepositorySHA256: string,
    RepositorySubject: string,
    RepositoryNotBefore: datetime,
    RepositoryNotAfter: datetime,
    RepositoryIssuer: string,
    RepositoryTimestampSHA1: string,
    RepositoryTimestampSHA256: string,
    RepositoryTimestampSubject: string,
    RepositoryTimestampNotBefore: datetime,
    RepositoryTimestampNotAfter: datetime,
    RepositoryTimestampIssuer: string,
    RepositoryTimestampValue: datetime,
    RepositoryTimestampHasASN1Error: bool,
    PackageOwners: dynamic
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
    '{""Column"":""HashAlgorithm"",""DataType"":""string"",""Properties"":{""Ordinal"":9}},'
    '{""Column"":""HashValue"",""DataType"":""string"",""Properties"":{""Ordinal"":10}},'
    '{""Column"":""AuthorSHA1"",""DataType"":""string"",""Properties"":{""Ordinal"":11}},'
    '{""Column"":""AuthorSHA256"",""DataType"":""string"",""Properties"":{""Ordinal"":12}},'
    '{""Column"":""AuthorSubject"",""DataType"":""string"",""Properties"":{""Ordinal"":13}},'
    '{""Column"":""AuthorNotBefore"",""DataType"":""datetime"",""Properties"":{""Ordinal"":14}},'
    '{""Column"":""AuthorNotAfter"",""DataType"":""datetime"",""Properties"":{""Ordinal"":15}},'
    '{""Column"":""AuthorIssuer"",""DataType"":""string"",""Properties"":{""Ordinal"":16}},'
    '{""Column"":""AuthorTimestampSHA1"",""DataType"":""string"",""Properties"":{""Ordinal"":17}},'
    '{""Column"":""AuthorTimestampSHA256"",""DataType"":""string"",""Properties"":{""Ordinal"":18}},'
    '{""Column"":""AuthorTimestampSubject"",""DataType"":""string"",""Properties"":{""Ordinal"":19}},'
    '{""Column"":""AuthorTimestampNotBefore"",""DataType"":""datetime"",""Properties"":{""Ordinal"":20}},'
    '{""Column"":""AuthorTimestampNotAfter"",""DataType"":""datetime"",""Properties"":{""Ordinal"":21}},'
    '{""Column"":""AuthorTimestampIssuer"",""DataType"":""string"",""Properties"":{""Ordinal"":22}},'
    '{""Column"":""AuthorTimestampValue"",""DataType"":""datetime"",""Properties"":{""Ordinal"":23}},'
    '{""Column"":""AuthorTimestampHasASN1Error"",""DataType"":""bool"",""Properties"":{""Ordinal"":24}},'
    '{""Column"":""RepositorySHA1"",""DataType"":""string"",""Properties"":{""Ordinal"":25}},'
    '{""Column"":""RepositorySHA256"",""DataType"":""string"",""Properties"":{""Ordinal"":26}},'
    '{""Column"":""RepositorySubject"",""DataType"":""string"",""Properties"":{""Ordinal"":27}},'
    '{""Column"":""RepositoryNotBefore"",""DataType"":""datetime"",""Properties"":{""Ordinal"":28}},'
    '{""Column"":""RepositoryNotAfter"",""DataType"":""datetime"",""Properties"":{""Ordinal"":29}},'
    '{""Column"":""RepositoryIssuer"",""DataType"":""string"",""Properties"":{""Ordinal"":30}},'
    '{""Column"":""RepositoryTimestampSHA1"",""DataType"":""string"",""Properties"":{""Ordinal"":31}},'
    '{""Column"":""RepositoryTimestampSHA256"",""DataType"":""string"",""Properties"":{""Ordinal"":32}},'
    '{""Column"":""RepositoryTimestampSubject"",""DataType"":""string"",""Properties"":{""Ordinal"":33}},'
    '{""Column"":""RepositoryTimestampNotBefore"",""DataType"":""datetime"",""Properties"":{""Ordinal"":34}},'
    '{""Column"":""RepositoryTimestampNotAfter"",""DataType"":""datetime"",""Properties"":{""Ordinal"":35}},'
    '{""Column"":""RepositoryTimestampIssuer"",""DataType"":""string"",""Properties"":{""Ordinal"":36}},'
    '{""Column"":""RepositoryTimestampValue"",""DataType"":""datetime"",""Properties"":{""Ordinal"":37}},'
    '{""Column"":""RepositoryTimestampHasASN1Error"",""DataType"":""bool"",""Properties"":{""Ordinal"":38}},'
    '{""Column"":""PackageOwners"",""DataType"":""dynamic"",""Properties"":{""Ordinal"":39}}'
']'",
        };

        private static readonly bool PackageSignatureAddTypeToDefaultTableName = AddTypeToDefaultTableName(typeof(NuGet.Insights.Worker.PackageSignatureToCsv.PackageSignature), PackageSignatureDefaultTableName);

        private static readonly bool PackageSignatureAddTypeToDDL = AddTypeToDDL(typeof(NuGet.Insights.Worker.PackageSignatureToCsv.PackageSignature), PackageSignatureDDL);
    }
}