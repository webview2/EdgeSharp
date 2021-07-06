// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Network;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace EdgeSharp.Core.Owin
{
    /// <summary>
    /// The default implementation of <see cref="IOwinAppStartup"/> class.
    /// </summary>
    public abstract class OwinAppStartup : StartupBase, IOwinAppStartup
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OwinAppStartup"/>
        /// </summary>
        /// <param name="coreServices">Win32 EdgeSahrp core services registration class.</param>
        public OwinAppStartup(ICoreServices coreServices)
            : base(coreServices)
        {
            Environment = "Development";
            ErrorHandlingPath = "/Home/Error";
        }

        /// <inheritdoc />
        public string Environment { get; set; }

        /// <inheritdoc />
        public string ErrorHandlingPath { get; set; }

        /// <inheritdoc />
        public IConfiguration Configuration { get; set; }

        /// <inheritdoc />
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
        }

        /// <inheritdoc />
        public sealed override void ConfigureCoreServices(IServiceCollection services)
        {
            services.TryAddSingleton<IErrorHandler, OwinErrorHandler>();
            services.TryAddSingleton<IOwinSchemeHandler, OwinSchemeHandler>();

            // Ensure that the base method is called last
            base.ConfigureCoreServices(services);
        }

        /// <inheritdoc />
        public abstract void Configure(IConfigurationBuilder configBuilder);
        /// <inheritdoc />
        public abstract void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory);
        /// <inheritdoc />
        public abstract void Configure(IWebHostBuilder builder);
        /// <inheritdoc />
        public abstract void Configure(IWebHost host);
    }
}


