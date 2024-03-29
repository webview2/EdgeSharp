﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace EdgeSharp
{
    public static partial class Interop
    {
        public static partial class SHCore
        {
            [DllImport(Libraries.SHCore, ExactSpelling = true)]
            public static extern HRESULT SetProcessDpiAwareness(PROCESS_DPI_AWARENESS value);
        }
    }
}
