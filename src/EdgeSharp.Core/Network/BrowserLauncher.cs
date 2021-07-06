// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace EdgeSharp.Core.Network
{
    public static class BrowserLauncher
    {
        public static void Open(string url)
        {
            try
            {
                try
                {
                    Process.Start(url);
                }
                catch
                {
                    try
                    {
                        // hack because of this: https://github.com/dotnet/corefx/issues/10361
                        url = url.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                    }
                    catch (Exception exception)
                    {
                        Logger.Instance.Log.LogError(exception);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }
    }
}
