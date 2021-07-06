// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;
using Microsoft.Web.WebView2.Core;

namespace EdgeSharp.Browser
{
    public partial class BrowserWindow
    {
        protected virtual void UnregisterEvents()
        {
            BrowserWindow webView2 = this;

            webView2.CoreWebView2.SourceChanged -= new EventHandler<CoreWebView2SourceChangedEventArgs>(webView2.BrowserWindow_SourceChanged);
            webView2.CoreWebView2.WebMessageReceived -= new EventHandler<CoreWebView2WebMessageReceivedEventArgs>(webView2.BrowserWindow_WebMessageReceived);
            webView2.CoreWebView2.WebResourceRequested -= new EventHandler<CoreWebView2WebResourceRequestedEventArgs>(webView2.BrowserWindow_WebResourceRequested);
        }

        #region Disposal

        private bool _disposed;

        ~BrowserWindow()
		{
			Dispose(false);
		}

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            // If there are managed resources
            if (disposing && _isInitialized)
            {
                UnregisterEvents();
                RemoveAllHostObjects();

                this._resourceRequestHandler.Dispose();

                _coreWebView2Controller?.Close();
                _coreWebView2Controller = null;
            }

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
