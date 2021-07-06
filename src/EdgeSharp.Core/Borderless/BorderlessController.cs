// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Configuration;
using System;
using static EdgeSharp.Interop;
using static EdgeSharp.Interop.User32;

namespace EdgeSharp.Core.Borderless
{
    public partial class BorderlessController
    {
        protected BorderlessOption _borderlessOption;
        protected DragWindowInfo _dragWindowInfo;
        protected DragMouseLLHook _dragMouseHook;

        private IntPtr _windowHandle;

        public BorderlessController(IntPtr windowHandle, BorderlessOption borderlessOption)
        {
            _windowHandle = windowHandle;
            _borderlessOption = borderlessOption;
            _dragWindowInfo = new DragWindowInfo(_windowHandle, _borderlessOption);
        }

        public void InitiateWindowDrag(IntPtr hWnd, IntPtr lParam)
        {
            if (_dragWindowInfo != null)
            {
                ReleaseCapture();
                _dragWindowInfo.Reset();

                var dragPoint = new POINT();
                dragPoint.x = PARAM.SignedLOWORD(lParam);
                dragPoint.y = PARAM.SignedHIWORD(lParam);

                var windowTopLeftPoint = new POINT();
                if (_dragWindowInfo.IsCursorInDraggableRegion(ref dragPoint, ref windowTopLeftPoint))
                {
                    SetCapture(hWnd);

                    _dragWindowInfo.DragInitiated = true;
                    _dragWindowInfo.DragPoint = dragPoint;
                    _dragWindowInfo.DragWindowLocation = windowTopLeftPoint;

                    InstallDragMouseHook();
                }
            }
        }

        #region Install/Detach Hooks

        private void InstallDragMouseHook()
        {
            try
            {
                UninstallDragMouseHook();

                _dragMouseHook = new DragMouseLLHook(_dragWindowInfo);
                _dragMouseHook.Install();
                DragMouseLLHook.MouseMoveHandler += MouseLLHook_MouseMoveHandler;
                DragMouseLLHook.LeftButtonUpHandler += MouseLLHook_LeftButtonUpHandler;
            }
            catch
            {
                UninstallDragMouseHook();
            }
        }

        private void UninstallDragMouseHook()
        {
            try
            {
                _dragMouseHook?.Uninstall();
            }
            catch
            {
            }
        }

        private void MouseLLHook_MouseMoveHandler(object sender, MouseMoveEventArgs eventArgs)
        {
            if (_windowHandle != IntPtr.Zero &&
                eventArgs != null &&
                 !eventArgs.DeltaChangeSize.IsEmpty)
            {
                SetWindowPos(_windowHandle, IntPtr.Zero, eventArgs.DeltaChangeSize.Width, eventArgs.DeltaChangeSize.Height, 0, 0,
                                    SWP.NOACTIVATE
                                    | SWP.NOZORDER
                                    | SWP.NOOWNERZORDER
                                    | SWP.NOSIZE);
            }
        }

        private void MouseLLHook_LeftButtonUpHandler(object sender, LeftButtonUpEventArgs eventArgs)
        {
            UninstallDragMouseHook();
            ReleaseCapture();
            _dragWindowInfo.Reset();
        }

        #endregion Install/Detach Hooks
    }
}