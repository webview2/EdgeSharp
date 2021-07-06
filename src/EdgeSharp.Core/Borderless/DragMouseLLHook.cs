// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;
using System.Runtime.InteropServices;
using static EdgeSharp.Interop;
using static EdgeSharp.Interop.User32;

namespace EdgeSharp.Core.Borderless
{
    public class DragMouseLLHook : IDisposable
    {
        public Func<HookEventArgs, bool> HookEventHandler;

        protected DragWindowInfo _dragWindowInfo;
        protected IntPtr _hookID = IntPtr.Zero;
        protected HOOKPROC _filterFunc = null;
        protected WH _hookType;

        private bool _disposed = false;

        public static event EventHandler<MouseMoveEventArgs> MouseMoveHandler;

        public static event EventHandler<LeftButtonUpEventArgs> LeftButtonUpHandler;

        public DragMouseLLHook(DragWindowInfo dragWindowInfo, WH hook = WH.MOUSE_LL)
        {
            _dragWindowInfo = dragWindowInfo;
            _hookID = IntPtr.Zero;
            _hookType = hook;
            _filterFunc = new HOOKPROC(this.CoreHookProc);
            HookEventHandler = OnMouseEvent;
        }

        protected IntPtr CoreHookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
            {
                return CallNextHookEx(_hookID, (HC)code, wParam, lParam);
            }

            if (code == (int)HC.ACTION)
            {
                var handler = HookEventHandler;
                HookEventArgs e = new HookEventArgs();
                e.HookCode = code;
                e.wParam = wParam;
                e.lParam = lParam;

                if (handler != null)
                {
                    bool handled = handler.Invoke(e);
                    if (handled)
                    {
                        // Handled
                        return (IntPtr)1;
                    }
                }
            }

            // Yield to the next hook in the chain
            return CallNextHookEx(_hookID, (HC)code, wParam, lParam);
        }

        protected virtual bool OnMouseEvent(HookEventArgs args)
        {
            if (args == null)
            {
                return false;
            }

            WM wParam = (WM)args.wParam.ToInt32();
            switch (wParam)
            {
                case WM.MOUSEMOVE:
                    if (_dragWindowInfo != null && _dragWindowInfo.DragInitiated)
                    {
                        POINT cursorPos = new POINT();
                        if (GetCurrentCursorLocation(args.lParam, ref cursorPos))
                        {
                            var ptDelta = new POINT(cursorPos.x - _dragWindowInfo.DragPoint.x, cursorPos.y - _dragWindowInfo.DragPoint.y);
                            var newLocation = new POINT(_dragWindowInfo.DragWindowLocation.x + ptDelta.x, _dragWindowInfo.DragWindowLocation.y + ptDelta.y);
                            OnMouseMove(newLocation.x, newLocation.y);

                            return false;
                        }
                    }
                    break;

                case WM.LBUTTONUP:
                    if (_dragWindowInfo != null && _dragWindowInfo.DragInitiated)
                    {
                        OnLeftButtonUp();
                    }
                    break;
            }

            return false;
        }

        private bool GetCurrentCursorLocation(IntPtr lParam, ref POINT cursorPos)
        {
            try
            {
                var hookInfo = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                cursorPos = hookInfo.pt;
                return true;
            }
            catch { }

            return false;
        }

        #region

        private static void OnMouseMove(int xDelta, int yDelta)
        {
            var handler = MouseMoveHandler;
            if (handler != null)
            {
                var args = new MouseMoveEventArgs(xDelta, yDelta);
                handler.Invoke(null, args);
            }
        }

        private static void OnLeftButtonUp()
        {
            var handler = LeftButtonUpHandler;
            if (handler != null)
            {
                var args = new LeftButtonUpEventArgs();
                handler.Invoke(null, args);
            }
        }

        #endregion

        public void Install()
        {
            _hookID = SetHook(_hookType, _filterFunc);
        }

        public void Uninstall()
        {
            UnhookWindowsHookEx(_hookID);
            _hookID = IntPtr.Zero;
        }

        public bool IsInstalled
        {
            get { return _hookID != IntPtr.Zero; }
        }

        #region Disposal

        ~DragMouseLLHook()
        {
            Dispose(false);
        }

        protected void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
            }

            if (IsInstalled)
                Uninstall();

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}