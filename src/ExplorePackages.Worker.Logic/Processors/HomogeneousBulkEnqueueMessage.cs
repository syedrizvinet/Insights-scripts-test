﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Knapcode.ExplorePackages.Worker
{
    public class HomogeneousBulkEnqueueMessage
    {
        [JsonProperty("n")]
        public string SchemaName { get; }

        [JsonProperty("v")]
        public int SchemaVersion { get; }

        [JsonProperty("m")]
        public List<JToken> Messages { get; set; }

        [JsonProperty("d")]
        public TimeSpan NotBefore { get; set; }
    }
}
