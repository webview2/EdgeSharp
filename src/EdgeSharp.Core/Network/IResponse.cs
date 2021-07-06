// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System.Collections.Generic;
using System.Net;

namespace EdgeSharp.Core.Network
{
    public interface IResponse
    {
        object Content { get; set; }
        IDictionary<string, string[]> Headers { get; }
        HttpStatusCode StatusCode { get; set; }
        string ReasonPhrase { get; set; }
        bool HasRouteResponse { get; set; }
    }
}
