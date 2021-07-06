// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Borderless;
using EdgeSharp.Core.Configuration;
using System;
using System.Windows.Forms;
using static EdgeSharp.Interop.User32;

namespace EdgeSharp.WinForms
{
    public partial class BrowserForm
    {
        protected override void WndProc(ref Message message)
        {
            WM wmMsg = (WM)message.Msg;
            switch (wmMsg)
            {
                case WM.PARENTNOTIFY:
                    {
                        WM wParmLw = (WM)LOWORD((int)message.WParam);
                        switch (wParmLw)
                        {
                            case WM.LBUTTONDOWN:
                                if (_config.IsBorderlessWindowDraggable())
                                {
                                    _borderlessController?.InitiateWindowDrag(message.HWnd, message.LParam);
                                    message.Result = IntPtr.Zero;
                                    return;
                                }
                                break;

                            default:
                                break;
                        }
                    }
                    break;

                case WM.ERASEBKGND:
                    message.Result = IntPtr.Zero;
                    return;

                case WM.NCHITTEST:
                    base.WndProc(ref message);
                    var cursor = this.PointToClient(Cursor.Position);

                    DragRegion.Width = this.ClientSize.Width;
                    DragRegion.Height = this.ClientSize.Height;

                    if (DragRegion.TopLeftGrip.Contains(cursor)) message.Result = (IntPtr)HT.TOPLEFT;
                    else if (DragRegion.TopRightGrip.Contains(cursor)) message.Result = (IntPtr)HT.TOPRIGHT;
                    else if (DragRegion.BottomLeftGrip.Contains(cursor)) message.Result = (IntPtr)HT.BOTTOMLEFT;
                    else if (DragRegion.BottomRightGrip.Contains(cursor)) message.Result = (IntPtr)HT.BOTTOMRIGHT;
                    else if (DragRegion.TopGrip.Contains(cursor)) message.Result = (IntPtr)HT.TOP;
                    else if (DragRegion.LeftGrip.Contains(cursor)) message.Result = (IntPtr)HT.LEFT;
                    else if (DragRegion.RightGrip.Contains(cursor)) message.Result = (IntPtr)HT.RIGHT;
                    else if (DragRegion.BottomGrip.Contains(cursor)) message.Result = (IntPtr)HT.BOTTOM;
                    return;
            }

            base.WndProc(ref message);
        }
    }
}
