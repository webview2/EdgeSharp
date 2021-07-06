using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Infrastructure;
using static EdgeSharp.Interop.User32;
using EdgeSharp.Core;

namespace EdgeSharp.Browser
{
    public class BrowserDispatcher : Dispatcher
    {
        protected readonly IntPtr _nativeHostHandle;
        protected ConcurrentQueue<Action> _dispatcherQueue;

        public BrowserDispatcher(IntPtr nativeHostHandle)
        {
            _nativeHostHandle = nativeHostHandle;
            _dispatcherQueue = new ConcurrentQueue<Action>();
        }

        public override void Execute(string actionName)
        {
            actionName = string.IsNullOrWhiteSpace(actionName) ? string.Empty : actionName;
            var browserWindow = ServiceLocator.Current.GetInstance<IBrowserWindow>();
            switch (actionName)
            {
                case DisapatcherExecuteType.Reload:
                    Post(() => browserWindow?.Reload());
                    break;

                case DisapatcherExecuteType.Exit:
                    browserWindow?.Exit();
                    break;
            }
        }

        public override void Post(Action action)
        {
            try
            {
                _dispatcherQueue.Enqueue(action);
                SendMessageW(_nativeHostHandle, WM.APP, IntPtr.Zero, IntPtr.Zero);
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }

        public override void Dispatch()
        {
            try
            {
                Action action;
                while (_dispatcherQueue.TryDequeue(out action))
                {
                    action?.Invoke();
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }
    }
}
