// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using EdgeSharp.Browser;
using EdgeSharp.Core.Borderless;
using EdgeSharp.Core.Configuration;
using EdgeSharp.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using static EdgeSharp.Interop;
using static EdgeSharp.Interop.User32;

namespace EdgeSharp.NativeHosts
{
    public partial class WinNativeHost : INativeHost
    {
        [DllImport(Libraries.Kernel32)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport(Libraries.User32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
        public unsafe static extern IntPtr LoadIconW(
         IntPtr hInstance,
         IntPtr lpIconName);

        protected const string EDGESHARP_WINDOW_CLASS = "EdgeShaprWindowClass";
        protected const uint IDI_APPLICATION = 32512;
        protected const int CW_USEDEFAULT = unchecked((int)0x80000000);
        protected const int TRUEVALUE = 1;
        protected const string CMDLINE_RESTORE_ARG = "--restore";

        protected static RECT DefaultBounds => new RECT(CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT);
        public static WinNativeHost NativeInstance { get; set; }
        protected BorderlessController _borderlessController;
        protected IConfiguration _config;
        protected IWindowOptions _windowOptions;
        protected IntPtr _handle;
        protected bool _isInitialized;
        protected IntPtr _consoleParentInstance;
        protected WNDPROC _wndProc;
        protected WindowStylePlacement _windowStylePlacement;

        public event EventHandler<CreatedEventArgs> Created;
        public event EventHandler<MovingEventArgs> Moving;
        public event EventHandler<SizeChangedEventArgs> SizeChanged;
        public event EventHandler<CloseEventArgs> Close;

        public WinNativeHost(IConfiguration config)
        {
            _config = config;
            _windowOptions = config?.WindowOptions == null ? new WindowOptions() : config.WindowOptions;
            _isInitialized = false;
            _handle = IntPtr.Zero;
        }

        public IntPtr Handle => _handle;

        public unsafe virtual void CreateWindow()
        {
            SetCurrentProcessExplicitAppUserModelID(AppId);
            SetDpiAwarenessContext();

            _wndProc = WndProc;
            _consoleParentInstance = GetConsoleWindow();
            _windowOptions.WindowState = (_windowOptions.Fullscreen || _windowOptions.KioskMode) ?  WindowState.Fullscreen : _windowOptions.WindowState;
            _windowStylePlacement = new WindowStylePlacement(_config);

            User32.WNDCLASS wcex = new User32.WNDCLASS();
            wcex.style = User32.CS.HREDRAW | User32.CS.VREDRAW;
            wcex.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(_wndProc);
            wcex.cbClsExtra = 0;
            wcex.cbWndExtra = 0;
            wcex.hIcon = GetIconHandle();
            wcex.hCursor = User32.LoadCursorW(IntPtr.Zero, (IntPtr)CursorResourceId.IDC_ARROW);
            wcex.hbrBackground = Gdi32.GetStockObject(Gdi32.StockObject.WHITE_BRUSH);
            wcex.lpszMenuName = null;
            wcex.hInstance = _consoleParentInstance; 

            fixed (char* c = EDGESHARP_WINDOW_CLASS)
            {
                wcex.lpszClassName = c;
            }

            if (User32.RegisterClassW(ref wcex) == 0)
            {
                Logger.Instance.Log.LogError("EdgeSharp window registration failed");
                return;
            }

            var stylePlacement = GetWindowStylePlacement(_windowOptions.WindowState);

            var hWnd = User32.CreateWindowExW(
                            stylePlacement.ExStyles,
                            EDGESHARP_WINDOW_CLASS,
                            _windowOptions.Title,
                            stylePlacement.Styles,
                            stylePlacement.RECT.left,
                            stylePlacement.RECT.top,
                            stylePlacement.RECT.Width,
                            stylePlacement.RECT.Height,
                            IntPtr.Zero,
                            IntPtr.Zero,
                            _consoleParentInstance,
                            null);

            if (hWnd == IntPtr.Zero)
            {
                Logger.Instance.Log.LogError("EdgeSharp window creation failed");
                return;
            }

            PlaceWindow(hWnd, stylePlacement);
            ShowWindow(hWnd, stylePlacement.ShowCommand);
            UpdateWindow(hWnd);
            RegisterComponents(hWnd);
        }

        public virtual void Run()
        {
            try
            {
                RunMessageLoopInternal();
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }

        public virtual Size GetWindowClientSize()
        {
            return GetClientSize();
        }

        public virtual float GetWindowDpiScale()
        {
            const int StandardDpi = 96;
            float scale = 1;
            var hdc = GetDC(_handle);
            try {
                var dpi = Gdi32.GetDeviceCaps(hdc, Gdi32.DeviceCapability.LOGPIXELSY);
                scale = (float)dpi / StandardDpi;
            }
            finally {
                ReleaseDC(_handle, hdc);
            }
            return scale;
        }

        public virtual void ResizeBrowser(IntPtr browserHande, int width, int height)
        {
            SetWindowPos(browserHande, IntPtr.Zero, 0, 0, width, height, SWP.NOZORDER);
        }

        public virtual WindowState GetWindowState() 
        {
            var placement = new WINDOWPLACEMENT();
            placement.length = (uint)Marshal.SizeOf(placement);
            GetWindowPlacement(_handle, out placement);

            switch (placement.showCmd) {
                case SW.Maximized:
                    return WindowState.Maximize;
                case SW.Minimized:
                    return WindowState.Minimize;
                case SW.Normal:
                    return WindowState.Normal;
            }
            // If unknown
            return WindowState.Normal;
        }


        /// <summary> Sets window state. Maximise / Minimize / Restore. </summary>
        /// <param name="state"> The state to set. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public virtual bool SetWindowState(WindowState state) 
        {
            // Window State should not change for Kiosk mode
            if (this._windowOptions.KioskMode)
            {
                return false;
            }

            BOOL result = BOOL.FALSE;

            switch (state)
            {
                case WindowState.Normal:
                    // Restore the window
                    result = ShowWindow(_handle, SW.RESTORE);
                    break;

                case WindowState.Minimize:
                    // Minimize the window
                    result = ShowWindow(_handle, SW.SHOWMINIMIZED);
                    break;

                case WindowState.Maximize:
                    // Maximize the window
                    result = ShowWindow(_handle, SW.SHOWMAXIMIZED);
                    break;
            }

            return  result == BOOL.TRUE;
        }

        public virtual void Exit()
        {
            if (_handle != IntPtr.Zero)
            {
                try
                {
                    UnregisterComponents();
                    ShowWindow(_handle, SW.HIDE);
                    Task.Run(() =>
                    {
                        uint processId = 0;
                        GetWindowThreadProcessId(_handle, out processId);
                        var process = Process.GetProcessById((int)processId);
                        if (process != null)
                        {
                            process.Kill();
                        }
                    });
                }
                catch {}
            }
        }

        public virtual void SetWindowTitle(string title)
        {
            if (Handle != IntPtr.Zero)
            {
                SetWindowText(Handle, title);
            }
        }

        #region Create Window Protected
        protected virtual Rectangle GetWindowBounds()
        {
            var bounds = _config.BrowserBounds;

            switch (_windowOptions.WindowState)
            {
                case WindowState.Normal:
                    break;

                case WindowState.Maximize:
                case WindowState.Fullscreen:
                    _config.BrowserBounds = WindowHelper.FullScreenBounds(bounds);
                    break;
            }

            RECT rect;
            rect.left = bounds.Left;
            rect.top = bounds.Top;
            rect.right = bounds.Right;
            rect.bottom = bounds.Bottom;

            return rect;
        }
        protected virtual Size GetClientSize()
        {
            var size = new Size();
            if (_handle != IntPtr.Zero)
            {
                RECT rect = new RECT();
                GetClientRect(_handle, ref rect);
                size.Width = rect.Width;
                size.Height = rect.Height;
            }

            return size;
        }

        protected virtual void OnCreated(IntPtr hWnd)
        {
            if (_windowOptions.Borderless)
            {
                var borderlessOption = _windowOptions.BorderlessOption ?? new BorderlessOption();
                _borderlessController = new BorderlessController(_handle, borderlessOption);
            }
        }

        protected virtual void PlaceWindow(IntPtr hWnd, WindowStylePlacement stylePlacement)
        {
            if (_windowOptions.Fullscreen || _windowOptions.KioskMode)
            {
                SetFullscreenScreen(hWnd, (int)stylePlacement.Styles, (int)stylePlacement.ExStyles);
            }
            else
            {
                // Center window if State is Normal
                if (_windowOptions.WindowState == WindowState.Normal && _windowOptions.StartCentered)
                {
                    WindowHelper.CenterWindowToScreen(hWnd);
                }
            }

            WINDOWPLACEMENT wpPrev;
            var placement = GetWindowPlacement(hWnd, out wpPrev);
            if (placement == BOOL.TRUE)
            {
                _windowStylePlacement.WindowPlacement = wpPrev;
            }
        }

        protected virtual void OnSizeChanged(int width, int height)
        {
            var handler = SizeChanged;
            handler?.Invoke(width, new SizeChangedEventArgs(width, height));
        }

        #endregion Create Window Protected

        #region Create Window Private

        protected virtual IntPtr GetIconHandle()
        {
            var hIcon = IconHandler.LoadIconFromFile(_windowOptions.RelativePathToIconFile);
            try
            {
                if (hIcon == null)
                {
                    var assembly = Assembly.GetEntryAssembly();
                    var iconAsResource = assembly?.GetManifestResourceNames()
                        .FirstOrDefault(res => res.EndsWith(_windowOptions.RelativePathToIconFile));
                    if (iconAsResource != null)
                    {
                        using (var resStream = assembly.GetManifestResourceStream(iconAsResource))
                        {
                            using(var fileStream = new FileStream(_windowOptions.RelativePathToIconFile, FileMode.Create))
                            {
                                resStream?.CopyTo(fileStream);
                            }
                        }
                    }
                    hIcon = IconHandler.LoadIconFromFile(_windowOptions.RelativePathToIconFile);
                }
            }
            catch
            {
                // ignore
            }

            return hIcon ?? LoadIconW(IntPtr.Zero, (IntPtr)IDI_APPLICATION);
        }

        protected virtual WindowStylePlacement GetWindowStylePlacement(WindowState state)
        {
            WindowStylePlacement windowStyle = new WindowStylePlacement(_config);

            var styles = WindowStylePlacement.NormalStyles;
            var exStyles = WindowStylePlacement.NormalExStyles;

            if (_windowOptions.NoResize)
            {
                styles = WS.OVERLAPPEDWINDOW & ~WS.THICKFRAME | WS.CLIPCHILDREN | WS.CLIPSIBLINGS;
                styles &= ~WS.MAXIMIZEBOX;
            }

            if (_windowOptions.Borderless)
            {
                styles = WS.POPUPWINDOW | WS.CLIPCHILDREN | WS.CLIPSIBLINGS;
                styles |= WS.SIZEBOX;
            }

            if (_windowOptions.KioskMode || _windowOptions.Fullscreen)
            {
                styles &= ~(WS.CAPTION);
                exStyles &= ~(WS_EX.DLGMODALFRAME | WS_EX.WINDOWEDGE | WS_EX.CLIENTEDGE | WS_EX.STATICEDGE);
                state = WindowState.Fullscreen;
            }

            windowStyle.Styles = styles;
            windowStyle.ExStyles = exStyles;
            windowStyle.RECT = GetWindowBounds();

            switch (state)
            {
                case WindowState.Normal:
                    windowStyle.ShowCommand = SW.SHOWNORMAL;
                    break;

                case WindowState.Maximize:
                    windowStyle.Styles |= WS.MAXIMIZE;
                    windowStyle.ShowCommand = SW.SHOWMAXIMIZED;
                    break;

                case WindowState.Fullscreen:
                    windowStyle.ShowCommand = SW.SHOWMAXIMIZED;
                    break;

                default:
                    break;
            }

            return windowStyle;
        }

        private SW GetShowWindowCommand(WindowState state)
        {
            switch (state)
            {
                case WindowState.Normal:
                    return SW.SHOWNORMAL;

                case WindowState.Maximize:
                case WindowState.Fullscreen:
                    return SW.SHOWMAXIMIZED;
            }

            return SW.SHOWNORMAL;
        }

        #endregion Create Window Private

        #region WndProc

        // An application returns IntPtr.Zero if it processes the message.
        protected virtual IntPtr WndProc(IntPtr hWnd, uint message, IntPtr wParam, IntPtr lParam)
        {
            WM wmMsg = (WM)message;
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
                                    return IntPtr.Zero;
                                }
                                break;

                            default:
                                break;
                        }
                    }
                    break;

                case WM.CREATE:
                    {
                        NativeInstance = this;
                        _handle = hWnd;
                        Dispatcher.Browser = new BrowserDispatcher(_handle);
                        OnCreated(hWnd);
                        var createdEvent = new CreatedEventArgs(_handle, _handle);
                        Created?.Invoke(this, createdEvent);
                        _isInitialized = true;
                        break;
                    }

                case WM.PAINT:
                    {
                        PAINTSTRUCT ps = new PAINTSTRUCT();
                        BeginPaint(hWnd, ref ps);
                        EndPaint(hWnd, ref ps);
                        return IntPtr.Zero;
                    }

                case WM.ERASEBKGND:
                    return new IntPtr(TRUEVALUE);


                case WM.NCHITTEST:
                    var result = DefWindowProcW(hWnd, wmMsg, wParam, lParam);

                    var cursor = new Point();
                    cursor.X = PARAM.SignedLOWORD(lParam);
                    cursor.Y = PARAM.SignedHIWORD(lParam);

                    var winSize = GetClientSize();
                    DragRegion.Width = winSize.Width;
                    DragRegion.Height = winSize.Height;

                    if (DragRegion.TopLeftGrip.Contains(cursor)) return (IntPtr)HT.TOPLEFT;
                    else if (DragRegion.TopRightGrip.Contains(cursor)) return (IntPtr)HT.TOPRIGHT;
                    else if (DragRegion.BottomLeftGrip.Contains(cursor)) return (IntPtr)HT.BOTTOMLEFT;
                    else if (DragRegion.BottomRightGrip.Contains(cursor)) return (IntPtr)HT.BOTTOMRIGHT;
                    else if (DragRegion.TopGrip.Contains(cursor)) return (IntPtr)HT.TOP;
                    else if (DragRegion.LeftGrip.Contains(cursor)) return (IntPtr)HT.LEFT;
                    else if (DragRegion.RightGrip.Contains(cursor)) return(IntPtr)HT.RIGHT;
                    else if (DragRegion.BottomGrip.Contains(cursor)) return (IntPtr)HT.BOTTOM;
                    return result;

                case WM.MOVING:
                case WM.MOVE:
                    {
                        Moving?.Invoke(this, new MovingEventArgs());
                        return IntPtr.Zero;
                    }

                case WM.SIZING:
                case WM.SIZE:
                    {
                        var size = GetClientSize();
                        OnSizeChanged(size.Width, size.Height);
                        break;
                    }
                case WM.DPICHANGED:
                    {
                        if (lParam != IntPtr.Zero)
                        {
                            var winRect = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));
                            SetWindowPos(hWnd, 
                                        IntPtr.Zero,
                                        winRect.left,
                                        winRect.top,
                                        winRect.Width,
                                        winRect.Height,
                                        SWP.NOZORDER | SWP.NOACTIVATE);

                            return IntPtr.Zero;
                        }
                        
                        break;
                    }
                case WM.GETMINMAXINFO:
                    {
                        if (HandleMinMaxSizes(lParam))
                        {
                            return IntPtr.Zero;
                        }
                        break;
                    }

                case WM.CLOSE:
                    {
                        if (_handle != IntPtr.Zero && _isInitialized)
                        {
                            Close?.Invoke(this, new CloseEventArgs());
                        }

                        UnregisterComponents();
                        DestroyWindow(_handle);
                        break;
                    }

                case WM.DESTROY:
                    {
                        PostQuitMessage(0);
                        break;
                    }
                case WM.APP:
                    {
                        Dispatcher.Browser.Dispatch();
                        break;
                    }
                default:
                    break;
            }

            return (DefWindowProcW(hWnd, wmMsg, wParam, lParam));
          
        }

        #region WndProc Methods

        protected virtual void HandleSizeChanged(int width, int height)
        {
            SizeChanged?.Invoke(this, new SizeChangedEventArgs(width, height));
        }

        private unsafe bool HandleMinMaxSizes(IntPtr lParam)
        {
            bool isHandled = false;

            MINMAXINFO* mmi = (MINMAXINFO*)lParam;
            if (!_windowOptions.MinimumSize.IsEmpty)
            {
                mmi->ptMinTrackSize.X = _windowOptions.MinimumSize.Width;
                mmi->ptMinTrackSize.Y = _windowOptions.MinimumSize.Height;
                isHandled = true;
            }

            if (!_windowOptions.MaximumSize.IsEmpty)
            {
                mmi->ptMaxTrackSize.X = _windowOptions.MaximumSize.Width;
                mmi->ptMaxTrackSize.Y = _windowOptions.MaximumSize.Height;
                isHandled = true;
            }

            return isHandled;
        }

        #endregion WndProc Methods
        #endregion WndProc

        #region Message Loop

        protected static void RunMessageLoopInternal()
        {
            MSG msg = new MSG();
            while (GetMessageW(ref msg, IntPtr.Zero, 0, 0) != 0)
            {
                if (msg.message == WM.CLOSE)
                {
                    NativeInstance.UnregisterComponents();
                }

                TranslateMessage(ref msg);
                DispatchMessageW(ref msg);
            }
        }

        private void UnregisterComponents()
        {
            UnregisterComponents(_handle);
        }

        #endregion

        #region Install/Detach Hooks, Hot Keys

        public virtual void RegisterComponents(IntPtr handle)
        {
            try
            { 
            }
            catch
            {
            }
        }

        public virtual void UnregisterComponents(IntPtr hwnd)
        {
            try
            {
            }
            catch { }
        }


        #endregion
    }
}