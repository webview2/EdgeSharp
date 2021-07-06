// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Infrastructure;
using System.Collections.Generic;
using System.Drawing;

namespace EdgeSharp.Core.Configuration
{
    /// <summary>
    /// Main EdgeSharp configuration class.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Gets or sets he start resource url.
        /// </summary>
        string StartUrl { get; set; }
        /// <summary>
        ///Gets or sets WebView2 SDK version.
        /// </summary>
        string SdkVersion { get; set; }
        /// <summary>
        /// Gets or sets WebView2 runtime version.
        /// </summary>
        string RuntimeVersion { get; set; }
        /// <summary>
        /// Gets a value indicating whether the application is in debug mode. 
        /// </summary>
        bool DebuggingMode { get; set; }
        /// <summary>
        /// Gets or sets WebView2 control/window size.
        /// </summary>
        Rectangle BrowserBounds { get; set; }
        /// <summary>
        /// Gets or sets WebView2 creation and environment options.
        /// </summary>
        WebView2CreationOptions WebView2CreationOptions { get; set; }
        /// <summary>
        /// Gets or sets the application url schemes.
        /// </summary>
        IList<UrlScheme> UrlSchemes { get; set; }
        /// <summary>
        /// Gets or sets Chromium command line arguments.
        /// </summary>
        /// <remarks>
        /// Note: command line argument is different from option:
        /// Sample argument = "disable-web-security" : "1"
        /// </remarks>
        IDictionary<string, string> CommandLineArgs { get; set; }
        /// <summary>
        /// Gets or sets Chromium command line options.
        /// </summary>
        /// <remarks>
        /// Note: command line option is different from argument:
        /// Sample option = --disable-web-security
        /// </remarks>
        IList<string> CommandLineOptions { get; set; }
        /// <summary>
        /// Gets or sets window options.
        /// </summary>
        IWindowOptions WindowOptions { get; set; }
        /// <summary>
        /// Gets or sets place holder for extra parameters/configuration as may be needed by the developer.
        /// </summary>
        IDictionary<string, object> ExtensionData { get; set; }
    }
}