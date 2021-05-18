﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace NuGet.Insights
{
    public class NullThrottle : IThrottle, Protocol.IThrottle, Knapcode.MiniZip.IThrottle
    {
        public static NullThrottle Instance { get; } = new NullThrottle();

        public void Release()
        {
        }

        public Task WaitAsync()
        {
            return Task.CompletedTask;
        }
    }
}
