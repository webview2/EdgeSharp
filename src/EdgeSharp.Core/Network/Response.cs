// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Infrastructure;
using System.Collections.Generic;
using System.Net;

namespace EdgeSharp.Core.Network
{
    public class Response : IResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        public Response()
        {
            Headers = new Dictionary<string, string[]>();
            StatusCode = HttpStatusCode.OK;
            ReasonPhrase = ResponseConstants.StatusOKText;
        }

        /// <summary>
        /// ResourceResponse 
        /// </summary>
        /// <param name="statusCode"><see cref="StatusCode"/></param>
        /// <param name="reasonPhrase"><see cref="ReasonPhrase"/></param>
        /// <param name="headers"><see cref="Headers"/></param>
        /// <param name="stream"><see cref="Stream"/></param>
        public Response(HttpStatusCode statusCode, string reasonPhrase, IDictionary<string, string[]> headers, object content)
        {
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
            Headers = headers;
            Content = content;
        }

        public HttpStatusCode StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public object Content { get; set; }
        public bool HasRouteResponse { get; set; }
        public IDictionary<string, string[]> Headers { get; }
    }
}
