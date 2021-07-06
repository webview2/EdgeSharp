// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Network;
using Microsoft.Web.WebView2.Core;
using System;

namespace EdgeSharp.Core
{
    /// <summary>
    /// Primary handler to process resource files, resource url, requests.
    /// </summary>
    public interface IResourceRequestHandler
    {
        /// <summary>
        /// Gets WebView2 core object.
        /// </summary>
        CoreWebView2 CoreWebView2 { get; }
        /// <summary>
        /// Gets or sets WebView2 environment options.
        /// </summary>
        CoreWebView2EnvironmentOptions EnvironmentOptions { get; set; }

        /// <summary>
        /// Initialize method is invoked after WebView2 is created.
        /// </summary>
        /// <param name="coreWebView2">The <see cref="CoreWebView2"/> instance.</param>
        void Initialize(CoreWebView2 coreWebView2);
        /// <summary>
        /// Handle web message received.
        /// This is invoked after CoreWebView2.WebMessageReceived Event received.
        /// </summary>
        /// <param name="eventArgs">The <see cref="CoreWebView2WebMessageReceivedEventArgs"/> instance.</param>
        void HandleWebMessageReceived(CoreWebView2WebMessageReceivedEventArgs eventArgs);
        /// <summary>
        /// Handle resource request.
        /// This is invoked after CoreWebView2.WebResourceRequested Event received.
        /// </summary>
        /// <param name="eventArgs">The <see cref="CoreWebView2WebResourceRequestedEventArgs"/> instance.</param>
        void HandleResourceRequest(CoreWebView2WebResourceRequestedEventArgs eventArgs);
        /// <summary>
        /// It sets no content response to resource/request.
        /// </summary>
        /// <param name="reasonPhrase">Reason for no content.</param>
        /// <param name="deferral">The <see cref="CoreWebView2Deferral"/> instance.</param>
        /// <param name="callback">The callback.</param>
        void SetNoContentResponse(string reasonPhrase, CoreWebView2Deferral deferral, Action<IResponse, CoreWebView2Deferral> callback);
        /// <summary>
        /// Sets resource/request response.
        /// </summary>
        /// <param name="response">The <see cref="IResponse"/> instance.</param>
        /// <param name="deferral">The <see cref="CoreWebView2Deferral"/> instance.</param>
        /// <param name="callback">The callback.</param>
        void SetResponse(IResponse response, CoreWebView2Deferral deferral, Action<IResponse, CoreWebView2Deferral> callback);
        /// <summary>
        /// Adds/register host object to script.
        /// </summary>
        /// <param name="name">Host object name.</param>
        /// <param name="hostObject">The host object.</param>
        void AddHostObject(string name, object hostObject);
        /// <summary>
        /// Remove registered host object from script.
        /// </summary>
        /// <param name="name">Host object name.</param>
        void RemoveHostObject(string name);
        /// <summary>
        /// Remove alls registered host objects from script.
        /// </summary>
        void RemoveAllHostObjects();
        void Dispose();
      }
}
