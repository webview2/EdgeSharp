// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace EdgeSharp.Core.Network
{
    public class RequestData
    {
        public string url { get; set; }
        public string method { get; set; }
        public object content { get; set; }
        public IDictionary<string, string> parameters { get; set; }
        public string rawjson { get; set; }

        public dynamic ToDynamicObject()
        {
            dynamic request = new ExpandoObject();
            request.Url = url;
            request.Content = content;
            if (string.IsNullOrWhiteSpace(method))
            {
                request.Method = method;
            }

            if (parameters != null && parameters.Any())
            {
                request.Parameters = parameters;
            }

            if (string.IsNullOrWhiteSpace(rawjson))
            {
                request.RawJson = rawjson;
            }

            return request;
        }
    }

    public class PathAndQuery
    {
        public string Path { get; set; }
        public IDictionary<string, string> QueryParameters { get; set; }

        public void Parse(string requestUrl)
        {
            var uri = CreateUri(requestUrl);
            Path = uri?.AbsolutePath;
            QueryParameters = GetParameters(uri?.PathAndQuery);
        }

        public static Uri CreateUri(string requestUrl)
        {
            if (string.IsNullOrWhiteSpace(requestUrl))
            {
                return null;
            }

            try
            {
                return new Uri(requestUrl);
            }
            catch { }

            try
            {
                if (!requestUrl.Trim().StartsWith("/"))
                {
                    requestUrl = $"/{requestUrl.Trim()}";
                }

                var dummyScheme = "http";
                var dummyHost = "dummy.com";
                var wellFormedUrl = $"{dummyScheme}://{dummyHost}{requestUrl}";
                return new Uri(wellFormedUrl);
            }
            catch { }

            return null;
        }

        public static IDictionary<string, string> GetParameters(string querypath)
        {
            if (string.IsNullOrWhiteSpace(querypath))
            {
                return null;
            }

            var nameValueCollection = new NameValueCollection();

            string querystring = string.Empty;
            int index = querypath.IndexOf('?');
            if (index > 0)
            {
                querystring = querypath.Substring(querypath.IndexOf('?'));
                nameValueCollection = HttpUtility.ParseQueryString(querystring);
            }

            if (string.IsNullOrEmpty(querystring))
            {
                return null;
            }

            return nameValueCollection.AllKeys.ToDictionary(x => x, x => nameValueCollection[x]);
        }
    }
}
