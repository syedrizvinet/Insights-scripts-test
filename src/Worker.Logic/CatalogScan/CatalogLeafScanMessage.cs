﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Newtonsoft.Json;

namespace NuGet.Insights.Worker
{
    public class CatalogLeafScanMessage
    {
        [JsonProperty("s")]
        public string StorageSuffix { get; set; }

        [JsonProperty("p0")]
        public string ScanId { get; set; }

        [JsonProperty("p1")]
        public string PageId { get; set; }

        [JsonProperty("r")]
        public string LeafId { get; set; }
    }
}
