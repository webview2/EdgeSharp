﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace EdgeSharp
{
    public static partial class Interop
    {
        public static partial class SHCore
        {
            public enum PROCESS_DPI_AWARENESS : int
            {
                UNAWARE = 0,
                SYSTEM_AWARE = 1,
                PER_MONITOR_AWARE = 2
            }
        }
    }
}
