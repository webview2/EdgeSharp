// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Configuration;
using System;
using System.Drawing;

namespace EdgeSharp
{
    public interface INativeHost : IDisposable
    {
        event EventHandler<CreatedEventArgs> Created;
        event EventHandler<MovingEventArgs> Moving;
        event EventHandler<SizeChangedEventArgs> SizeChanged;
        event EventHandler<CloseEventArgs> Close;
        IntPtr Handle { get; }
        void CreateWindow();
        void SetDpiAwarenessContext();
        void SetHighDpiMode(HighDpiMode dpiAwareness);
        void Run();
        Size GetWindowClientSize();
        float GetWindowDpiScale();
        void ResizeBrowser(IntPtr browserWindow, int width, int height);
        void Exit();
        void SetWindowTitle(string title);

        void RegisterComponents(IntPtr hwnd);
        void UnregisterComponents(IntPtr hwnd);

        /// <summary> Gets the current window state Maximised / Normal / Minimised etc. </summary>
        /// <returns> The window state. </returns>
        WindowState GetWindowState();

        /// <summary> Sets window state. Maximise / Minimize / Restore. </summary>
        /// <param name="state"> The state to set. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        bool SetWindowState(WindowState state);

        void ToggleFullscreen(IntPtr hWnd);
    }
}
