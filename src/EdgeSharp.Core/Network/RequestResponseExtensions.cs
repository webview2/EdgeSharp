// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Infrastructure;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace EdgeSharp.Core.Network
{
    public static class RequestResponseExtensions
    {
        public static bool IsClientErrorCode(this HttpStatusCode statusCode)
        {
            // 4xx client errors
            return (int)statusCode >= ResponseConstants.MinClientErrorStatusCode && 
                   (int)statusCode <= ResponseConstants.MaxClientErrorStatusCode;
        }

        public static bool IsServerErrorCode(this HttpStatusCode statusCode)
        {
            // 5xx server errors
            return (int)statusCode >= ResponseConstants.MinServerErrorStatusCode;
        }

        public static IDictionary<string, string[]> RequestHeadersToDictionary(this CoreWebView2HttpRequestHeaders headers)
        {
            var headerDict = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            foreach (var header in headers)
            {
                headerDict.Add(header.Key, new string[] { header.Value });
            }

            return headerDict;
        }

        public static IDictionary<string, IList<object>> RequestQueryParametersToDictionary(this string url)
        {
            var paramDic = new Dictionary<string, IList<object>>();

            try
            {
                var pathAndQuery = new PathAndQuery();
                pathAndQuery.Parse(url);

                return pathAndQuery.RequestQueryParametersToDictionary();
            }
            catch { }

            return paramDic;
        }

        public static IDictionary<string, IList<object>> RequestQueryParametersToDictionary(this PathAndQuery pathAndQuery)
        {
            var paramDic = new Dictionary<string, IList<object>>();

            try
            {
                if (pathAndQuery.QueryParameters != null && pathAndQuery.QueryParameters.Any())
                {
                    foreach (var item in pathAndQuery.QueryParameters)
                    {
                        if (!paramDic.ContainsKey(item.Key))
                        {
                            var values = item.Value.Split(',');
                            paramDic.Add(item.Key, values);
                        }
                    }
                }
            }
            catch { }

            return paramDic;
        }

        public static string ResponseHeadersToString(this IResponse response)
        {
            if (response == null || response.Headers == null || !response.Headers.Any())
            {
                return string.Empty;
            }

            var headerStringBuilder = new StringBuilder();
            foreach (var header in response.Headers)
            {
                foreach (var item in header.Value)
                {
                    headerStringBuilder.AppendLine(header.Key + ":" + item);
                }
            }

            return headerStringBuilder.ToString();
        }

        public static Stream GetMemoryStream(this string statusText)
        {
            if (string.IsNullOrWhiteSpace(statusText))
            {
                return null;
            }

            var preamble = Encoding.UTF8.GetPreamble();
            var bytes = Encoding.UTF8.GetBytes(statusText);

            var memoryStream = new MemoryStream(preamble.Length + bytes.Length);

            memoryStream.Write(preamble, 0, preamble.Length);
            memoryStream.Write(bytes, 0, bytes.Length);

            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}
