// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;

namespace EdgeSharp.Core.Borderless
{
    public class HookEventArgs : EventArgs
    {
        public int HookCode;    // Hook code
        public IntPtr wParam;   // WPARAM argument
        public IntPtr lParam;   // LPARAM argument
    }
}