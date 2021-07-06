// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System.Collections.Generic;

namespace EdgeSharp.Core.Network
{
    public interface IActionRequest
    {
        string RequestId { get; set; }
        string RoutePath { get; set; }
        object Content { get; set; }
        IDictionary<string, string[]> Headers { get; set; }
        IDictionary<string, IList<object>> Parameters { get; set; }
    }
}
