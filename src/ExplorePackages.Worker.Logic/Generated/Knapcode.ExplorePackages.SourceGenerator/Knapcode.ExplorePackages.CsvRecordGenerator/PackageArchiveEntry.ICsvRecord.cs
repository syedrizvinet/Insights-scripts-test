﻿// <auto-generated />

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Knapcode.ExplorePackages;

namespace Knapcode.ExplorePackages.Worker.PackageArchiveToCsv
{
    /* Kusto DDL:

    .drop table JverPackageArchiveEntries ifexists;

    .create table JverPackageArchiveEntries (
        LowerId: string,
        Identity: string,
        Id: string,
        Version: string,
        CatalogCommitTimestamp: datetime,
        Created: datetime,
        ResultType: string,
        SequenceNumber: int,
        Path: string,
        FileName: string,
        FileExtension: string,
        TopLevelFolder: string,
        Flags: int,
        CompressionMethod: int,
        LastModified: datetime,
        Crc32: long,
        CompressedSize: long,
        UncompressedSize: long,
        LocalHeaderOffset: long,
        Comment: string
    );

    .alter-merge table JverPackageArchiveEntries policy retention softdelete = 30d;

    .alter table JverPackageArchiveEntries policy partitioning '{'
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

    .create table JverPackageArchiveEntries ingestion csv mapping 'JverPackageArchiveEntries_mapping'
    '['
        '{"Column":"LowerId","DataType":"string","Properties":{"Ordinal":2}},'
        '{"Column":"Identity","DataType":"string","Properties":{"Ordinal":3}},'
        '{"Column":"Id","DataType":"string","Properties":{"Ordinal":4}},'
        '{"Column":"Version","DataType":"string","Properties":{"Ordinal":5}},'
        '{"Column":"CatalogCommitTimestamp","DataType":"datetime","Properties":{"Ordinal":6}},'
        '{"Column":"Created","DataType":"datetime","Properties":{"Ordinal":7}},'
        '{"Column":"ResultType","DataType":"string","Properties":{"Ordinal":8}},'
        '{"Column":"SequenceNumber","DataType":"int","Properties":{"Ordinal":9}},'
        '{"Column":"Path","DataType":"string","Properties":{"Ordinal":10}},'
        '{"Column":"FileName","DataType":"string","Properties":{"Ordinal":11}},'
        '{"Column":"FileExtension","DataType":"string","Properties":{"Ordinal":12}},'
        '{"Column":"TopLevelFolder","DataType":"string","Properties":{"Ordinal":13}},'
        '{"Column":"Flags","DataType":"int","Properties":{"Ordinal":14}},'
        '{"Column":"CompressionMethod","DataType":"int","Properties":{"Ordinal":15}},'
        '{"Column":"LastModified","DataType":"datetime","Properties":{"Ordinal":16}},'
        '{"Column":"Crc32","DataType":"long","Properties":{"Ordinal":17}},'
        '{"Column":"CompressedSize","DataType":"long","Properties":{"Ordinal":18}},'
        '{"Column":"UncompressedSize","DataType":"long","Properties":{"Ordinal":19}},'
        '{"Column":"LocalHeaderOffset","DataType":"long","Properties":{"Ordinal":20}},'
        '{"Column":"Comment","DataType":"string","Properties":{"Ordinal":21}}'
    ']'

    */
    partial record PackageArchiveEntry
    {
        public int FieldCount => 22;

        public void WriteHeader(TextWriter writer)
        {
            writer.WriteLine("ScanId,ScanTimestamp,LowerId,Identity,Id,Version,CatalogCommitTimestamp,Created,ResultType,SequenceNumber,Path,FileName,FileExtension,TopLevelFolder,Flags,CompressionMethod,LastModified,Crc32,CompressedSize,UncompressedSize,LocalHeaderOffset,Comment");
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
            fields.Add(SequenceNumber.ToString());
            fields.Add(Path);
            fields.Add(FileName);
            fields.Add(FileExtension);
            fields.Add(TopLevelFolder);
            fields.Add(Flags.ToString());
            fields.Add(CompressionMethod.ToString());
            fields.Add(CsvUtility.FormatDateTimeOffset(LastModified));
            fields.Add(Crc32.ToString());
            fields.Add(CompressedSize.ToString());
            fields.Add(UncompressedSize.ToString());
            fields.Add(LocalHeaderOffset.ToString());
            fields.Add(Comment);
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
            writer.Write(SequenceNumber);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, Path);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, FileName);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, FileExtension);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, TopLevelFolder);
            writer.Write(',');
            writer.Write(Flags);
            writer.Write(',');
            writer.Write(CompressionMethod);
            writer.Write(',');
            writer.Write(CsvUtility.FormatDateTimeOffset(LastModified));
            writer.Write(',');
            writer.Write(Crc32);
            writer.Write(',');
            writer.Write(CompressedSize);
            writer.Write(',');
            writer.Write(UncompressedSize);
            writer.Write(',');
            writer.Write(LocalHeaderOffset);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, Comment);
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
            await writer.WriteAsync(SequenceNumber.ToString());
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, Path);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, FileName);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, FileExtension);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, TopLevelFolder);
            await writer.WriteAsync(',');
            await writer.WriteAsync(Flags.ToString());
            await writer.WriteAsync(',');
            await writer.WriteAsync(CompressionMethod.ToString());
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatDateTimeOffset(LastModified));
            await writer.WriteAsync(',');
            await writer.WriteAsync(Crc32.ToString());
            await writer.WriteAsync(',');
            await writer.WriteAsync(CompressedSize.ToString());
            await writer.WriteAsync(',');
            await writer.WriteAsync(UncompressedSize.ToString());
            await writer.WriteAsync(',');
            await writer.WriteAsync(LocalHeaderOffset.ToString());
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, Comment);
            await writer.WriteLineAsync();
        }

        public ICsvRecord ReadNew(Func<string> getNextField)
        {
            return new PackageArchiveEntry
            {
                ScanId = CsvUtility.ParseNullable(getNextField(), Guid.Parse),
                ScanTimestamp = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset),
                LowerId = getNextField(),
                Identity = getNextField(),
                Id = getNextField(),
                Version = getNextField(),
                CatalogCommitTimestamp = CsvUtility.ParseDateTimeOffset(getNextField()),
                Created = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset),
                ResultType = Enum.Parse<PackageArchiveResultType>(getNextField()),
                SequenceNumber = int.Parse(getNextField()),
                Path = getNextField(),
                FileName = getNextField(),
                FileExtension = getNextField(),
                TopLevelFolder = getNextField(),
                Flags = int.Parse(getNextField()),
                CompressionMethod = int.Parse(getNextField()),
                LastModified = CsvUtility.ParseDateTimeOffset(getNextField()),
                Crc32 = long.Parse(getNextField()),
                CompressedSize = long.Parse(getNextField()),
                UncompressedSize = long.Parse(getNextField()),
                LocalHeaderOffset = long.Parse(getNextField()),
                Comment = getNextField(),
            };
        }
    }
}
