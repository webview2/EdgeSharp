// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Browser;
using EdgeSharp.Core;
using EdgeSharp.Core.Configuration;
using EdgeSharp.Core.Infrastructure;
using System;

namespace EdgeSharp
{
    public partial class WindowController : IWindowController
    {
        protected readonly IConfiguration _config;
        protected readonly IBrowserWindow _window;
        protected readonly INativeHost _nativeHost;

        private IntPtr _nativeHandle;

        public WindowController(IConfiguration config, IBrowserWindow window, INativeHost nativeHost)
        {
            _config = config;
            _window = window;
            _nativeHost = nativeHost;

            _nativeHost.Created += OnWindowCreated;
            _nativeHost.Moving += OnWindowMoving;
            _nativeHost.SizeChanged += OnWindowSizeChanged;
            _nativeHost.Close += OnWindowClosed;
        }

        public virtual void OnWindowCreated(object sender, CreatedEventArgs createdEventArgs)
        {
            if (createdEventArgs != null)
            {
                _nativeHandle = createdEventArgs.Handle;
                (_window as BrowserWindow)?.Initialize();
                _window.Source = new Uri(_config.StartUrl);
                (_window as BrowserWindow)?.InitCoreWebView2(_nativeHandle);
            }
        }

        public virtual void OnWindowSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if (sizeChangedEventArgs != null)
            {
                (_window as BrowserWindow)?.Resize(sizeChangedEventArgs.Width, sizeChangedEventArgs.Height);
            }
        }

        public virtual void OnWindowClosed(object sender, CloseEventArgs closeEventArgs)
        {
        }

        public virtual void OnWindowMoving(object sender, MovingEventArgs e)
        {
        }

        public virtual int Run(string[] args)
        {
            try
            {
                return RunInternal(args);
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
                return 1;
            }
        }

        protected virtual int RunInternal(string[] args)
        {
            // Create and show window
            _nativeHost?.CreateWindow();

            // Run message loop
            _nativeHost?.Run(); 

            return 0;
        }
    }
}
