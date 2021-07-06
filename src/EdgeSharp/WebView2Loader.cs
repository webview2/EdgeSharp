// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Infrastructure;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace EdgeSharp
{
    public static class WebView2Loader
    {
        private const string WebView2LoaderDll = "WebView2Loader.dll";

        private const string win_x86RuntimeRelativePath = @"runtimes\win-x86\native";
        private const string win_x64RuntimeRelativePath = @"runtimes\win-x64\native";
        private const string win_arm64RuntimeRelativePath = @"runtimes\win-arm64\native";

        // Note: Embedded path may not allow dash (-)
        // So ensure win-x64 => winX64
        private const string win_x86ResourcePath = "EdgeSharp.runtimes.winX86.native";
        private const string win_x64ResourcePath = "EdgeSharp.runtimes.winX64.native";
        private const string win_arm64ResourcePath = "EdgeSharp.runtimes.winArm64.native";

        public static void Load()
        {
            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var webview2LoaderDllFileLocation = Path.Combine(appDirectory, WebView2LoaderDll);
            if (!File.Exists(webview2LoaderDllFileLocation))
            {
                if (!CopyFromRuntimes(webview2LoaderDllFileLocation))
                {
                    LoadFromEmbeddedResources(webview2LoaderDllFileLocation);
                }
            }
        }

        public static bool CopyFromRuntimes(string fileDestinationPath)
        {
            Architecture architecture = RuntimeInformation.ProcessArchitecture;
            var relativePath = win_x64RuntimeRelativePath;

            switch (architecture)
            {
                case Architecture.X86:
                    relativePath = win_x86RuntimeRelativePath;
                    break;
                case Architecture.X64:
                    relativePath = win_x64RuntimeRelativePath;
                    break;
                case Architecture.Arm64:
                    relativePath = win_arm64RuntimeRelativePath;
                    break;
                default:
                    break;
            }

            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var fileSourcePath = Path.Combine(appDirectory, relativePath, WebView2LoaderDll);

            try
            {
                if (File.Exists(fileSourcePath))
                {
                    File.Copy(fileSourcePath, fileDestinationPath, true);
                    return true;
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }

            return false;
        }

        public static void LoadFromEmbeddedResources(string fileDestinationPath)
        {
            var resourcePath = win_x64ResourcePath;

            Architecture architecture = RuntimeInformation.ProcessArchitecture;
            switch (architecture)
            {
                case Architecture.X86:
                    resourcePath = win_x86ResourcePath;
                    break;
                case Architecture.X64:
                    resourcePath = win_x64ResourcePath;
                    break;
                case Architecture.Arm64:
                    resourcePath = win_arm64ResourcePath;
                    break;
                default:
                    break;
            }

            var resourceFilePath = $"{resourcePath}.{WebView2LoaderDll}";

            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceFilePath))
            {
                if (resource != null)
                {
                    using (var file = new FileStream(fileDestinationPath, FileMode.Create, FileAccess.Write))
                    {
                        if (file != null)
                        {
                            resource?.CopyTo(file);
                        }
                    }
                }
            }
        }
    }
}
