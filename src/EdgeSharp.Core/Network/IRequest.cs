// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System.Collections.Generic;

namespace EdgeSharp.Core.Network
{
    public interface IRequest
    {
        object Content { get; }
        string RequestId { get; set; }
        string Method { get; set; }
        string Url { get; set; }
        IDictionary<string, string[]> Headers { get; }
        IDictionary<string, IList<object>> Parameters { get; set; }
    }
}
