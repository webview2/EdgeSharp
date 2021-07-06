// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using EdgeSharp.Core.Network;

namespace EdgeSharp.Core
{
    /// <summary>
    /// Represents the startup.
    /// </summary>
    public interface IStartup
    {
        /// <summary>
        /// This is used to create custom <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>
        /// Note: this is redundant for Owin based applications.
        /// </remarks>
        /// <returns></returns>
        IServiceCollection CreateServiceCollection();
        /// <summary>
        /// Creates/initializes common infrastructure objects [Configuration, Logging, AppSetting] that are not previously added to <see cref="IServiceCollection" />.
        /// <remarks>
        /// Note: the objects [Configuration, Logging, AppSetting] will only be created/initialized if not added in:
        ///   - ConfigureCoreServices
        ///   - CoreServices
        ///   - ConfigureServices
        /// </remarks>
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> for application services.</param>
        void Initialize(IServiceProvider serviceProvider);
        /// <summary>
        /// For adding Win32, WinForms, WPF specific core services to the application dependency injection container.
        /// </summary>
        /// <remarks>
        /// Note: services are added here if they were not previously added in ConfigureServices.
        /// This is why it uses for instance "TryAddSingleton".
        /// </remarks>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        void ConfigureCoreServices(IServiceCollection services);
        /// <summary>
        /// The primary way to add services. All default handlers should be overriden here using custom handlers.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        void ConfigureServices(IServiceCollection services);
        /// <summary>
        /// Creates a <see cref="ServiceProvider"/> containing services from the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>
        /// Note: 
        ///   1. This where external dependency providers can be used to create<see cref="ServiceProvider"/>.
        ///   2. This is redundant for Owin based applications.
        /// </remarks>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>The <see cref="ServiceProvider"/> object.</returns>
        IServiceProvider BuildServiceProvider(IServiceCollection services);
        /// <summary>
        /// To register custom <see cref="ActionController"/> instance using the assembly fullpath.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="assemblyFullPath"></param>
        void RegisterActionControllerAssembly(IServiceCollection services, string assemblyFullPath);
        /// <summary>
        /// To register custom <see cref="ActionController"/> instance using the assembly binary.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="assembly"></param>
        void RegisterActionControllerAssembly(IServiceCollection services, Assembly assembly);
        /// <summary>
        /// To register all controller actions of <see cref="ActionController"/> previously registered in RegisterActionControllerAssembly.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> for application services.</param>
        void RegisterActionRoutes(IServiceProvider serviceProvider);
    }
}
