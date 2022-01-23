// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NuGet.Insights
{
    public class RegistrationPage
    {
        [JsonPropertyName("items")]
        public List<RegistrationLeafItem> Items { get; set; }
    }
}
