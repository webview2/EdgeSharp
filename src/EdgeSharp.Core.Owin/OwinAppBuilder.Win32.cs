// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using Chromium.AspNetCore.Bridge;
using EdgeSharp.Core.Infrastructure;
using EdgeSharp.Core.Network;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace EdgeSharp.Core.Owin
{
    // Shorthand for Owin pipeline func
    using AppFunc = Func<IDictionary<string, object>, Task>;

    /// <summary>
    /// Owin EdgeSharp Win32 application builder.
    /// </summary>
    public sealed class OwinAppBuilder
    {
        private static IWebHost _owinHost;
        private IOwinPipeline _owinPipeline;
        private IOwinAppStartup _owinApp;
        private TaskCompletionSource<AppFunc> _tcsAppFunc;

        private string[] _args;
        private IServiceCollection _serviceCollection;
        private IServiceProvider _serviceProvider;
        private StartupBase _appBase;
        private Core.Configuration.IConfiguration _config;
        private IBrowserWindow _browserWindow;
        private IErrorHandler _errorHandler;
        private Type _useConfigType;
        private Type _useWindowType;
        private Type _useErrorHandlerType;
        private int _stepCompleted;

        /// <summary>
        /// Initializes a new instance of <see cref="OwinAppBuilder"/>.
        /// </summary>
        private OwinAppBuilder(string[] args)
        {
            _args = args;
            _owinPipeline = null;
            _config = null;
            _useConfigType = null;
            _useWindowType = null;
            _useErrorHandlerType = null;
            _stepCompleted = -1;
        }

        /// <summary>
        /// Create the <see cref="OwinAppBuilder"/> instance.
        /// </summary>
        /// <returns>Instance of the <see cref="OwinAppBuilder"/>.</returns>
        public static OwinAppBuilder Create(string[] args)
        {
            var appBuilder = new OwinAppBuilder(args);
            return appBuilder;
        }

        /// <summary>
        /// Allows the developer to use an external EdgeSahrp <see cref="IConfiguration"/>.
        /// </summary>
        /// <remarks>
        /// If an instance of <see cref="IConfiguration" /> is provided, that is what is used.
        /// If the instance of <see cref="IConfiguration" /> is not provided, the of TServive is used to create one.
        /// </remarks>
        /// <typeparam name="TService">A derived type of <see cref="IConfiguration" /> definition. </typeparam>
        /// <param name="config">The <see cref="IConfiguration" /> instance.</param>
        /// <returns>Instance of the <see cref="OwinAppBuilder"/>.</returns>
        public OwinAppBuilder UseConfig<TService>(Core.Configuration.IConfiguration config = null) where TService : Core.Configuration.IConfiguration
        {
            if (config != null)
            {
                _config = config;
            }
            else
            {
                _useConfigType = null;
                typeof(TService).EnsureIsDerivedType(typeof(Core.Configuration.IConfiguration));
                _useConfigType = typeof(TService);
            }

            return this;
        }

        /// <summary>
        /// Allows the developer to use an external <see cref="IBrowserWindow"/>.
        /// </summary>
        /// <remarks>
        /// If an instance of <see cref="IBrowserWindow" /> is provided, that is what is used.
        /// If the instance of <see cref="IBrowserWindow" /> is not provided, the of TServive is used to create one.
        /// </remarks>
        /// <typeparam name="TService">A derived type of <see cref="IBrowserWindow" /> definition. </typeparam>
        /// <param name="browserWindow">The <see cref="IBrowserWindow" /> instance.</param>
        /// <returns>Instance of the <see cref="OwinAppBuilder"/>.</returns>
        public OwinAppBuilder UseWindow<TService>(IBrowserWindow browserWindow = null) where TService : IBrowserWindow
        {
            if (browserWindow != null)
            {
                _browserWindow = browserWindow;
            }
            else
            {
                _useWindowType = null;
                typeof(TService).EnsureIsDerivedType(typeof(IBrowserWindow));
                _useWindowType = typeof(TService);
            }

            return this;
        }

        /// <summary>
        /// Allows the developer to use an external <see cref="IStartup"/>.
        /// </summary>
        /// <remarks>
        /// If an instance of <see cref="IStartup" /> is provided, that is what is used.
        /// If the instance of <see cref="IStartup" /> is not provided, the of TApp is used to create one.
        /// </remarks>
        /// <typeparam name="TApp">A derived type of <see cref="IStartup" /> definition. </typeparam>
        /// <param name="app">The <see cref="IStartup" /> instance.</param>
        /// <returns>Instance of the <see cref="OwinAppBuilder"/>.</returns>
        public OwinAppBuilder UseApp<TApp>(OwinAppStartup app = null) where TApp : OwinAppStartup
        {
            _appBase = app;
            if (_appBase == null)
            {
                typeof(TApp).EnsureIsDerivedType(typeof(OwinAppStartup));
                _appBase = (TApp)Activator.CreateInstance(typeof(TApp));
            }

            _stepCompleted = 1;
            return this;
        }

        /// <summary>
        /// Allows the developer to use an external <see cref="IErrorHandler"/>.
        /// </summary>
        /// <remarks>
        /// If an instance of <see cref="IErrorHandler" /> is provided, that is what is used.
        /// If the instance of <see cref="IErrorHandler" /> is not provided, the of TServive is used to create one.
        /// </remarks>
        /// <typeparam name="TService">A derived type of <see cref="IErrorHandler" /> definition. </typeparam>
        /// <param name="errorHandler">The <see cref="IErrorHandler" /> instance.</param>
        /// <returns>Instance of the <see cref="OwinAppBuilder"/>.</returns>
        public OwinAppBuilder UseErrorHandler<TService>(IErrorHandler errorHandler = null) where TService : IErrorHandler
        {
            if (errorHandler != null)
            {
                _errorHandler = errorHandler;
            }
            else
            {
                _useErrorHandlerType = null;
                typeof(TService).EnsureIsDerivedType(typeof(IErrorHandler));
                _useErrorHandlerType = typeof(TService);
            }

            return this;
        }

        /// <summary>
        /// Builds and create the services.
        /// </summary>
        /// <returns>Instance of the <see cref="OwinAppBuilder"/>.</returns>
        public OwinAppBuilder Build()
        {
            if (_stepCompleted != 1)
            {
                throw new Exception("Invalid order: Step 1: UseApp must be completed before Step 2: Build.");
            }

            if (_appBase == null)
            {
                throw new Exception($"Application {nameof(StartupBase)} cannot be null.");
            }

            if (_appBase.IsOwinApp())
            {
                BuildOwinInternal();
            }
            else
            {
                BuildInternal();
            }

            if (_serviceProvider == null)
            {
                throw new Exception($"The service provider is not created.");
            }

            _appBase.Initialize(_serviceProvider);
            _appBase.RegisterActionRoutes(_serviceProvider);

            ServiceLocator.Bootstrap(_serviceProvider);

            _stepCompleted = 2;
            return this;
        }

        /// <summary>
        /// Runs the application after build.
        /// </summary>
        public void Run()
        {
            if (_stepCompleted != 2)
            {
                throw new Exception("Invalid order: Step 2: Build must be completed before Step 3: Run.");
            }

            if (_serviceProvider == null)
            {
                throw new Exception("ServiceProvider is not initialized.");
            }

            try
            {
                var appName = Assembly.GetEntryAssembly()?.GetName().Name;
                var windowController = _serviceProvider.GetService<IWindowController>();
                try
                {

                    if (_tcsAppFunc != null)
                    {
                        Task.Run(async () => await _owinHost.RunAsync());
                        _owinPipeline = _serviceProvider.GetService<IOwinPipeline>() ?? new OwinPipeline();
                        _owinPipeline.AppFunc = _tcsAppFunc.Task.Result;
                        _owinPipeline.ErrorHandlingPath = _owinApp.ErrorHandlingPath;
                        _owinPipeline.ParseRoutes(_serviceProvider);
                    }

                    windowController.Run(_args);
                }
                catch (Exception exception)
                {
                    Logger.Instance.Log.LogError(exception, $"Error running application:{appName}.");
                }
                finally
                {
                    windowController.Dispose();
                    (_serviceProvider as ServiceProvider)?.Dispose();

                    if (_owinHost != null)
                    {
                        Task.Run(async () => await _owinHost.StopAsync());
                    }
                }

            }
            catch (Exception exception)
            {
                var appName = Assembly.GetEntryAssembly()?.GetName().Name;
                Logger.Instance.Log.LogError(exception, $"Error running application:{appName}.");
            }
        }

        #region Build

        private void BuildOwinInternal()
        {
            _owinApp = _appBase as IOwinAppStartup;
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

                _appBase.ConfigureServices(services);

                // This must be done before registering core services
                RegisterUseComponents(services);

                _appBase.ConfigureCoreServices(services);
            })
            .Configure(app =>
            {
                var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
                var config = app.ApplicationServices.GetRequiredService<IConfiguration>();
                var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

                // Configure App, Env, LoggerFactory
                _owinApp.Configure(app, env, loggerFactory);

            });

            // Configure builder
            builder.UseStartup<OwinStartup>()
                   // https://github.com/aspnet/Hosting/issues/903
                   // Ignore the startup class assembly as the "entry point" and instead point it to this app
                   .UseSetting(WebHostDefaults.ApplicationKey, Assembly.GetEntryAssembly().FullName);

            _owinApp.Configure(builder);

            _owinHost = builder.Build();

            // Configure host
            _owinApp.Configure(_owinHost);

            _serviceProvider = _owinHost.Services;
        }

        public void BuildInternal()
        {
            if (_serviceCollection == null)
            {
                _serviceCollection = new ServiceCollection();
            }

            _appBase.ConfigureServices(_serviceCollection);

            // This must be done before registering core services
            RegisterUseComponents(_serviceCollection);

            _appBase.ConfigureCoreServices(_serviceCollection);
            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        #endregion


        private void RegisterUseComponents(IServiceCollection services)
        {
            #region IConfiguration

            if (_config != null)
            {
                services.TryAddSingleton<Core.Configuration.IConfiguration>(_config);
            }
            else if (_useConfigType != null)
            {
                services.TryAddSingleton(typeof(Core.Configuration.IConfiguration), _useConfigType);
            }

            #endregion IConfiguration

            #region IBrowserWindow

            if (_browserWindow != null)
            {
                services.TryAddSingleton<IBrowserWindow>(_browserWindow);
            }
            else if (_useWindowType != null)
            {
                services.TryAddSingleton(typeof(IBrowserWindow), _useWindowType);
            }

            #endregion IBrowserWindow

            #region IErrorHandler

            if (_errorHandler != null)
            {
                services.TryAddSingleton<IErrorHandler>(_errorHandler);
            }
            else if (_useErrorHandlerType != null)
            {
                services.TryAddSingleton(typeof(IErrorHandler), _useErrorHandlerType);
            }

            #endregion IErrorHandler
        }
    }
}