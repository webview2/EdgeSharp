// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Infrastructure;
using Microsoft.Web.WebView2.Core;
using System;

namespace EdgeSharp.Core.Network
{
    /// <summary>
    /// Scheme handler - to handle scheme - http, https etc.
    /// </summary>
    public interface ISchemeHandler
    {
        /// <summary>
        /// Gets the url scheme.
        /// </summary>
        UrlScheme UrlScheme { get; }

        /// <summary>
        /// Process request.
        /// </summary>
        /// <param name="request">The <see cref="IRequest"/> instance.</param>
        /// <param name="deferral">The <see cref="CoreWebView2Deferral"/> instance.</param>
        /// <param name="callback">The callback.</param>
        void ProcessRequest(IRequest request, CoreWebView2Deferral deferral, Action<IResponse, CoreWebView2Deferral> callback);
    }
}
