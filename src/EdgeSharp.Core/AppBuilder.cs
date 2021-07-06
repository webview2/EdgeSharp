// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;
using EdgeSharp.Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EdgeSharp.Core
{
    /// <summary>
    /// The App builder class.
    /// </summary>
    /// <typeparam name="TStartup">The type of startup object.</typeparam>
    public class AppBuilder<TStartup> where TStartup : IStartup
    {
        protected readonly IStartup _startup;
        protected IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="AppBuilder<TStartup>" />
        /// </summary>
        /// <param name="startup">The startup object.</param>
        public AppBuilder(IStartup startup = null)
        {
            _startup = startup;
            if (_startup == null)
            {
                typeof(TStartup).EnsureIsDerivedType(typeof(IStartup));
                _startup = (TStartup)Activator.CreateInstance(typeof(TStartup));
            }
        }

        /// <summary>
        /// Creates a <see cref="ServiceProvider"/> containing services from the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <returns>The <see cref="ServiceProvider"/> object.</returns>
        public virtual IServiceProvider Build()
        {
            var serviceCollection = new ServiceCollection();
            _startup.ConfigureServices(serviceCollection);
            _startup.ConfigureCoreServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _startup.Initialize(_serviceProvider);
            _startup.RegisterActionRoutes(_serviceProvider);

            return _serviceProvider;
        }

        /// <summary>
        /// Run the application.
        /// </summary>
        /// <param name="provider">The <see cref="IServiceProvider"/> instance.</param>
        public virtual void Run(IServiceProvider provider)
        {
        }

        /// <summary>
        /// Stops child processes and dispose al created services.
        /// </summary>
        public virtual void Stop()
        {
            (_serviceProvider as ServiceProvider)?.Dispose();
        }
    }
}
