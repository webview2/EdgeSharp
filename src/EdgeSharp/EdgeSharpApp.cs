// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core;
using Microsoft.Extensions.DependencyInjection;

namespace EdgeSharp
{
    /// <summary>
    /// Win32 EdgeSahrp basic application (<see cref="IStartup"/>) class.
    /// </summary>
    public abstract class EdgeSharpApp : StartupBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EdgeSharpApp"/>.
        /// </summary>
        /// <param name="coreServices">Win32 EdgeSahrp core services registration class.</param>
        public EdgeSharpApp(ICoreServices coreServices = null)
           : base(coreServices ?? new CoreServices())
        {
        }

        /// <inheritdoc />
        public sealed override void ConfigureCoreServices(IServiceCollection services)
        {
            base.ConfigureCoreServices(services);
        }
    }
}
