// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core;
using EdgeSharp.Core.Infrastructure;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace EdgeSharp.Wpf
{
    public class BrowserDispatcher : Core.Infrastructure.Dispatcher
    {
        protected readonly WebView2 _webView2;
        protected IBrowserWindow _browserWindow;

        public BrowserDispatcher(WebView2 webView2)
        {
            _webView2 = webView2;
        }

        public override void Execute(string actionName)
        {
            Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(() =>
            {
                if (_browserWindow == null)
                {
                    _browserWindow = GetParentOfType(_webView2);
                }

                actionName = string.IsNullOrWhiteSpace(actionName) ? string.Empty : actionName;
                switch (actionName)
                {
                    case DisapatcherExecuteType.Reload:
                        if (_browserWindow != null)
                        {
                            _browserWindow.Reload();
                        }
                        else
                        {
                            _webView2?.Reload();
                        }
                        break;

                    case DisapatcherExecuteType.Exit:
                        if (_browserWindow != null)
                        {
                            _browserWindow.Exit();
                        }
                        else
                        {
                            Application.Current.Shutdown();
                        }

                        break;
                }
            }));
        }

        public override void Post(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(() =>
            {
                action();
            }));
        }

        public override void Dispatch()
        {
        }

        #region Helper

        public static IBrowserWindow GetParentOfType(DependencyObject control)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(control);
            if (parent == null)
                parent = LogicalTreeHelper.GetParent(control);
            if (parent == null)
                return null;

            if (parent is IBrowserWindow)
                return parent as IBrowserWindow;
            else
                return GetParentOfType(parent);
        }

        #endregion
    }
}
