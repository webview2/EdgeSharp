// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Configuration;
using EdgeSharp.Core.Defaults;
using EdgeSharp.Core.Infrastructure;
using EdgeSharp.Core.Network;
using EdgeSharp.Core.Owin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EdgeSharp.Core
{
    /// <summary>
    /// The base <see cref="ICoreServices"/> class.
    /// </summary>
    public abstract class CoreServicesBase : ICoreServices
    {
        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<IErrorHandler, ErrorHandler>();
            services.TryAddSingleton<IDataTransferOptions, DataTransferOptions>();
            services.TryAddSingleton<IConfiguration, Defaults.Configuration>();
            services.TryAddSingleton<IAppSettings, AppSettings>();
            services.TryAddSingleton<IErrorHandler, ErrorHandler>();

            services.TryAddSingleton<IDataTransferOptions, DataTransferOptions>();
            services.TryAddSingleton<IActionParameterBinder, ActionParameterBinder>();
            services.TryAddSingleton<IScriptExecutor, ScriptExecutor>();
            services.TryAddSingleton<IActionRouteProvider, ActionRouteProvider>();
            services.TryAddSingleton<IHostObjectProvider, HostObjectProvider>();
            services.TryAddSingleton<IActionControllerProvider, ActionControllerProvider>();

            services.TryAddSingleton<IResourceRequestSchemeHandler, ResourceRequestSchemeHandler>();
            services.TryAddSingleton<IRouteToActionSchemeHandler, RouteToActionSchemeHandler>();

            services.TryAddSingleton<IOwinPipeline, OwinPipeline>();

            services.TryAddSingleton<IResourceRequestHandler, ResourceRequestHandler>();
        }
    }
}
