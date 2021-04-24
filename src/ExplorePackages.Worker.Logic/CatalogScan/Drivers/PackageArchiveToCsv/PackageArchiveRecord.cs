﻿using System;

namespace Knapcode.ExplorePackages.Worker.PackageArchiveToCsv
{
    public partial record PackageArchiveRecord : PackageRecord, ICsvRecord
    {
        public PackageArchiveRecord()
        {
        }

        public PackageArchiveRecord(Guid? scanId, DateTimeOffset? scanTimestamp, PackageDeleteCatalogLeaf leaf)
            : base(scanId, scanTimestamp, leaf)
        {
            ResultType = PackageArchiveResultType.Deleted;
        }

        public PackageArchiveRecord(Guid? scanId, DateTimeOffset? scanTimestamp, PackageDetailsCatalogLeaf leaf)
            : base(scanId, scanTimestamp, leaf)
        {
            ResultType = PackageArchiveResultType.Available;
        }

        public PackageArchiveResultType ResultType { get; set; }
        
        public long Size { get; set; }

        public long OffsetAfterEndOfCentralDirectory { get; set; }
        public long CentralDirectorySize { get; set; }
        public long OffsetOfCentralDirectory { get; set; }
        public long EntryCount { get; set; }
        public string Comment { get; set; }
    }
}
