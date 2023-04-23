﻿// <auto-generated />

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NuGet.Insights;

namespace NuGet.Insights.Worker.PackageVersionToCsv
{
    /* Kusto DDL:

    .drop table PackageVersions ifexists;

    .create table PackageVersions (
        LowerId: string,
        Identity: string,
        Id: string,
        Version: string,
        CatalogCommitTimestamp: datetime,
        Created: datetime,
        ResultType: string,
        OriginalVersion: string,
        FullVersion: string,
        Major: int,
        Minor: int,
        Patch: int,
        Revision: int,
        Release: string,
        ReleaseLabels: dynamic,
        Metadata: string,
        IsPrerelease: bool,
        IsListed: bool,
        IsSemVer2: bool,
        SemVerType: string,
        SemVerOrder: int,
        IsLatest: bool,
        IsLatestStable: bool,
        IsLatestSemVer2: bool,
        IsLatestStableSemVer2: bool
    ) with (docstring = "See https://github.com/NuGet/Insights/blob/main/docs/tables/PackageVersions.md", folder = "");

    .alter-merge table PackageVersions policy retention softdelete = 30d;

    .alter table PackageVersions policy partitioning '{'
      '"PartitionKeys": ['
        '{'
          '"ColumnName": "Identity",'
          '"Kind": "Hash",'
          '"Properties": {'
            '"Function": "XxHash64",'
            '"MaxPartitionCount": 256'
          '}'
        '}'
      ']'
    '}';

    .create table PackageVersions ingestion csv mapping 'BlobStorageMapping'
    '['
        '{"Column":"LowerId","DataType":"string","Properties":{"Ordinal":2}},'
        '{"Column":"Identity","DataType":"string","Properties":{"Ordinal":3}},'
        '{"Column":"Id","DataType":"string","Properties":{"Ordinal":4}},'
        '{"Column":"Version","DataType":"string","Properties":{"Ordinal":5}},'
        '{"Column":"CatalogCommitTimestamp","DataType":"datetime","Properties":{"Ordinal":6}},'
        '{"Column":"Created","DataType":"datetime","Properties":{"Ordinal":7}},'
        '{"Column":"ResultType","DataType":"string","Properties":{"Ordinal":8}},'
        '{"Column":"OriginalVersion","DataType":"string","Properties":{"Ordinal":9}},'
        '{"Column":"FullVersion","DataType":"string","Properties":{"Ordinal":10}},'
        '{"Column":"Major","DataType":"int","Properties":{"Ordinal":11}},'
        '{"Column":"Minor","DataType":"int","Properties":{"Ordinal":12}},'
        '{"Column":"Patch","DataType":"int","Properties":{"Ordinal":13}},'
        '{"Column":"Revision","DataType":"int","Properties":{"Ordinal":14}},'
        '{"Column":"Release","DataType":"string","Properties":{"Ordinal":15}},'
        '{"Column":"ReleaseLabels","DataType":"dynamic","Properties":{"Ordinal":16}},'
        '{"Column":"Metadata","DataType":"string","Properties":{"Ordinal":17}},'
        '{"Column":"IsPrerelease","DataType":"bool","Properties":{"Ordinal":18}},'
        '{"Column":"IsListed","DataType":"bool","Properties":{"Ordinal":19}},'
        '{"Column":"IsSemVer2","DataType":"bool","Properties":{"Ordinal":20}},'
        '{"Column":"SemVerType","DataType":"string","Properties":{"Ordinal":21}},'
        '{"Column":"SemVerOrder","DataType":"int","Properties":{"Ordinal":22}},'
        '{"Column":"IsLatest","DataType":"bool","Properties":{"Ordinal":23}},'
        '{"Column":"IsLatestStable","DataType":"bool","Properties":{"Ordinal":24}},'
        '{"Column":"IsLatestSemVer2","DataType":"bool","Properties":{"Ordinal":25}},'
        '{"Column":"IsLatestStableSemVer2","DataType":"bool","Properties":{"Ordinal":26}}'
    ']'

    */
    partial record PackageVersionRecord
    {
        public int FieldCount => 27;

        public void WriteHeader(TextWriter writer)
        {
            writer.WriteLine("ScanId,ScanTimestamp,LowerId,Identity,Id,Version,CatalogCommitTimestamp,Created,ResultType,OriginalVersion,FullVersion,Major,Minor,Patch,Revision,Release,ReleaseLabels,Metadata,IsPrerelease,IsListed,IsSemVer2,SemVerType,SemVerOrder,IsLatest,IsLatestStable,IsLatestSemVer2,IsLatestStableSemVer2");
        }

        public void Write(List<string> fields)
        {
            fields.Add(ScanId.ToString());
            fields.Add(CsvUtility.FormatDateTimeOffset(ScanTimestamp));
            fields.Add(LowerId);
            fields.Add(Identity);
            fields.Add(Id);
            fields.Add(Version);
            fields.Add(CsvUtility.FormatDateTimeOffset(CatalogCommitTimestamp));
            fields.Add(CsvUtility.FormatDateTimeOffset(Created));
            fields.Add(ResultType.ToString());
            fields.Add(OriginalVersion);
            fields.Add(FullVersion);
            fields.Add(Major.ToString());
            fields.Add(Minor.ToString());
            fields.Add(Patch.ToString());
            fields.Add(Revision.ToString());
            fields.Add(Release);
            fields.Add(ReleaseLabels);
            fields.Add(Metadata);
            fields.Add(CsvUtility.FormatBool(IsPrerelease));
            fields.Add(CsvUtility.FormatBool(IsListed));
            fields.Add(CsvUtility.FormatBool(IsSemVer2));
            fields.Add(SemVerType.ToString());
            fields.Add(SemVerOrder.ToString());
            fields.Add(CsvUtility.FormatBool(IsLatest));
            fields.Add(CsvUtility.FormatBool(IsLatestStable));
            fields.Add(CsvUtility.FormatBool(IsLatestSemVer2));
            fields.Add(CsvUtility.FormatBool(IsLatestStableSemVer2));
        }

        public void Write(TextWriter writer)
        {
            writer.Write(ScanId);
            writer.Write(',');
            writer.Write(CsvUtility.FormatDateTimeOffset(ScanTimestamp));
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, LowerId);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, Identity);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, Id);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, Version);
            writer.Write(',');
            writer.Write(CsvUtility.FormatDateTimeOffset(CatalogCommitTimestamp));
            writer.Write(',');
            writer.Write(CsvUtility.FormatDateTimeOffset(Created));
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, ResultType.ToString());
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, OriginalVersion);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, FullVersion);
            writer.Write(',');
            writer.Write(Major);
            writer.Write(',');
            writer.Write(Minor);
            writer.Write(',');
            writer.Write(Patch);
            writer.Write(',');
            writer.Write(Revision);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, Release);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, ReleaseLabels);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, Metadata);
            writer.Write(',');
            writer.Write(CsvUtility.FormatBool(IsPrerelease));
            writer.Write(',');
            writer.Write(CsvUtility.FormatBool(IsListed));
            writer.Write(',');
            writer.Write(CsvUtility.FormatBool(IsSemVer2));
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, SemVerType.ToString());
            writer.Write(',');
            writer.Write(SemVerOrder);
            writer.Write(',');
            writer.Write(CsvUtility.FormatBool(IsLatest));
            writer.Write(',');
            writer.Write(CsvUtility.FormatBool(IsLatestStable));
            writer.Write(',');
            writer.Write(CsvUtility.FormatBool(IsLatestSemVer2));
            writer.Write(',');
            writer.Write(CsvUtility.FormatBool(IsLatestStableSemVer2));
            writer.WriteLine();
        }

        public async Task WriteAsync(TextWriter writer)
        {
            await writer.WriteAsync(ScanId.ToString());
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatDateTimeOffset(ScanTimestamp));
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, LowerId);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, Identity);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, Id);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, Version);
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatDateTimeOffset(CatalogCommitTimestamp));
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatDateTimeOffset(Created));
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, ResultType.ToString());
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, OriginalVersion);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, FullVersion);
            await writer.WriteAsync(',');
            await writer.WriteAsync(Major.ToString());
            await writer.WriteAsync(',');
            await writer.WriteAsync(Minor.ToString());
            await writer.WriteAsync(',');
            await writer.WriteAsync(Patch.ToString());
            await writer.WriteAsync(',');
            await writer.WriteAsync(Revision.ToString());
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, Release);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, ReleaseLabels);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, Metadata);
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatBool(IsPrerelease));
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatBool(IsListed));
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatBool(IsSemVer2));
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, SemVerType.ToString());
            await writer.WriteAsync(',');
            await writer.WriteAsync(SemVerOrder.ToString());
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatBool(IsLatest));
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatBool(IsLatestStable));
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatBool(IsLatestSemVer2));
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatBool(IsLatestStableSemVer2));
            await writer.WriteLineAsync();
        }

        public ICsvRecord ReadNew(Func<string> getNextField)
        {
            return new PackageVersionRecord
            {
                ScanId = CsvUtility.ParseNullable(getNextField(), Guid.Parse),
                ScanTimestamp = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset),
                LowerId = getNextField(),
                Identity = getNextField(),
                Id = getNextField(),
                Version = getNextField(),
                CatalogCommitTimestamp = CsvUtility.ParseDateTimeOffset(getNextField()),
                Created = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset),
                ResultType = Enum.Parse<PackageVersionResultType>(getNextField()),
                OriginalVersion = getNextField(),
                FullVersion = getNextField(),
                Major = int.Parse(getNextField()),
                Minor = int.Parse(getNextField()),
                Patch = int.Parse(getNextField()),
                Revision = int.Parse(getNextField()),
                Release = getNextField(),
                ReleaseLabels = getNextField(),
                Metadata = getNextField(),
                IsPrerelease = bool.Parse(getNextField()),
                IsListed = CsvUtility.ParseNullable(getNextField(), bool.Parse),
                IsSemVer2 = CsvUtility.ParseNullable(getNextField(), bool.Parse),
                SemVerType = CsvUtility.ParseNullable(getNextField(), Enum.Parse<NuGet.Insights.SemVerType>),
                SemVerOrder = int.Parse(getNextField()),
                IsLatest = bool.Parse(getNextField()),
                IsLatestStable = bool.Parse(getNextField()),
                IsLatestSemVer2 = bool.Parse(getNextField()),
                IsLatestStableSemVer2 = bool.Parse(getNextField()),
            };
        }
    }
}
