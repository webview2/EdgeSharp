// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Configuration;
using System;
using System.Drawing;
using System.Linq;
using static EdgeSharp.Interop;
using static EdgeSharp.Interop.User32;

namespace EdgeSharp.Core.Borderless
{
    public class DragWindowInfo
    {
        protected BorderlessOption _borderlessOption;

        public DragWindowInfo(IntPtr parentHandle, BorderlessOption borderlessOption)
        {
            MainWindowHandle = parentHandle;
            _borderlessOption = borderlessOption;
        }

        public IntPtr MainWindowHandle { get; set; }
        public bool DragInitiated { get; set; }
        public POINT DragPoint { get; set; }
        public POINT DragWindowLocation { get; set; }

        public void Reset()
        {
            DragInitiated = false;
            DragPoint = new POINT();
            DragWindowLocation = new POINT();
        }

        public bool IsCursorInDraggableRegion(ref POINT cursorLoc, ref POINT windowTopLeftPoint)
        {
            if (_borderlessOption == null ||
                _borderlessOption.DragZones == null ||
                !_borderlessOption.DragZones.Any())
            {
                return false;
            }

            // Cache location for zone
            var zonePt = new Point(cursorLoc.x, cursorLoc.y);

            var rectCurCursorLoc = new Point(cursorLoc.x, cursorLoc.y);
            ClientToScreen(MainWindowHandle, ref rectCurCursorLoc);
            cursorLoc = new POINT(rectCurCursorLoc.X, rectCurCursorLoc.Y);

            RECT rect = new RECT();
            GetWindowRect(MainWindowHandle, ref rect);

            Rectangle rectangle = rect;

            Point curCursorLoc = new Point(rectCurCursorLoc.X, rectCurCursorLoc.Y);

            // Mouse must be within Window
            if (!rectangle.Contains(curCursorLoc))
            {
                return false;
            }

            foreach (var zone in _borderlessOption.DragZones)
            {
                if (zone.EntireWindow)
                {
                    windowTopLeftPoint = new POINT(rectangle.Left, rectangle.Top);
                    return true;
                }

                var size = GetWindowClientSize();
                var scale = GetWindowDpiScale();
                if (zone.InZone(size, zonePt, scale))
                {
                    windowTopLeftPoint = new POINT(rectangle.Left, rectangle.Top);
                    return true;
                }
            }

            return false;
        }

        private Size GetWindowClientSize()
        {
            var size = new Size();
            if (MainWindowHandle != IntPtr.Zero)
            {
                RECT rect = new RECT();
                GetClientRect(MainWindowHandle, ref rect);
                size.Width = rect.Width;
                size.Height = rect.Height;
            }

            return size;
        }

        private float GetWindowDpiScale()
        {
            const int StandardDpi = 96;
            float scale = 1;
            var hdc = GetDC(MainWindowHandle);
            try
            {
                var dpi = Gdi32.GetDeviceCaps(hdc, Gdi32.DeviceCapability.LOGPIXELSY);
                scale = (float)dpi / StandardDpi;
            }
            finally
            {
                ReleaseDC(MainWindowHandle, hdc);
            }

            return scale;
        }
    }
}