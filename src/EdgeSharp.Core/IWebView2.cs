// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;
using System.Drawing;
using System.Threading.Tasks;

namespace EdgeSharp.Core
{
    /// <summary>
    /// Represents the webview - a common way to define all webview controls - Win32, WinForms, Wpf.
    /// </summary>
    public interface IWebView2 : IResourceRequestHandler
    {
        /// <summary>
        /// Gets or sets the Source property.
        /// The Source property is the URI of the top level document of the WebView.
        /// Setting the Source is equivalent to calling <see cref="M:Microsoft.Web.WebView2.Core.CoreWebView2.Navigate(System.String)" WebView2. />.
        /// </summary>
        Uri Source { get; set; }
        /// <summary>
        /// Gets or sets the zoom factor for the WebView.
        /// </summary>
        double ZoomFactor { get; set; }
        /// <summary>
        /// Gets or sets the default background color for the WebView.
        /// </summary>
        Color DefaultBackgroundColor { get; set; }
        /// <summary>
        /// Gets a value indicating whether the webview can navigate to a next page in the navigation history.
        /// </summary>
        bool CanGoForward { get; }
        /// <summary>
        /// Gets a value indicating whether the webview can navigate to a the previous page in the navigation history.
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        ///  Executes the provided script in the top level document of the WebView.
        /// </summary>
        /// <param name="script">The JavaScript to execute.</param>
        /// <returns>The <see cref="Task"/> that return a string.</returns>
        Task<string> ExecuteScriptAsync(string script);
        /// <summary>
        /// Renders the provided HTML as the top level document of the WebView.
        /// </summary>
        /// <param name="htmlContent">The HTML content to navigate to.</param>
        void NavigateToString(string htmlContent);
        /// <summary>
        /// Reloads the top level document of the WebView.
        /// </summary>
        void Reload();
        /// <summary>
        /// Navigates to the next page in navigation history.
        /// </summary>
        void GoForward();
        /// <summary>
        /// Navigates to the previous page in navigation history.
        /// </summary>
        void GoBack();
        /// <summary>
        /// Stops any in progress navigation in the WebView.
        /// </summary>
        void Stop();
    }
}
