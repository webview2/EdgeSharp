// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using EdgeSharp.Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EdgeSharp.Core
{
    /// <summary>
    /// Proxy service provider.
    /// </summary>
    public class ServiceLocator
    {
        private static ServiceLocator instance;

        /// <summary>
        /// Gets or sets the  global current <see cref="ServiceLocator"/> instance.
        /// </summary>
        public static ServiceLocator Current
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance.
        /// </summary>
        public virtual IServiceProvider Provider { get; private set; }

        /// <summary>
        /// Gets list of service objects by service <see cref="Type"/>.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type"/> instance.</param>
        /// <returns>List of objects.</returns>
        public virtual IEnumerable<object> GetInstances(Type serviceType)
        {
            if (Provider == null)
            {
                return null;
            }

            return Provider.GetServices(serviceType);
        }

        /// <summary>
        ///  Gets service object by service <see cref="Type"/>.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns>The object.</returns>
        public virtual object GetInstance(Type serviceType)
        {
            if (Provider == null)
            {
                return null;
            }

            return Provider.GetService(serviceType);
        }

        /// <summary>
        /// Gets list of service objects by service <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="TService">Type of service.</typeparam>
        /// <returns>List of objects.</returns>
        public virtual IEnumerable<TService> GetInstances<TService>()
        {
            if (Provider == null)
            {
                return null;
            }

            return Provider.GetServices<TService>();
        }

        /// <summary>
        /// Gets service object by service <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="TService">Type of service.</typeparam>
        /// <returns>The object</returns>
        public virtual TService GetInstance<TService>()
        {
            if (Provider == null)
            {
                return default(TService);
            }

            return Provider.GetService<TService>();
        }

        /// <summary>
        /// The bootstrap function builds the app and cerates <see cref="ServiceLocator"/> instance.
        /// </summary>
        /// <typeparam name="TApp">Type of <see cref="StartupBase"/>.</typeparam>
        /// <param name="appBuilder">The <see cref="AppBuilder<TStartup>" /> instance.</param>
        public static void Bootstrap<TApp>(AppBuilder<TApp> appBuilder) where TApp : StartupBase
        {
            if (appBuilder == null)
            {
                return;
            }

            if (Current != null)
            {
                Logger.Instance.Log.LogWarning("ServiceLocator can only be created once.");
                return;
            }


            var serviceLocator = new ServiceLocator();
            serviceLocator.Provider = appBuilder.Build();
            appBuilder.Run(serviceLocator.Provider);

            Current = serviceLocator;
        }

        /// <summary>
        /// The function sets a new <see cref="ServiceLocator"/> instance from injected <see cref="IServiceProvider"/> instance.
        /// </summary>
        /// <param name="provider">The <see cref="IServiceProvider"/> instance.</param>
        public static void Bootstrap(IServiceProvider provider)
        {
            if (provider == null)
            {
                return;
            }

            if (Current != null)
            {
                Logger.Instance.Log.LogWarning("ServiceLocator can only be created once.");
                return;
            }

            var serviceLocator = new ServiceLocator
            {
                Provider = provider
            };

            Current = serviceLocator;
        }
    }
}

