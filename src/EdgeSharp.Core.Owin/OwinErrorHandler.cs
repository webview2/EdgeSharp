// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using Chromium.AspNetCore.Bridge;
using EdgeSharp.Core.Configuration;
using EdgeSharp.Core.Defaults;
using EdgeSharp.Core.Infrastructure;
using EdgeSharp.Core.Network;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace EdgeSharp.Core.Owin
{
    /// <summary>
    /// The default implementation of Owin <see cref="ErrorHandler"/>.
    /// </summary>
    public class OwinErrorHandler : ErrorHandler
    {
        protected readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of <see cref="OwinErrorHandler"/>.
        /// </summary>
        /// <param name="config">EdgeSharp <see cref="IConfiguration"/> instance.</param>
        /// <param name="owinPipeline">The <see cref="IOwinPipeline"/> instance.</param>
        public OwinErrorHandler(IConfiguration config, IOwinPipeline owinPipeline)
            : base(owinPipeline)
        {
            _config = config;
        }

        /// <inheritdoc />
        public async override Task<IResponse> HandleErrorAsync(UrlSchemeType type, IRequest request, IResponse response, Exception exception)
        {
            if (exception != null)
            {
                Logger.Instance.Log.LogError(exception);
            }

            if (_owinPipeline.IsUrlActionRoute(request.Url) && !_owinPipeline.IsUrlErrorHandlingPath(request.Url))
            {
                string errorPageUrl = GetErrorPageUrl(request.Url, _config.StartUrl);
                var newOwinRequest = new ResourceRequest(errorPageUrl, "GET", new Dictionary<string, string[]>(), null);
                var owinResponse = await RequestInterceptor.ProcessRequest(_owinPipeline.AppFunc, newOwinRequest);

                return new Response((HttpStatusCode)owinResponse.StatusCode,
                            owinResponse.ReasonPhrase, owinResponse.Headers, owinResponse.Stream);
            }

            return response;
        }

        private string GetErrorPageUrl(string url, string startUrl)
        {
            var refererUri = CreateUri(url, startUrl);
            return $"{refererUri?.Scheme}{Uri.SchemeDelimiter}{refererUri?.Host}{refererUri?.Port}{_owinPipeline.ErrorHandlingPath}";
        }

        private Uri CreateUri(string url, string startUrl)
        {
            try
            {
                return new Uri(url);
            }
            catch { }

            try
            {
                return new Uri(startUrl);
            }
            catch { }

            return null;
        }
    }
}
