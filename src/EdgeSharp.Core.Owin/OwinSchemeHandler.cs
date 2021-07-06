// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using Chromium.AspNetCore.Bridge;
using EdgeSharp.Core.Infrastructure;
using EdgeSharp.Core.Network;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace EdgeSharp.Core.Owin
{
    /// <summary>
    /// The default implementation of <see cref="IOwinSchemeHandler"/>.
    /// </summary>
    public class OwinSchemeHandler : IOwinSchemeHandler
    {
        protected readonly IOwinPipeline _owinPipeline;
        protected readonly IErrorHandler _errorHandler;

        /// <summary>
        /// Initializes a new instance of <see cref="OwinSchemeHandler"/>
        /// </summary>
        /// <param name="owinPipeline">The <see cref="IOwinPipeline"/> instance.</param>
        /// <param name="errorHandler">The <see cref="IErrorHandler"/> instance.</param>
        public OwinSchemeHandler(IOwinPipeline owinPipeline, IErrorHandler errorHandler)
        {
            _owinPipeline = owinPipeline;
            _errorHandler = errorHandler;
            UrlScheme = new UrlScheme(UrlSchemeType.Owin);
        }

        /// <inheritdoc />
        public UrlScheme UrlScheme { get; }

        /// <inheritdoc />
        public virtual void ProcessRequest(IRequest request, CoreWebView2Deferral deferral, Action<IResponse, CoreWebView2Deferral> callback)
        {
            Task.Run(async () =>
            {
                IResponse response = default(IResponse);

                try
                {
                    var owinRequest = new ResourceRequest(request.Url, request.Method, request.Headers, request.Content as Stream);
                    var owinResponse = await RequestInterceptor.ProcessRequest(_owinPipeline.AppFunc, owinRequest);

                    response = new Response((HttpStatusCode)owinResponse.StatusCode,
                        owinResponse.ReasonPhrase, owinResponse.Headers, owinResponse.Stream);

                    if (response.StatusCode.IsClientErrorCode() || response.StatusCode.IsServerErrorCode())
                    {
                        response = await _errorHandler.HandleErrorAsync(UrlSchemeType.Owin, request, response, null);
                    }
                }
                catch (Exception exception)
                {
                    response = await _errorHandler.HandleErrorAsync(UrlSchemeType.Owin, request, response, exception);
                }
                finally
                {
                    Dispatcher.Browser.Post(() =>
                    {
                        callback.Invoke(response, deferral);
                    });
                }
            });
        }
    }
}