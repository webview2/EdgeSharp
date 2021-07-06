// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Network;
using System;

namespace EdgeSharp.Core.Infrastructure
{
    /// <summary>
    /// The url scheme.
    /// </summary>
    public class UrlScheme
    {
        public UrlScheme(string scheme, string host, UrlSchemeType schemeType)
        {
            Name = Guid.NewGuid().ToString();
            Scheme = scheme;
            Host = host;
            Folder = null;
            BaseUrl = null;
            BaseUrlStrict = false;
            SchemeType = schemeType;
        }

        public UrlScheme(string scheme, string host, string folder, UrlSchemeType schemeType)
        {
            Name = Guid.NewGuid().ToString();
            Scheme = scheme;
            Host = host;
            Folder = folder;
            BaseUrl = null;
            BaseUrlStrict = false;
            SchemeType = schemeType;
        }

        public UrlScheme(string url, UrlSchemeType schemeType)
        {
            Name = Guid.NewGuid().ToString();
            Host = null;
            Folder = null;
            BaseUrl = null;
            BaseUrlStrict = false;
            SchemeType = schemeType;

            if (!string.IsNullOrWhiteSpace(url))
            {
                var uri = new Uri(url);
                Scheme = uri.Scheme;
                Host = uri.Host;
            }
        }

        public UrlScheme(UrlSchemeType schemeType)
            : this(null, schemeType)
        {
        }

        public UrlScheme(string baseUrl, bool baseUrlStrict, UrlSchemeType schemeType)
            : this(baseUrl, schemeType)
        {
         
            BaseUrlStrict = baseUrlStrict;
        }

        public string Key
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Scheme) && 
                    string.IsNullOrWhiteSpace(Host)   &&
                    string.IsNullOrWhiteSpace(Folder))
                {
                    return null;
                }    

                return RouteKeys.CreateSchemeKey(Scheme, Host, Folder);
            }
        }

        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string Folder { get; set; }
        public UrlSchemeType SchemeType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether url must be relative to base.
        /// Only valid for external url.
        /// If base is http://a.com/me/you then 
        /// http://a.com/me/you/they is valid but
        /// http://a.com/me/they is not  valid
        /// </summary>
        public bool BaseUrlStrict { get; set; }
        public bool ValidScheme
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Scheme) && !string.IsNullOrWhiteSpace(Host);
            }
        }

        public bool IsStandardScheme
        {
            get
            {
                if (string.IsNullOrEmpty(Scheme))
                {
                    return false;
                }

                switch (Scheme.ToLower())
                {
                    case "http":
                    case "https":
                    case "file":
                    case "ftp":
                    case "about":
                    case "data":
                        return true;
                }

                return false;
            }
        }

        public bool IsUrlOfSameScheme(string url)
        {
            if (string.IsNullOrWhiteSpace(Scheme) ||
                string.IsNullOrWhiteSpace(Host) ||
                string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            var uri = new Uri(url);

            if (string.IsNullOrWhiteSpace(uri.Scheme) ||
                string.IsNullOrWhiteSpace(uri.Host))
            {
                return false;
            }

            if (IsOfSameScheme(uri.Scheme, uri.Host))
            {
                return IsValidUrl(url);
            }

            return false;
        }

        public bool IsOfSameScheme(UrlScheme urlScheme)
        {
            if (urlScheme == null)
            {
                return false;
            }

            return IsOfSameScheme(urlScheme.Scheme, urlScheme.Host);
        }

        public bool IsOfSameScheme(string scheme, string host)
        {
            if (string.IsNullOrWhiteSpace(scheme) ||
                string.IsNullOrWhiteSpace(host))
            {
                return false;
            }

            if (Scheme.ToLower().Equals(scheme.ToLower()) &&
                  Host.ToLower().Equals(host.ToLower()))
            {
                return true;
            }

            return false;
        }

        private bool IsValidUrl(string url)
        {
            if (BaseUrlStrict &&
                !string.IsNullOrWhiteSpace(BaseUrl) &&
                !string.IsNullOrWhiteSpace(url))
            {
                if (url.StartsWith(BaseUrl, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        public static string CreateKey(string url)
        {
            string scheme = string.Empty;
            string host = string.Empty;

            if (!string.IsNullOrWhiteSpace(url))
            {
                var uri = new Uri(url);
                scheme = uri.Scheme;
                host = uri.Host;
            }

            return RouteKeys.CreateSchemeKey(scheme, host, null);
        }
    }
}