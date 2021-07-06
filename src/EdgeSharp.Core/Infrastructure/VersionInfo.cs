// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;
using System.Diagnostics;
using System.IO;

namespace EdgeSharp.Core.Infrastructure
{
    public static class VersionInfo
    {
        public static string SdkVersion
        {
            get
            {
                try
                {
                    var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    var location = Path.Combine(appDirectory, "Microsoft.Web.WebView2.Core.dll");
                    var fileVersion = FileVersionInfo.GetVersionInfo(location);
                    return fileVersion.FileVersion;
                }
                catch { }

                return "Unknown";
            }
        }
    }
}
