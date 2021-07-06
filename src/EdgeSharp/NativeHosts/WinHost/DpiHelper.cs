// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Configuration;
using System;
using static EdgeSharp.Interop;

namespace EdgeSharp.NativeHosts
{
    internal static class DpiHelper
    {
        /// <summary>
        ///  Indicates whether the first (Parking)Window has been created. From that moment on,
        ///  we will not be able nor allowed to change the Process' DpiMode.
        /// </summary>
        internal static bool FirstParkingWindowCreated { get; set; }

        /// <summary>
        ///  Gets the DPI awareness.
        /// </summary>
        /// <returns>The thread's/process' current HighDpi mode</returns>
        internal static HighDpiMode GetHostApplicationDpiAwareness()
        {
            // For Windows 10 RS2 and above
            if (OsVersion.IsWindows10_1607OrGreater)
            {
                IntPtr dpiAwareness = User32.GetThreadDpiAwarenessContext();

                if (User32.AreDpiAwarenessContextsEqual(dpiAwareness, DPI_AWARENESS_CONTEXT.SYSTEM_AWARE))
                {
                    return HighDpiMode.SYSTEM_AWARE;
                }

                if (User32.AreDpiAwarenessContextsEqual(dpiAwareness, DPI_AWARENESS_CONTEXT.UNAWARE))
                {
                    return HighDpiMode.UNAWARE;
                }

                if (User32.AreDpiAwarenessContextsEqual(dpiAwareness, DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2))
                {
                    return HighDpiMode.PER_MONITOR_AWARE2;
                }

                if (User32.AreDpiAwarenessContextsEqual(dpiAwareness, DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE))
                {
                    return HighDpiMode.PER_MONITOR_AWARE;
                }

                if (User32.AreDpiAwarenessContextsEqual(dpiAwareness, DPI_AWARENESS_CONTEXT.UNAWARE_GDISCALED))
                {
                    return HighDpiMode.UNAWARE_GDI_SCALED;
                }
            }
            else if (OsVersion.IsWindows8_1OrGreater)
            {
                SHCore.GetProcessDpiAwareness(IntPtr.Zero, out SHCore.PROCESS_DPI_AWARENESS processDpiAwareness);
                switch (processDpiAwareness)
                {
                    case SHCore.PROCESS_DPI_AWARENESS.UNAWARE:
                        return HighDpiMode.UNAWARE;
                    case SHCore.PROCESS_DPI_AWARENESS.SYSTEM_AWARE:
                        return HighDpiMode.SYSTEM_AWARE;
                    case SHCore.PROCESS_DPI_AWARENESS.PER_MONITOR_AWARE:
                        return HighDpiMode.PER_MONITOR_AWARE;
                }
            }
            else
            {
                // Available on Vista and higher
                return User32.IsProcessDPIAware().IsTrue() ? HighDpiMode.SYSTEM_AWARE : HighDpiMode.UNAWARE;
            }

            // We should never get here, except someone ported this with force to < Windows 7.
            return HighDpiMode.UNAWARE;
        }

        /// <summary>
        ///  Sets the DPI awareness. If not available on the current OS, it falls back to the next possible.
        /// </summary>
        /// <returns>true/false - If the process DPI awareness is successfully set, returns true. Otherwise false.</returns>
        internal static bool SetProcessDpiAwarenessContext(HighDpiMode highDpiMode)
        {
            if (OsVersion.IsWindows10_1703OrGreater)
            {
                // SetProcessIntPtr needs Windows 10 RS2 and above
                IntPtr rs2AndAboveDpiFlag;
                switch (highDpiMode)
                {
                    case HighDpiMode.SYSTEM_AWARE:
                        rs2AndAboveDpiFlag = DPI_AWARENESS_CONTEXT.SYSTEM_AWARE;
                        break;
                    case HighDpiMode.PER_MONITOR_AWARE:
                        rs2AndAboveDpiFlag = DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE;
                        break;
                    case HighDpiMode.PER_MONITOR_AWARE2:
                        // Necessary for RS1, since this SetProcessIntPtr IS available here.
                        rs2AndAboveDpiFlag = User32.IsValidDpiAwarenessContext(DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2).IsTrue() ?
                                             DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2 :
                                             DPI_AWARENESS_CONTEXT.SYSTEM_AWARE;
                        break;
                    case HighDpiMode.UNAWARE_GDI_SCALED:
                        // Let's make sure, we do not try to set a value which has been introduced in later Windows releases.
                        rs2AndAboveDpiFlag = User32.IsValidDpiAwarenessContext(DPI_AWARENESS_CONTEXT.UNAWARE_GDISCALED).IsTrue() ?
                                             DPI_AWARENESS_CONTEXT.UNAWARE_GDISCALED :
                                             DPI_AWARENESS_CONTEXT.UNAWARE;
                        break;
                    default:
                        rs2AndAboveDpiFlag = DPI_AWARENESS_CONTEXT.UNAWARE;
                        break;
                }
                return User32.SetProcessDpiAwarenessContext(rs2AndAboveDpiFlag).IsTrue();
            }
            else if (OsVersion.IsWindows8_1OrGreater)
            {
                // 8.1 introduced SetProcessDpiAwareness
                SHCore.PROCESS_DPI_AWARENESS dpiFlag;
                switch (highDpiMode)
                {
                    case HighDpiMode.UNAWARE:
                    case HighDpiMode.UNAWARE_GDI_SCALED:
                        dpiFlag = SHCore.PROCESS_DPI_AWARENESS.UNAWARE;
                        break;
                    case HighDpiMode.SYSTEM_AWARE:
                        dpiFlag = SHCore.PROCESS_DPI_AWARENESS.SYSTEM_AWARE;
                        break;
                    case HighDpiMode.PER_MONITOR_AWARE:
                    case HighDpiMode.PER_MONITOR_AWARE2:
                        dpiFlag = SHCore.PROCESS_DPI_AWARENESS.PER_MONITOR_AWARE;
                        break;
                    default:
                        dpiFlag = SHCore.PROCESS_DPI_AWARENESS.SYSTEM_AWARE;
                        break;
                }

                return SHCore.SetProcessDpiAwareness(dpiFlag) == HRESULT.S_OK;
            }
            else
            {
                // Vista or higher has SetProcessDPIAware
                SHCore.PROCESS_DPI_AWARENESS dpiFlag = (SHCore.PROCESS_DPI_AWARENESS)(-1);
                switch (highDpiMode)
                {
                    case HighDpiMode.UNAWARE:
                    case HighDpiMode.UNAWARE_GDI_SCALED:
                        // We can return, there is nothing to set if we assume we're already in DpiUnaware.
                        return true;
                    case HighDpiMode.SYSTEM_AWARE:
                    case HighDpiMode.PER_MONITOR_AWARE:
                    case HighDpiMode.PER_MONITOR_AWARE2:
                        dpiFlag = SHCore.PROCESS_DPI_AWARENESS.SYSTEM_AWARE;
                        break;
                }

                if (dpiFlag == SHCore.PROCESS_DPI_AWARENESS.SYSTEM_AWARE)
                {
                    return User32.SetProcessDPIAware().IsTrue();
                }
            }

            return false;
        }
    }
}
