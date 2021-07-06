// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using Chromium.AspNetCore.Bridge;
using EdgeSharp.Core.Infrastructure;
using EdgeSharp.Core.Owin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace EdgeSharp.Core
{
    // Shorthand for Owin pipeline func
    using AppFunc = Func<IDictionary<string, object>, Task>;

    /// <summary>
    /// The Owin App builder class.
    /// </summary>
    /// <typeparam name="TStartup">The type of startup object.</typeparam>
    public class OwinAppBuilder<TStartup> : AppBuilder<TStartup> where TStartup : IStartup
    {
        protected IWebHost _owinHost;
        protected IOwinPipeline _owinPipeline;
        protected IOwinAppStartup _owinApp;
        protected TaskCompletionSource<AppFunc> _tcsAppFunc;

        /// <summary>
        /// Initializes a new instance of <see cref="OwinAppBuilder<TStartup>" />
        /// </summary>
        /// <param name="startup">The startup object.</param>
        public OwinAppBuilder(IStartup startup = null)
            :base(startup)
        {
        }

        /// <summary>
        /// Creates a <see cref="ServiceProvider"/> containing services from the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <returns>The <see cref="ServiceProvider"/> object.</returns>
        public override IServiceProvider Build()
        {
            if (_startup.IsOwinApp())
            {
                _serviceProvider = BuildOwinInternal();
                _startup.Initialize(_serviceProvider);
                _startup.RegisterActionRoutes(_serviceProvider);
            }
            else
            {
                _serviceProvider = base.Build();
            }

            if (_serviceProvider == null)
            {
                throw new Exception($"The service provider is not created.");
            }

            return _serviceProvider;
        }

        /// <summary>
        /// Run the application.
        /// </summary>
        /// <param name="provider">The <see cref="IServiceProvider"/> instance</param>
        public override void Run(IServiceProvider provider)
        {
            try
            {
                if (_tcsAppFunc != null)
                {
                    Task.Run(async () => await _owinHost.RunAsync());
                    _owinPipeline = provider.GetService<IOwinPipeline>() ?? new OwinPipeline();
                    _owinPipeline.AppFunc = _tcsAppFunc.Task.Result;
                    _owinPipeline.ErrorHandlingPath = _owinApp.ErrorHandlingPath;
                    _owinPipeline.ParseRoutes(_serviceProvider);
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
                if (_owinHost != null)
                {
                    Task.Run(async () => await _owinHost.StopAsync());
                }
            }
        }

        /// <summary>
        /// Stops child processes and dispose al created services.
        /// </summary>
        public override void Stop()
        {
            try
            {
                base.Stop();

                if (_owinHost != null)
                {
                    Task.Run(async () => await _owinHost.StopAsync());
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }

        private IServiceProvider BuildOwinInternal()
        {
            _owinApp = _startup as IOwinAppStartup;
            _tcsAppFunc = new TaskCompletionSource<AppFunc>();

            var environment = string.IsNullOrWhiteSpace(_owinApp.Environment) ? "Development" : _owinApp.Environment;
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

            var builder = new WebHostBuilder();

            builder
             .ConfigureAppConfiguration((hostingContext, config) =>
             {
                 _owinApp.Configure(config);
                 _owinApp.Configuration = config.Build();
             })
            .ConfigureServices(services =>
            {
                //  var server = new OwinServer(new CustomOwinFeatureImpl2());
                var server = new OwinServer();
                server.UseOwin(appFunc =>
                {
                    _tcsAppFunc.SetResult(appFunc);
                });

                services.AddSingleton<IOwinAppStartup>(_owinApp);
                services.AddSingleton<IServer>(server);

                _startup.ConfigureServices(services);
                _startup.ConfigureCoreServices(services);
            })
            .Configure(app =>
            {
                var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
                var config = app.ApplicationServices.GetRequiredService<IConfiguration>();
                var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

                _owinApp.Configure(app, env, loggerFactory);

            });

            // Configure builder
            builder.UseStartup<OwinStartup>()
                   // https://github.com/aspnet/Hosting/issues/903
                   // Ignore the startup class assembly as the "entry point" and instead point it to this app
                   .UseSetting(WebHostDefaults.ApplicationKey, Assembly.GetEntryAssembly().FullName);

            _owinApp.Configure(builder);
            _owinHost = builder.Build();
            _owinApp.Configure(_owinHost);

            return _owinHost.Services;
        }
    }
}
