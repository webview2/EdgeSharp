using EdgeSharp.Core.Configuration;
using EdgeSharp.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace EdgeSharp.Tests.EdgeSharpCore.Configuration
{
    public class ConfigurationTests
    {
        [Fact]
        public void ConfigTest()
        {
            // Arrange
            var expected = ExpectedConfig;

            // Act
            var actual = ActualConfig;

            // Assert
            Assert.NotNull(expected);
            Assert.NotNull(actual);
            Assert.NotNull(expected.WebView2CreationOptions);
            Assert.NotNull(actual.WebView2CreationOptions);
            Assert.NotNull(expected.UrlSchemes);
            Assert.NotNull(actual.UrlSchemes);
            Assert.NotNull(expected.CommandLineArgs);
            Assert.NotNull(actual.CommandLineArgs);
            Assert.NotNull(expected.CommandLineOptions);
            Assert.NotNull(actual.CommandLineOptions);
            Assert.NotNull(expected.WindowOptions);
            Assert.NotNull(actual.WindowOptions);
            Assert.NotNull(expected.ExtensionData);
            Assert.NotNull(actual.ExtensionData);

            Assert.Equal(expected.StartUrl, actual.StartUrl);
            Assert.Equal(expected.DebuggingMode, actual.DebuggingMode);
            Assert.Equal(expected.BrowserBounds, actual.BrowserBounds);

            Assert.Equal(expected.UrlSchemes.Count, actual.UrlSchemes.Count);
            Assert.Equal(expected.CommandLineArgs.Count, actual.CommandLineArgs.Count);
            Assert.Equal(expected.CommandLineOptions.Count, actual.CommandLineOptions.Count);
            Assert.Equal(expected.ExtensionData.Count, actual.ExtensionData.Count);
        }

        private IConfiguration ExpectedConfig
        {
            get
            {
                return new Core.Defaults.Configuration();
            }
        }


        private IConfiguration ActualConfig
        {
            get
            {
                return new DefaultConfig();
            }
        }

        private class DefaultConfig : IConfiguration
        {
            public string StartUrl { get; set; }
            public string AppExeLocation { get; set; }
            public string SdkVersion { get; set; }
            public string RuntimeVersion { get; set; }
            public bool DebuggingMode { get; set; }
            public Rectangle BrowserBounds { get; set; }
            public WebView2CreationOptions WebView2CreationOptions { get; set; }
            public IList<UrlScheme> UrlSchemes { get; set; }
            public IDictionary<string, string> CommandLineArgs { get; set; }
            public IList<string> CommandLineOptions { get; set; }
            public IDictionary<string, string> CustomSettings { get; set; }
            public IWindowOptions WindowOptions { get; set; }
            public IDictionary<string, object> ExtensionData { get; set; }

            public DefaultConfig()
            {
                AppExeLocation = AppDomain.CurrentDomain.BaseDirectory;
                StartUrl = "https://www.bing.com/";
                DebuggingMode = true;

                UrlSchemes = new List<UrlScheme>();
                CommandLineArgs = new Dictionary<string, string>();
                CustomSettings = new Dictionary<string, string>();
                CommandLineOptions = new List<string>();
                ExtensionData = new Dictionary<string, object>();

                var requestScheme = new UrlScheme("http", "edgesharp.com", null, UrlSchemeType.RouteToAction);
                var aspNetCoreScheme = new UrlScheme("https", "edgesharp.owin.com", UrlSchemeType.Owin);
                var externalBrowserScheme = new UrlScheme("https://github.com/edgesharp/edgesharp", true, UrlSchemeType.ExternalBrowser);

                UrlSchemes.Add(requestScheme);
                UrlSchemes.Add(aspNetCoreScheme);
                UrlSchemes.Add(externalBrowserScheme);

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
}
