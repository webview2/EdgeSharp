// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Configuration;

namespace EdgeSharp.NativeHosts
{
    public partial class WinNativeHost
    {
        public virtual void SetDpiAwarenessContext()
        {
            SetHighDpiMode(_config.WindowOptions.HighDpiMode);
        }

        public virtual void SetHighDpiMode(HighDpiMode dpiAwareness)
        {
            if (!DpiHelper.FirstParkingWindowCreated)
            {
                DpiHelper.SetProcessDpiAwarenessContext(dpiAwareness);
                DpiHelper.FirstParkingWindowCreated = true;
            }
        }
    }
}
