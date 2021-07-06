// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Configuration;
using System;
using static EdgeSharp.Interop.User32;

namespace EdgeSharp.Wpf
{
    public partial class BrowserWindow
    {
        protected virtual IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            WM wmMsg = (WM)msg;
            switch (wmMsg)
            {
                case WM.PARENTNOTIFY:
                    {
                        WM wParmLw = (WM)LOWORD((int)wParam);
                        switch (wParmLw)
                        {
                            case WM.LBUTTONDOWN:
                                if (_config.IsBorderlessWindowDraggable())
                                {
                                    _borderlessController?.InitiateWindowDrag(hWnd, lParam);
                                    handled = true;
                                    return IntPtr.Zero;
                                }
                                break;

                            default:
                                break;
                        }
                    }
                    break;

                case WM.ERASEBKGND:
                    handled = true;
                    return new IntPtr(1);

                default:
                    break;
            }

            return IntPtr.Zero;
        }
    }
}
