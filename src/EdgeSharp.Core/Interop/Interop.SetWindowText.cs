﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace EdgeSharp
{
    public static partial class Interop
    {
        public static partial class User32
        {
            [DllImport(Libraries.User32, CharSet = CharSet.Unicode)]
            public static extern int SetWindowTextW(IntPtr hWnd, string text);

            public static int SetWindowText(IntPtr hWnd, string text)
            {
                return SetWindowTextW(hWnd, text);
            }

            public static int SetWindowTextW(HandleRef hWnd, string text)
            {
                int result = SetWindowTextW(hWnd.Handle, text);
                GC.KeepAlive(hWnd.Wrapper);
                return result;
            }
        }
    }
}
