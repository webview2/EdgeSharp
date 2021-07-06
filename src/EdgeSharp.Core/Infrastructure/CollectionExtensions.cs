// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System.Collections.Generic;
using System.Linq;

namespace EdgeSharp.Core.Infrastructure
{
    public static class CollectionExtensions
    {
        public static bool NotEmpty(this IEnumerable<object> list)
        {
            return list != null && list.Any();
        }

        public static bool NotEmpty(this IList<object> list)
        {
            return list != null && list.Any();
        }

        public static bool IsNullOrEmpty(this IEnumerable<object> list)
        {
            return list == null || !list.Any();
        }

        public static bool IsNullOrEmpty(this IList<object> list)
        {
            return list == null || !list.Any();
        }
    }
}
