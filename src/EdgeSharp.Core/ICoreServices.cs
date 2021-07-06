// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using Microsoft.Extensions.DependencyInjection;

namespace EdgeSharp.Core
{
    /// <summary>
    /// Represents core services.
    /// </summary>
    public interface ICoreServices
    {
        /// <summary>
        /// Adds core services.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        void ConfigureServices(IServiceCollection services);
    }
}
