﻿// <auto-generated />

using System;
using System.IO;
using System.Threading.Tasks;
using Knapcode.ExplorePackages;

namespace Knapcode.ExplorePackages.Worker.FindCatalogLeafItem
{
    /* Kusto DDL:

    .drop table JverCatalogLeafItems;

    .create table JverCatalogLeafItems (
        CommitId: string,
        CommitTimestamp: datetime,
        Id: string,
        Version: string,
        Type: string,
        Url: string,
        LowerId: string,
        LowerNormalizedVersion: string,
        PageUrl: string
    );

    .create table JverCatalogLeafItems ingestion csv mapping 'JverCatalogLeafItems_mapping'
    '['
        '{"Column":"CommitId","DataType":"string","Properties":{"Ordinal":0}},'
        '{"Column":"CommitTimestamp","DataType":"datetime","Properties":{"Ordinal":1}},'
        '{"Column":"Id","DataType":"string","Properties":{"Ordinal":2}},'
        '{"Column":"Version","DataType":"string","Properties":{"Ordinal":3}},'
        '{"Column":"Type","DataType":"string","Properties":{"Ordinal":4}},'
        '{"Column":"Url","DataType":"string","Properties":{"Ordinal":5}},'
        '{"Column":"LowerId","DataType":"string","Properties":{"Ordinal":6}},'
        '{"Column":"LowerNormalizedVersion","DataType":"string","Properties":{"Ordinal":7}},'
        '{"Column":"PageUrl","DataType":"string","Properties":{"Ordinal":8}}'
    ']'

    */
    partial record CatalogLeafItemRecord
    {
        public void Write(TextWriter writer)
        {
            CsvUtility.WriteWithQuotes(writer, CommitId);
            writer.Write(',');
            writer.Write(CsvUtility.FormatDateTimeOffset(CommitTimestamp));
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, Id);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, Version);
            writer.Write(',');
            writer.Write(Type);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, Url);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, LowerId);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, LowerNormalizedVersion);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, PageUrl);
            writer.WriteLine();
        }

        public async Task WriteAsync(TextWriter writer)
        {
            await CsvUtility.WriteWithQuotesAsync(writer, CommitId);
            await writer.WriteAsync(',');
            await writer.WriteAsync(CsvUtility.FormatDateTimeOffset(CommitTimestamp));
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, Id);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, Version);
            await writer.WriteAsync(',');
            await writer.WriteAsync(Type.ToString());
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, Url);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, LowerId);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, LowerNormalizedVersion);
            await writer.WriteAsync(',');
            await CsvUtility.WriteWithQuotesAsync(writer, PageUrl);
            await writer.WriteLineAsync();
        }

        public CatalogLeafItemRecord Read(Func<string> getNextField)
        {
            return new CatalogLeafItemRecord
            {
                CommitId = getNextField(),
                CommitTimestamp = CsvUtility.ParseDateTimeOffset(getNextField()),
                Id = getNextField(),
                Version = getNextField(),
                Type = Enum.Parse<Knapcode.ExplorePackages.CatalogLeafType>(getNextField()),
                Url = getNextField(),
                LowerId = getNextField(),
                LowerNormalizedVersion = getNextField(),
                PageUrl = getNextField(),
            };
        }
    }
}
