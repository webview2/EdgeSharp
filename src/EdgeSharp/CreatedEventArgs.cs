// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;

namespace EdgeSharp
{
    public class CreatedEventArgs : EventArgs
    {
        public CreatedEventArgs(IntPtr handle, IntPtr winID)
        {
            Handle = handle;
            WinID = winID;
        }

        public IntPtr Handle { get; }
        public IntPtr WinID { get; }
    }
}
