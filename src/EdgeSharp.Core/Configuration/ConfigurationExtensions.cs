// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Infrastructure;
using System;
using System.Collections.Generic;

namespace EdgeSharp.Core.Configuration
{
    public static class ConfigurationExtensions
    {
        private const string STARTS_ARG_ONE_DASH = "-";
        private const string STARTS_ARG_TWO_DASHES = "--";
        private const char ARG_EQUALS = '=';

        public static void AppendArgs(this IConfiguration config, IEnumerable<string> args)
        {
            if (config == null) return;

            if (config.CommandLineArgs == null) config.CommandLineArgs = new Dictionary<string, string>();
            if (config.CommandLineOptions == null) config.CommandLineOptions = new List<string>();

            if (args == null) return;

            foreach (var arg in args)
            {
                if (string.IsNullOrWhiteSpace(arg)) continue;

                string currArg = arg.Trim().ToLower();
                if (currArg.StartsWith(STARTS_ARG_TWO_DASHES))
                {
                    currArg = currArg.Substring(2);
                }
                else if (currArg.StartsWith(STARTS_ARG_ONE_DASH))
                {
                    currArg = currArg.Substring(1);
                }

                var split = currArg.Split(ARG_EQUALS);
                if (split.NotEmpty())
                {
                    if (split.Length > 1)
                    {
                        config.CommandLineArgs[split[0]] = split[1];
                    }
                    else
                    {
                        config.CommandLineOptions.Add(currArg);
                    }
                }
            }
        }

        public static void SetHighDpiMode(this IConfiguration config)
        {
            if (config == null) return;
            if (config.WindowOptions == null) config.WindowOptions = new WindowOptions();

            if (config.CommandLineArgs != null)
            {
                foreach (var arg in config.CommandLineArgs)
                {
                    config.WindowOptions.SetHighDpiMode(arg.Value);
                }
            }

            if (config.CommandLineOptions.NotEmpty())
            {
                foreach (var arg in config.CommandLineOptions)
                {
                    config.WindowOptions.SetHighDpiMode(arg);
                }
            }
        }

        private static void SetHighDpiMode(this IWindowOptions windowOptions, string param)
        {
            if (windowOptions == null) return;
            if (string.IsNullOrEmpty(param)) return;

            if (param.Equals("dpiunaware", StringComparison.InvariantCultureIgnoreCase))
            {
                windowOptions.HighDpiMode = HighDpiMode.UNAWARE;
            }
            else if (param.Equals("dpisystemaware", StringComparison.InvariantCultureIgnoreCase))
            {
                windowOptions.HighDpiMode = HighDpiMode.SYSTEM_AWARE;
            }
            else if (param.Equals("dpipermonitoraware", StringComparison.InvariantCultureIgnoreCase))
            {
                windowOptions.HighDpiMode = HighDpiMode.PER_MONITOR_AWARE;
            }
            else if (param.Equals("dpipermonitorawarev2", StringComparison.InvariantCultureIgnoreCase))
            {
                windowOptions.HighDpiMode = HighDpiMode.PER_MONITOR_AWARE2;
            }
        }

        public static bool IsBorderlessWindowDraggable(this IConfiguration config)
        {
            var windowOptions = config?.WindowOptions ?? new WindowOptions();
            var borderlessOption = windowOptions?.BorderlessOption ?? new BorderlessOption();

            return windowOptions.Borderless && borderlessOption.UseDefaultDragHandler && borderlessOption.Draggable;
        }

        public static int GetBorderlessWindowGripSize(this IConfiguration config)
        {
            var windowOptions = config?.WindowOptions ?? new WindowOptions();
            var borderlessOption = windowOptions?.BorderlessOption ?? new BorderlessOption();

            if (windowOptions.Borderless)
            {
                return !windowOptions.NoResize ? windowOptions.BorderlessOption.Resizer : 0;
            }

            return 0;
        }
    }
}