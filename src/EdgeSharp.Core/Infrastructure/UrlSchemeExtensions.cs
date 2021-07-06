// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EdgeSharp.Core.Infrastructure
{
    public static class UrlSchemeExtensions
    {
        public static IEnumerable<UrlScheme> GetSchemes(this IEnumerable<UrlScheme> urlSchemes,  UrlSchemeType type)
        {
            return urlSchemes.Where((x => (x.SchemeType == type)));
        }

        public static UrlScheme GetScheme(this IEnumerable<UrlScheme> urlSchemes, string url)
        {
            if (!IsValid(urlSchemes, url))
            {
                return null;
            }

            return urlSchemes.FirstOrDefault((x => x.IsUrlOfSameScheme(url)));
        }

        public static void AddScheme(this IList<UrlScheme> urlSchemes, UrlScheme scheme)
        {
            if (scheme == null || !scheme.ValidScheme)
            {
                return;
            }

            if (!urlSchemes.Any(x => (x.SchemeType == scheme.SchemeType) &&
                                     x.Scheme.Equals(scheme.Scheme, StringComparison.InvariantCultureIgnoreCase) &&
                                     x.Host.Equals(scheme.Host, StringComparison.InvariantCultureIgnoreCase)))
            {
                urlSchemes.Add(scheme);
            }
        }

        public static bool IsValidSourceUrl(this string currentUrl, string startUrl)
        {
            Uri currentUri;
            try
            {
                currentUri = new Uri(currentUrl);
            }
            catch
            {
                return false;
            }

            try
            {
                Uri startUri = new Uri(startUrl);

                if (currentUri == null)
                {
                    return false;
                }

                // In case of Single Page Application 
                // the current form of the URL may not be same as the start -
                // Like start may end with and extention (.html) while current may not.
                if (startUri != null)
                {
                    var currentFile = currentUri.Authority + currentUri.AbsolutePath;
                    var startFile = startUri.Authority + startUri.AbsolutePath;

                    var currentFileExt = Path.GetExtension(currentFile);
                    var startFileExt = Path.GetExtension(startFile);

                    currentFileExt = string.IsNullOrWhiteSpace(currentFileExt) ? string.Empty : currentFileExt.ToLower();
                    startFileExt = string.IsNullOrWhiteSpace(startFileExt) ? string.Empty : startFileExt.ToLower();

                    return currentFileExt.Equals(startFileExt);
                }
            }
            catch 
            {
                return false;
            }

            return false;
        }

        private static bool IsValid(IEnumerable<object> list, string url)
        {
            return !list.IsNullOrEmpty() && !string.IsNullOrWhiteSpace(url);
        }
    }
}