// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace EdgeSharp.Core.Owin
{
    /// <summary>
    /// The actual Owin startup class. 
    /// </summary>
    internal sealed class OwinStartup
    {
        private readonly IOwinAppStartup _owinApp;

        public OwinStartup(IOwinAppStartup owinApp)
        {
            _owinApp = owinApp;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            _owinApp.Configure(app, env, loggerFactory);
        }
    }
}
