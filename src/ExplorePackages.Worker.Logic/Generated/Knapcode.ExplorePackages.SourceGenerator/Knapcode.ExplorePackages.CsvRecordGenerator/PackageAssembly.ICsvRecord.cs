﻿// <auto-generated />

using System;
using System.IO;
using Knapcode.ExplorePackages;

namespace Knapcode.ExplorePackages.Worker.FindPackageAssemblies
{
    /* Kusto DDL:

    .drop table JverPackageAssemblies;

    .create table JverPackageAssemblies (
        ScanId: guid,
        ScanTimestamp: datetime,
        Id: string,
        Version: string,
        CatalogCommitTimestamp: datetime,
        Created: datetime,
        ResultType: string,
        Path: string,
        FileName: string,
        FileExtension: string,
        TopLevelFolder: string,
        CompressedLength: long,
        EntryUncompressedLength: long,
        ActualUncompressedLength: long,
        FileSHA256: string,
        HasException: bool,
        AssemblyName: string,
        AssemblyVersion: string,
        Culture: string,
        AssemblyNameHasCultureNotFoundException: bool,
        AssemblyNameHasFileLoadException: bool,
        PublicKeyToken: string,
        PublicKeyTokenHasSecurityException: bool,
        HashAlgorithm: string,
        HasPublicKey: bool,
        PublicKeyLength: int,
        PublicKeySHA1: string
    );

    .create table JverPackageAssemblies ingestion csv mapping 'JverPackageAssemblies_mapping'
    '['
        '{"Column":"ScanId","DataType":"guid","Properties":{"Ordinal":0}},'
        '{"Column":"ScanTimestamp","DataType":"datetime","Properties":{"Ordinal":1}},'
        '{"Column":"Id","DataType":"string","Properties":{"Ordinal":2}},'
        '{"Column":"Version","DataType":"string","Properties":{"Ordinal":3}},'
        '{"Column":"CatalogCommitTimestamp","DataType":"datetime","Properties":{"Ordinal":4}},'
        '{"Column":"Created","DataType":"datetime","Properties":{"Ordinal":5}},'
        '{"Column":"ResultType","DataType":"string","Properties":{"Ordinal":6}},'
        '{"Column":"Path","DataType":"string","Properties":{"Ordinal":7}},'
        '{"Column":"FileName","DataType":"string","Properties":{"Ordinal":8}},'
        '{"Column":"FileExtension","DataType":"string","Properties":{"Ordinal":9}},'
        '{"Column":"TopLevelFolder","DataType":"string","Properties":{"Ordinal":10}},'
        '{"Column":"CompressedLength","DataType":"long","Properties":{"Ordinal":11}},'
        '{"Column":"EntryUncompressedLength","DataType":"long","Properties":{"Ordinal":12}},'
        '{"Column":"ActualUncompressedLength","DataType":"long","Properties":{"Ordinal":13}},'
        '{"Column":"FileSHA256","DataType":"string","Properties":{"Ordinal":14}},'
        '{"Column":"HasException","DataType":"bool","Properties":{"Ordinal":15}},'
        '{"Column":"AssemblyName","DataType":"string","Properties":{"Ordinal":16}},'
        '{"Column":"AssemblyVersion","DataType":"string","Properties":{"Ordinal":17}},'
        '{"Column":"Culture","DataType":"string","Properties":{"Ordinal":18}},'
        '{"Column":"AssemblyNameHasCultureNotFoundException","DataType":"bool","Properties":{"Ordinal":19}},'
        '{"Column":"AssemblyNameHasFileLoadException","DataType":"bool","Properties":{"Ordinal":20}},'
        '{"Column":"PublicKeyToken","DataType":"string","Properties":{"Ordinal":21}},'
        '{"Column":"PublicKeyTokenHasSecurityException","DataType":"bool","Properties":{"Ordinal":22}},'
        '{"Column":"HashAlgorithm","DataType":"string","Properties":{"Ordinal":23}},'
        '{"Column":"HasPublicKey","DataType":"bool","Properties":{"Ordinal":24}},'
        '{"Column":"PublicKeyLength","DataType":"int","Properties":{"Ordinal":25}},'
        '{"Column":"PublicKeySHA1","DataType":"string","Properties":{"Ordinal":26}}'
    ']'

    */
    partial record PackageAssembly
    {
        public void Write(TextWriter writer)
        {
            writer.Write(ScanId);
            writer.Write(',');
            writer.Write(CsvUtility.FormatDateTimeOffset(ScanTimestamp));
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
            CsvUtility.WriteWithQuotes(writer, Path);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, FileName);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, FileExtension);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, TopLevelFolder);
            writer.Write(',');
            writer.Write(CompressedLength);
            writer.Write(',');
            writer.Write(EntryUncompressedLength);
            writer.Write(',');
            writer.Write(ActualUncompressedLength);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, FileSHA256);
            writer.Write(',');
            writer.Write(CsvUtility.FormatBool(HasException));
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, AssemblyName);
            writer.Write(',');
            writer.Write(AssemblyVersion);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, Culture);
            writer.Write(',');
            writer.Write(CsvUtility.FormatBool(AssemblyNameHasCultureNotFoundException));
            writer.Write(',');
            writer.Write(CsvUtility.FormatBool(AssemblyNameHasFileLoadException));
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, PublicKeyToken);
            writer.Write(',');
            writer.Write(CsvUtility.FormatBool(PublicKeyTokenHasSecurityException));
            writer.Write(',');
            writer.Write(HashAlgorithm);
            writer.Write(',');
            writer.Write(CsvUtility.FormatBool(HasPublicKey));
            writer.Write(',');
            writer.Write(PublicKeyLength);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, PublicKeySHA1);
            writer.WriteLine();
        }

        public PackageAssembly Read(Func<string> getNextField)
        {
            return new PackageAssembly
            {
                ScanId = CsvUtility.ParseNullable(getNextField(), Guid.Parse),
                ScanTimestamp = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset),
                Id = getNextField(),
                Version = getNextField(),
                CatalogCommitTimestamp = CsvUtility.ParseDateTimeOffset(getNextField()),
                Created = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset),
                ResultType = Enum.Parse<PackageAssemblyResultType>(getNextField()),
                Path = getNextField(),
                FileName = getNextField(),
                FileExtension = getNextField(),
                TopLevelFolder = getNextField(),
                CompressedLength = CsvUtility.ParseNullable(getNextField(), long.Parse),
                EntryUncompressedLength = CsvUtility.ParseNullable(getNextField(), long.Parse),
                ActualUncompressedLength = CsvUtility.ParseNullable(getNextField(), long.Parse),
                FileSHA256 = getNextField(),
                HasException = bool.Parse(getNextField()),
                AssemblyName = getNextField(),
                AssemblyVersion = CsvUtility.ParseReference(getNextField(), System.Version.Parse),
                Culture = getNextField(),
                AssemblyNameHasCultureNotFoundException = CsvUtility.ParseNullable(getNextField(), bool.Parse),
                AssemblyNameHasFileLoadException = CsvUtility.ParseNullable(getNextField(), bool.Parse),
                PublicKeyToken = getNextField(),
                PublicKeyTokenHasSecurityException = CsvUtility.ParseNullable(getNextField(), bool.Parse),
                HashAlgorithm = CsvUtility.ParseNullable(getNextField(), Enum.Parse<System.Reflection.AssemblyHashAlgorithm>),
                HasPublicKey = CsvUtility.ParseNullable(getNextField(), bool.Parse),
                PublicKeyLength = CsvUtility.ParseNullable(getNextField(), int.Parse),
                PublicKeySHA1 = getNextField(),
            };
        }
    }
}
