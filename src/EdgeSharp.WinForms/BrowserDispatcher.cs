// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core;
using EdgeSharp.Core.Infrastructure;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Windows.Forms;

namespace EdgeSharp.WinForms
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
            Action action = () =>
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
                            Post(() => _browserWindow.Reload());
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
                            Application.Exit();
                        }

                        break;
                }
            };

            // Invoke
            _webView2.BeginInvoke(action);
        }

        public override void Post(Action action)
        {
            _webView2.BeginInvoke(action);
        }

        public override void Dispatch()
        {
        }

        #region Helper

        public static IBrowserWindow GetParentOfType(Control control)
        {
            if (control?.Parent == null)
                return null;

            if (control.Parent is IBrowserWindow parent)
                return parent;

            return GetParentOfType(control.Parent);
        }

        #endregion
    }
}
