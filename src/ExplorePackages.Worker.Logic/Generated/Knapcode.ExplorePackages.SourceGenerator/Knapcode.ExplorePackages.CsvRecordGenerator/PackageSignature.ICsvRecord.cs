﻿// <auto-generated />

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Knapcode.ExplorePackages;

namespace Knapcode.ExplorePackages.Worker.FindPackageSignature
{
    /* Kusto DDL:

    .drop table JverPackageSignatures;

    .create table JverPackageSignatures (
        ScanId: guid,
        ScanTimestamp: datetime,
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
        PackageOwners: string
    );

    .create table JverPackageSignatures ingestion csv mapping 'JverPackageSignatures_mapping'
    '['
        '{"Column":"ScanId","DataType":"guid","Properties":{"Ordinal":0}},'
        '{"Column":"ScanTimestamp","DataType":"datetime","Properties":{"Ordinal":1}},'
        '{"Column":"LowerId","DataType":"string","Properties":{"Ordinal":2}},'
        '{"Column":"Identity","DataType":"string","Properties":{"Ordinal":3}},'
        '{"Column":"Id","DataType":"string","Properties":{"Ordinal":4}},'
        '{"Column":"Version","DataType":"string","Properties":{"Ordinal":5}},'
        '{"Column":"CatalogCommitTimestamp","DataType":"datetime","Properties":{"Ordinal":6}},'
        '{"Column":"Created","DataType":"datetime","Properties":{"Ordinal":7}},'
        '{"Column":"ResultType","DataType":"string","Properties":{"Ordinal":8}},'
        '{"Column":"HashAlgorithm","DataType":"string","Properties":{"Ordinal":9}},'
        '{"Column":"HashValue","DataType":"string","Properties":{"Ordinal":10}},'
        '{"Column":"AuthorSHA1","DataType":"string","Properties":{"Ordinal":11}},'
        '{"Column":"AuthorSHA256","DataType":"string","Properties":{"Ordinal":12}},'
        '{"Column":"AuthorSubject","DataType":"string","Properties":{"Ordinal":13}},'
        '{"Column":"AuthorNotBefore","DataType":"datetime","Properties":{"Ordinal":14}},'
        '{"Column":"AuthorNotAfter","DataType":"datetime","Properties":{"Ordinal":15}},'
        '{"Column":"AuthorIssuer","DataType":"string","Properties":{"Ordinal":16}},'
        '{"Column":"AuthorTimestampSHA1","DataType":"string","Properties":{"Ordinal":17}},'
        '{"Column":"AuthorTimestampSHA256","DataType":"string","Properties":{"Ordinal":18}},'
        '{"Column":"AuthorTimestampSubject","DataType":"string","Properties":{"Ordinal":19}},'
        '{"Column":"AuthorTimestampNotBefore","DataType":"datetime","Properties":{"Ordinal":20}},'
        '{"Column":"AuthorTimestampNotAfter","DataType":"datetime","Properties":{"Ordinal":21}},'
        '{"Column":"AuthorTimestampIssuer","DataType":"string","Properties":{"Ordinal":22}},'
        '{"Column":"AuthorTimestampValue","DataType":"datetime","Properties":{"Ordinal":23}},'
        '{"Column":"AuthorTimestampHasASN1Error","DataType":"bool","Properties":{"Ordinal":24}},'
        '{"Column":"RepositorySHA1","DataType":"string","Properties":{"Ordinal":25}},'
        '{"Column":"RepositorySHA256","DataType":"string","Properties":{"Ordinal":26}},'
        '{"Column":"RepositorySubject","DataType":"string","Properties":{"Ordinal":27}},'
        '{"Column":"RepositoryNotBefore","DataType":"datetime","Properties":{"Ordinal":28}},'
        '{"Column":"RepositoryNotAfter","DataType":"datetime","Properties":{"Ordinal":29}},'
        '{"Column":"RepositoryIssuer","DataType":"string","Properties":{"Ordinal":30}},'
        '{"Column":"RepositoryTimestampSHA1","DataType":"string","Properties":{"Ordinal":31}},'
        '{"Column":"RepositoryTimestampSHA256","DataType":"string","Properties":{"Ordinal":32}},'
        '{"Column":"RepositoryTimestampSubject","DataType":"string","Properties":{"Ordinal":33}},'
        '{"Column":"RepositoryTimestampNotBefore","DataType":"datetime","Properties":{"Ordinal":34}},'
        '{"Column":"RepositoryTimestampNotAfter","DataType":"datetime","Properties":{"Ordinal":35}},'
        '{"Column":"RepositoryTimestampIssuer","DataType":"string","Properties":{"Ordinal":36}},'
        '{"Column":"RepositoryTimestampValue","DataType":"datetime","Properties":{"Ordinal":37}},'
        '{"Column":"RepositoryTimestampHasASN1Error","DataType":"bool","Properties":{"Ordinal":38}},'
        '{"Column":"PackageOwners","DataType":"string","Properties":{"Ordinal":39}}'
    ']'

    */
    partial record PackageSignature
    {
        public int FieldCount => 40;

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
            fields.Add(HashAlgorithm.ToString());
            fields.Add(HashValue);
            fields.Add(AuthorSHA1);
            fields.Add(AuthorSHA256);
            fields.Add(AuthorSubject);
            fields.Add(CsvUtility.FormatDateTimeOffset(AuthorNotBefore));
            fields.Add(CsvUtility.FormatDateTimeOffset(AuthorNotAfter));
            fields.Add(AuthorIssuer);
            fields.Add(AuthorTimestampSHA1);
            fields.Add(AuthorTimestampSHA256);
            fields.Add(AuthorTimestampSubject);
            fields.Add(CsvUtility.FormatDateTimeOffset(AuthorTimestampNotBefore));
            fields.Add(CsvUtility.FormatDateTimeOffset(AuthorTimestampNotAfter));
            fields.Add(AuthorTimestampIssuer);
            fields.Add(CsvUtility.FormatDateTimeOffset(AuthorTimestampValue));
            fields.Add(CsvUtility.FormatBool(AuthorTimestampHasASN1Error));
            fields.Add(RepositorySHA1);
            fields.Add(RepositorySHA256);
            fields.Add(RepositorySubject);
            fields.Add(CsvUtility.FormatDateTimeOffset(RepositoryNotBefore));
            fields.Add(CsvUtility.FormatDateTimeOffset(RepositoryNotAfter));
            fields.Add(RepositoryIssuer);
            fields.Add(RepositoryTimestampSHA1);
            fields.Add(RepositoryTimestampSHA256);
            fields.Add(RepositoryTimestampSubject);
            fields.Add(CsvUtility.FormatDateTimeOffset(RepositoryTimestampNotBefore));
            fields.Add(CsvUtility.FormatDateTimeOffset(RepositoryTimestampNotAfter));
            fields.Add(RepositoryTimestampIssuer);
            fields.Add(CsvUtility.FormatDateTimeOffset(RepositoryTimestampValue));
            fields.Add(CsvUtility.FormatBool(RepositoryTimestampHasASN1Error));
            fields.Add(PackageOwners);
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
            writer.Write(ResultType);
            writer.Write(',');
            writer.Write(HashAlgorithm);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, HashValue);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, AuthorSHA1);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, AuthorSHA256);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, AuthorSubject);
            writer.Write(',');
            writer.Write(CsvUtility.FormatDateTimeOffset(AuthorNotBefore));
            writer.Write(',');
            writer.Write(CsvUtility.FormatDateTimeOffset(AuthorNotAfter));
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, AuthorIssuer);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, AuthorTimestampSHA1);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, AuthorTimestampSHA256);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, AuthorTimestampSubject);
            writer.Write(',');
            writer.Write(CsvUtility.FormatDateTimeOffset(AuthorTimestampNotBefore));
            writer.Write(',');
            writer.Write(CsvUtility.FormatDateTimeOffset(AuthorTimestampNotAfter));
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, AuthorTimestampIssuer);
            writer.Write(',');
            writer.Write(CsvUtility.FormatDateTimeOffset(AuthorTimestampValue));
            writer.Write(',');
            writer.Write(CsvUtility.FormatBool(AuthorTimestampHasASN1Error));
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, RepositorySHA1);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, RepositorySHA256);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, RepositorySubject);
            writer.Write(',');
            writer.Write(CsvUtility.FormatDateTimeOffset(RepositoryNotBefore));
            writer.Write(',');
            writer.Write(CsvUtility.FormatDateTimeOffset(RepositoryNotAfter));
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, RepositoryIssuer);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, RepositoryTimestampSHA1);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, RepositoryTimestampSHA256);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, RepositoryTimestampSubject);
            writer.Write(',');
            writer.Write(CsvUtility.FormatDateTimeOffset(RepositoryTimestampNotBefore));
            writer.Write(',');
            writer.Write(CsvUtility.FormatDateTimeOffset(RepositoryTimestampNotAfter));
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, RepositoryTimestampIssuer);
            writer.Write(',');
            writer.Write(CsvUtility.FormatDateTimeOffset(RepositoryTimestampValue));
            writer.Write(',');
            writer.Write(CsvUtility.FormatBool(RepositoryTimestampHasASN1Error));
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, PackageOwners);
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
            await writer.WriteAsync(ResultType.ToString());
            await writer.WriteAsync(',');
            await writer.WriteAsync(HashAlgorithm.ToString());
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, HashValue);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, AuthorSHA1);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, AuthorSHA256);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, AuthorSubject);
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatDateTimeOffset(AuthorNotBefore));
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatDateTimeOffset(AuthorNotAfter));
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, AuthorIssuer);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, AuthorTimestampSHA1);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, AuthorTimestampSHA256);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, AuthorTimestampSubject);
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatDateTimeOffset(AuthorTimestampNotBefore));
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatDateTimeOffset(AuthorTimestampNotAfter));
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, AuthorTimestampIssuer);
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatDateTimeOffset(AuthorTimestampValue));
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatBool(AuthorTimestampHasASN1Error));
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, RepositorySHA1);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, RepositorySHA256);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, RepositorySubject);
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatDateTimeOffset(RepositoryNotBefore));
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatDateTimeOffset(RepositoryNotAfter));
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, RepositoryIssuer);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, RepositoryTimestampSHA1);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, RepositoryTimestampSHA256);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, RepositoryTimestampSubject);
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatDateTimeOffset(RepositoryTimestampNotBefore));
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatDateTimeOffset(RepositoryTimestampNotAfter));
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, RepositoryTimestampIssuer);
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatDateTimeOffset(RepositoryTimestampValue));
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatBool(RepositoryTimestampHasASN1Error));
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, PackageOwners);
            await writer.WriteLineAsync();
        }

        public PackageSignature Read(Func<string> getNextField)
        {
            return new PackageSignature
            {
                ScanId = CsvUtility.ParseNullable(getNextField(), Guid.Parse),
                ScanTimestamp = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset),
                LowerId = getNextField(),
                Identity = getNextField(),
                Id = getNextField(),
                Version = getNextField(),
                CatalogCommitTimestamp = CsvUtility.ParseDateTimeOffset(getNextField()),
                Created = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset),
                ResultType = Enum.Parse<PackageSignatureResultType>(getNextField()),
                HashAlgorithm = Enum.Parse<NuGet.Common.HashAlgorithmName>(getNextField()),
                HashValue = getNextField(),
                AuthorSHA1 = getNextField(),
                AuthorSHA256 = getNextField(),
                AuthorSubject = getNextField(),
                AuthorNotBefore = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset),
                AuthorNotAfter = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset),
                AuthorIssuer = getNextField(),
                AuthorTimestampSHA1 = getNextField(),
                AuthorTimestampSHA256 = getNextField(),
                AuthorTimestampSubject = getNextField(),
                AuthorTimestampNotBefore = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset),
                AuthorTimestampNotAfter = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset),
                AuthorTimestampIssuer = getNextField(),
                AuthorTimestampValue = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset),
                AuthorTimestampHasASN1Error = bool.Parse(getNextField()),
                RepositorySHA1 = getNextField(),
                RepositorySHA256 = getNextField(),
                RepositorySubject = getNextField(),
                RepositoryNotBefore = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset),
                RepositoryNotAfter = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset),
                RepositoryIssuer = getNextField(),
                RepositoryTimestampSHA1 = getNextField(),
                RepositoryTimestampSHA256 = getNextField(),
                RepositoryTimestampSubject = getNextField(),
                RepositoryTimestampNotBefore = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset),
                RepositoryTimestampNotAfter = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset),
                RepositoryTimestampIssuer = getNextField(),
                RepositoryTimestampValue = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset),
                RepositoryTimestampHasASN1Error = bool.Parse(getNextField()),
                PackageOwners = getNextField(),
            };
        }
    }
}
