// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace NuGet.Insights
{
    public class RegistrationCatalogEntry
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("listed")]
        public bool Listed { get; set; }
    }
}
