// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Browser;
using EdgeSharp.Core;
using EdgeSharp.NativeHosts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace EdgeSharp
{
    public sealed class CoreServices : CoreServicesBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<IBrowserWindow, BrowserWindow>();
            services.TryAddSingleton<IWindowController, WindowController>();

            var platform = HostRuntime.Platform;

            switch (platform)
            {
                case Platform.MacOSX:
                    throw new NotSupportedException("No support for MacOS yet.");

                case Platform.Linux:
                    throw new NotSupportedException("No support for Linux yet.");

                case Platform.Windows:
                    services.TryAddSingleton<INativeHost, WinNativeHost>();
                    break;

                default:
                    services.TryAddSingleton<INativeHost, WinNativeHost>();
                    break;
            }

            base.ConfigureServices(services);
        }
    }
}
