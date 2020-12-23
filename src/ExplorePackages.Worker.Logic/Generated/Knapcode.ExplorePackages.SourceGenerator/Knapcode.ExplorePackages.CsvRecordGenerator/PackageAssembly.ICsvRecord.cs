﻿// <auto-generated />
using System;
using System.IO;
using Knapcode.ExplorePackages;

namespace Knapcode.ExplorePackages.Worker.FindPackageAssemblies
{
    partial class PackageAssembly
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
            writer.Write(CompressedLength);
            writer.Write(',');
            writer.Write(UncompressedLength);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, Name);
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
            CsvUtility.WriteWithQuotes(writer, HashAlgorithm);
            writer.Write(',');
            writer.Write(CsvUtility.FormatBool(HasPublicKey));
            writer.Write(',');
            writer.Write(PublicKeyLength);
            writer.Write(',');
            CsvUtility.WriteWithQuotes(writer, PublicKeyHash);
            writer.WriteLine();
        }

        public void Read(Func<string> getNextField)
        {
            ScanId = CsvUtility.ParseNullable(getNextField(), Guid.Parse);
            ScanTimestamp = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset);
            Id = getNextField();
            Version = getNextField();
            CatalogCommitTimestamp = CsvUtility.ParseDateTimeOffset(getNextField());
            Created = CsvUtility.ParseNullable(getNextField(), CsvUtility.ParseDateTimeOffset);
            ResultType = Enum.Parse<PackageAssemblyResultType>(getNextField());
            Path = getNextField();
            CompressedLength = CsvUtility.ParseNullable(getNextField(), long.Parse);
            UncompressedLength = CsvUtility.ParseNullable(getNextField(), long.Parse);
            Name = getNextField();
            AssemblyVersion = CsvUtility.ParseReference(getNextField(), System.Version.Parse);
            Culture = getNextField();
            AssemblyNameHasCultureNotFoundException = CsvUtility.ParseNullable(getNextField(), bool.Parse);
            AssemblyNameHasFileLoadException = CsvUtility.ParseNullable(getNextField(), bool.Parse);
            PublicKeyToken = getNextField();
            PublicKeyTokenHasSecurityException = CsvUtility.ParseNullable(getNextField(), bool.Parse);
            HashAlgorithm = getNextField();
            HasPublicKey = CsvUtility.ParseNullable(getNextField(), bool.Parse);
            PublicKeyLength = CsvUtility.ParseNullable(getNextField(), int.Parse);
            PublicKeyHash = getNextField();
        }
    }
}
