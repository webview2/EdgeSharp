// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace EdgeSharp
{
    public static partial class Interop
    {
        public static string AppId = "c13006e1-71d4-4cb5-a1c7-4835b92239ca";
        public static partial class User32
        {
            [DllImport(Libraries.Shell32, SetLastError = true)]
            public static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);
        }
    }
}
