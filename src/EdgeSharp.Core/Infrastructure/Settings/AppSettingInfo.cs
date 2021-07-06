// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;
using System.IO;

namespace EdgeSharp.Core.Infrastructure
{
    public static class AppSettingInfo
    {
        public static string GetSettingsFilePath(string appName = "edgesharp", bool onSave = false)
        {
            try
            {
                var fileName = $"{appName}_appsettings.config";
                var appSettingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.DoNotVerify), "EdgeSharp");

                if (onSave)
                {
                    Directory.CreateDirectory(appSettingsDir);
                    if (Directory.Exists(appSettingsDir))
                    {
                        return Path.Combine(appSettingsDir, fileName);
                    }
                }
                else
                {
                    return Path.Combine(appSettingsDir, fileName);
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }

            return null;
        }
    }
}
