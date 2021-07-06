// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using Microsoft.Extensions.Logging;
using System;

namespace EdgeSharp.Core.Infrastructure
{
    public static class LoggerExtensions
    {
        public static void LogError(this ILogger logger, Exception exception)
        {
            logger.LogError(exception, exception?.Message);
        }
    }
}
