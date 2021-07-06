// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core;
using EdgeSharp.Core.Configuration;
using EdgeSharp.Core.Infrastructure;
using EdgeSharp.Core.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace EdgeSharp
{
    /// <summary>
    /// EdgeSharp Win32 application builder.
    /// </summary>
    public sealed class AppBuilder
    {
        private IStartup _startup;
        private IBrowserWindow _browserWindow;
        private IServiceCollection _serviceCollection;
        private IServiceProvider _serviceProvider;
        private ServiceProviderFactory _serviceProviderFactory;
        private IConfiguration _config;
        private IErrorHandler _errorHandler;
        private Type _useConfigType;
        private Type _useWindowType;
        private Type _useErrorHandlerType;
        private int _stepCompleted;

        /// <summary>
        /// Initializes a new instance of <see cref="AppBuilder"/>.
        /// </summary>
        private AppBuilder()
        {
            _config = null;
            _useConfigType = null;
            _useWindowType = null;
            _useErrorHandlerType = null;
            _stepCompleted = -1;
        }

        /// <summary>
        /// Create the <see cref="AppBuilder"/> instance.
        /// </summary>
        /// <returns>Instance of the <see cref="AppBuilder"/>.</returns>
        public static AppBuilder Create()
        {
            var appBuilder = new AppBuilder();
            return appBuilder;
        }

        /// <summary>
        /// Allows the developer to use an external <see cref="IServiceCollection"/>.
        /// Usually custom <see cref="IServiceCollection"/> is not needed. One is provided for the devloper.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>Instance of the <see cref="AppBuilder"/>.</returns>
        public AppBuilder UseServices(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            return this;
        }

        /// <summary>
        /// Allows the developer to use an external <see cref="ServiceProviderFactory"/>.
        /// </summary>
        /// <param name="serviceProviderFactory">A custom The <see cref="ServiceProviderFactory" /> instance.</param>
        /// <returns>Instance of the <see cref="AppBuilder"/>.</returns>
        public AppBuilder UseServiceProviderFactory(ServiceProviderFactory serviceProviderFactory)
        {
            _serviceProviderFactory = serviceProviderFactory;
            return this;
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
        /// <returns>Instance of the <see cref="AppBuilder"/>.</returns>
        public AppBuilder UseConfig<TService>(IConfiguration config = null) where TService : IConfiguration
        {
            if (config != null)
            {
                _config = config;
            }
            else
            {
                _useConfigType = null;
                typeof(TService).EnsureIsDerivedType(typeof(IConfiguration));
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
        /// <returns>Instance of the <see cref="AppBuilder"/>.</returns>
        public AppBuilder UseWindow<TService>(IBrowserWindow browserWindow = null) where TService : IBrowserWindow
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
        /// <returns>Instance of the <see cref="AppBuilder"/>.</returns>
        public AppBuilder UseApp<TApp>(IStartup app = null) where TApp : IStartup
        {
            _startup = app;
            if (_startup == null)
            {
                typeof(TApp).EnsureIsDerivedType(typeof(IStartup));
                _startup = (TApp)Activator.CreateInstance(typeof(TApp));
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
        /// <returns>Instance of the <see cref="AppBuilder"/>.</returns>
        public AppBuilder UseErrorHandler<TService>(IErrorHandler errorHandler = null) where TService : IErrorHandler
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
        /// <returns>Instance of the <see cref="AppBuilder"/>.</returns>
        public AppBuilder Build()
        {
            if (_stepCompleted != 1)
            {
                throw new Exception("Invalid order: Step 1: UseApp must be completed before Step 2: Build.");
            }

            if (_startup == null)
            {
                throw new Exception($"App {nameof(_startup)} cannot be null.");
            }

            if (_serviceCollection == null)
            {
                _serviceCollection = new ServiceCollection();
            }

            _startup.ConfigureServices(_serviceCollection);

            // This must be done before registering core services
            RegisterUseComponents(_serviceCollection);

            _startup.ConfigureCoreServices(_serviceCollection);

            if (_serviceProviderFactory != null)
            {
                _serviceProvider = _serviceProviderFactory.BuildServiceProvider(_serviceCollection);
            }
            else
            {
                _serviceProvider = _startup.BuildServiceProvider(_serviceCollection);
            }

            _startup.Initialize(_serviceProvider);
            _startup.RegisterActionRoutes(_serviceProvider);

            ServiceLocator.Bootstrap(_serviceProvider);

            _stepCompleted = 2;
            return this;
        }

        /// <summary>
        /// Runs the application after build.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public void Run(string[] args)
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
                    windowController.Run(args);
                }
                catch (Exception exception)
                {
                    Logger.Instance.Log.LogError(exception);
                }
                finally
                {
                    windowController.Dispose();
                    (_serviceProvider as ServiceProvider)?.Dispose();
                }

            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }

        private void RegisterUseComponents(IServiceCollection services)
        {
            #region IConfiguration

            if (_config != null)
            {
                services.TryAddSingleton<IConfiguration>(_config);
            }
            else if (_useConfigType != null)
            {
                services.TryAddSingleton(typeof(IConfiguration), _useConfigType);
            }

            #endregion IEdgeConfiguration

            #region IEdgeWindow

            if (_browserWindow != null)
            {
                services.TryAddSingleton<IBrowserWindow>(_browserWindow);
            }
            else if (_useWindowType != null)
            {
                services.TryAddSingleton(typeof(IBrowserWindow), _useWindowType);
            }

            #endregion IEdgeWindow

            #region IEdgeErrorHandler

            if (_errorHandler != null)
            {
                services.TryAddSingleton<IErrorHandler>(_errorHandler);
            }
            else if (_useErrorHandlerType != null)
            {
                services.TryAddSingleton(typeof(IErrorHandler), _useErrorHandlerType);
            }

            #endregion IEdgeErrorHandler
        }
    }
}