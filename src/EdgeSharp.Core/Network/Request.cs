// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace EdgeSharp.Core.Network
{
    public class Request : IRequest
    {
        public string RequestId { get; set; }
        public string Method { get; set; }
        public string Url { get; set; }
        public object Content { get; set; }
        public IDictionary<string, string[]> Headers { get; set; }
        public IDictionary<string, IList<object>> Parameters { get; set; }

        public Request(string requestID = null)
        {
            RequestId = requestID ?? Guid.NewGuid().ToString();
        }

        public Request(string url, string requestID)
            : this(requestID ?? Guid.NewGuid().ToString())
        {
            Url = url;
            RequestId = requestID ?? Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Resource Request
        /// </summary>
        /// <param name="url"><see cref="Url"/></param>
        /// <param name="method"><see cref="Method"/></param>
        /// <param name="headers"><see cref="Headers"/></param>
        /// <param name="stream"><see cref="Stream"/></param>
        public Request(string url, string method, IDictionary<string, string[]> headers, object content = null)
            : this(url, null)
        {
            Method = method;
            Headers = headers;
            Content = content;
        }

        public static IRequest CreateActionRequest(string url, object content)
        {
            var request = new Request();
            request.Url = url;
            request.Parameters = new Dictionary<string, IList<object>>();
            request.Content = content;

            try
            {
                var pathAndQuery = new PathAndQuery();
                pathAndQuery.Parse(url);
                request.Url = pathAndQuery.Path;
                request.Parameters = pathAndQuery.RequestQueryParametersToDictionary();
            }
            catch { }

            return request;
        }

        private static IDictionary<string, string[]> ToDictionary(IEnumerable<KeyValuePair<string, string>> headers)
        {
            var dict = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            foreach (var header in headers)
            {
                dict.Add(header.Key, new string[] { header.Value });
            }
            return dict;
        }

        private static IDictionary<string, string[]> ToDictionary(NameValueCollection nameValueCollection)
        {
            var dict = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in nameValueCollection.AllKeys)
            {
                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, new string[0]);
                }
                var strings = nameValueCollection.GetValues(key);
                if (strings == null)
                {
                    continue;
                }
                foreach (string value in strings)
                {
                    var values = dict[key].ToList();
                    values.Add(value);
                    dict[key] = values.ToArray();
                }
            }
            return dict;
        }
    }
}
