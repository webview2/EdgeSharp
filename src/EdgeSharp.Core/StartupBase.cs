// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Configuration;
using EdgeSharp.Core.Defaults;
using EdgeSharp.Core.Infrastructure;
using EdgeSharp.Core.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EdgeSharp.Core
{
    /// <summary>
    /// The base (<see cref="IStartup"/>) class.
    /// </summary>
    public abstract class StartupBase : IStartup
    {
        protected ICoreServices _coreServices;
        protected bool _servicesConfigured;
        protected bool _coreServicesConfigured;
        protected bool _servicesInitialized;

        /// <summary>
        /// Initializes a new instance of <see cref="StartupBase"/>.
        /// </summary>
        /// <param name="coreServices">Win32 EdgeSahrp core services registration class.</param>
        public StartupBase(ICoreServices coreServices)
        {
            _coreServices = coreServices;
        }

        /// <inheritdoc />
        public virtual IServiceCollection CreateServiceCollection()
        {
            return new ServiceCollection();
        }

        /// <inheritdoc />
        public virtual void Initialize(IServiceProvider serviceProvider)
        {
            if (!_servicesConfigured || !_coreServicesConfigured)
            {
                var exception = new Exception("Services must be configured before application is initialized.");
                Logger.Instance.Log.LogError(exception);
                throw exception;
            }

            #region Configuration

            var config = serviceProvider.GetService<IConfiguration>();
            if (config == null)
            {
                config = new Defaults.Configuration();
            }

            #endregion Configuration

            #region Application/User Settings

            var appSettings = serviceProvider.GetService<IAppSettings>();
            if (appSettings == null)
            {
                appSettings = new AppSettings();
            }

            var currentAppSettings = new CurrentAppSettings
            {
                Properties = appSettings
            };
            AppUser.App = currentAppSettings;
            AppUser.App.Properties.Read(config);

            #endregion

            #region Logger

            var logger = GetCurrentLogger(serviceProvider);
            if (logger == null)
            {
                logger = new SimpleLogger();
            }

            var defaultLogger = new DefaultLogger();
            defaultLogger.Log = logger;
            Logger.Instance = defaultLogger;

            #endregion

            _servicesInitialized = true;
        }

        /// <inheritdoc />
        public virtual void ConfigureCoreServices(IServiceCollection services)
        {
            if (!_servicesConfigured)
            {
                var exception = new Exception("Services must be configured before unregistered core services are registered.");
                Logger.Instance.Log.LogError(exception);
                throw exception;
            }

            _coreServices?.ConfigureServices(services);

            _coreServicesConfigured = true;
        }

        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {
            _servicesConfigured = true;
        }

        /// <inheritdoc />
        public virtual IServiceProvider BuildServiceProvider(IServiceCollection services)
        {
            return services.BuildServiceProvider();
        }

        /// <inheritdoc />
        public virtual void RegisterActionControllerAssembly(IServiceCollection services, string assemblyFullPath)
        {
            if (string.IsNullOrWhiteSpace(assemblyFullPath))
            {
                return;
            }

            try
            {
                if (File.Exists(assemblyFullPath))
                {
                    var assembly = Assembly.LoadFrom(assemblyFullPath);
                    RegisterActionControllerAssembly(services, assembly);
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }

        /// <inheritdoc />
        public virtual void RegisterActionControllerAssembly(IServiceCollection services, Assembly assembly)
        {
            if (assembly == null)
            {
                return;
            }

            try
            {
                services.RegisterAssembly(assembly, ServiceLifetime.Singleton);
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }

        /// <inheritdoc />
        public virtual void RegisterActionRoutes(IServiceProvider serviceProvider)
        {
            if (!_servicesInitialized)
            {
                throw new Exception("Services must be initialized before controller assemblies are scanned.");
            }

            var routeProvider = serviceProvider.GetService<IActionRouteProvider>();
            if (routeProvider != null)
            {
                var controllers = serviceProvider.GetServices<ActionController>();
                routeProvider.RegisterAllRoutes(controllers?.ToList());
            }
        }

        /// <inheritdoc />
        protected virtual ILogger GetCurrentLogger(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetService<ILogger>();
            if (logger != null)
            {
                return logger;
            }

            var appName = Assembly.GetEntryAssembly()?.GetName().Name;
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            if (loggerFactory != null)
            {
                return loggerFactory.CreateLogger(appName);
            }

            var loggerProvider = serviceProvider.GetService<ILoggerProvider>();
            if (loggerProvider != null)
            {
                return loggerProvider.CreateLogger(appName);
            }

            return null;
        }
    }
}
