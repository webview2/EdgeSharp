using EdgeSharp.Core.Configuration;
using EdgeSharp.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace EdgeSharp.Core.Defaults
{
    /// <summary>
    /// The default implementation of <see cref="IConfiguration"/>.
    /// </summary>
    public class Configuration : IConfiguration
    {
        /// <inheritdoc />
        public string StartUrl { get; set; }
        /// <inheritdoc />
        public string SdkVersion { get; set; }
        /// <inheritdoc />
        public string RuntimeVersion { get; set; }
        /// <inheritdoc />
        public bool DebuggingMode { get; set; }
        /// <inheritdoc />
        public IList<UrlScheme> UrlSchemes { get; set; }
        /// <inheritdoc />
        public IDictionary<string, string> CommandLineArgs { get; set; }
        /// <inheritdoc />
        public IList<string> CommandLineOptions { get; set; }
        /// <inheritdoc />
        public Rectangle BrowserBounds { get; set; }
        /// <inheritdoc />
        public WebView2CreationOptions WebView2CreationOptions { get; set; }
        /// <inheritdoc />
        public IWindowOptions WindowOptions { get; set; }
        /// <inheritdoc />
        public IDictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="Configuration"/>.
        /// </summary>
        public Configuration()
        {
            StartUrl = "https://www.bing.com/";
            DebuggingMode = true;

            UrlSchemes = new List<UrlScheme>();
            CommandLineArgs = new Dictionary<string, string>();
            CommandLineOptions = new List<string>();
            ExtensionData = new Dictionary<string, object>();

            var requestScheme           = new UrlScheme("http", "edgesharp.com", null, UrlSchemeType.RouteToAction);
            var aspNetCoreScheme = new UrlScheme("https", "edgesharp.owin.com", UrlSchemeType.Owin);
            var externalBrowserScheme = new UrlScheme("https://github.com/edgesharp/edgesharp", true, UrlSchemeType.ExternalBrowser);

            UrlSchemes.Add(requestScheme);
            UrlSchemes.Add(aspNetCoreScheme);
            UrlSchemes.Add(externalBrowserScheme);

            SdkVersion = VersionInfo.SdkVersion;
            BrowserBounds = new Rectangle(0, 0, 1200, 900);
            WindowOptions = new WindowOptions();
            WebView2CreationOptions = new WebView2CreationOptions();

            CommandLineOptions = new List<string>()
            {
                "disable-web-security"
            };
        }
    }
}