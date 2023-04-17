// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NuGet.Insights.Worker
{
    public class HeterogeneousBulkEnqueueMessage
    {
        [JsonPropertyName("m")]
        public List<JsonElement> Messages { get; set; }
    }
}