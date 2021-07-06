// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using Microsoft.Extensions.Logging;

namespace EdgeSharp.Core.Infrastructure
{
    public class DefaultLogger : Logger
    {
        private ILogger _logger;

        public override ILogger Log
        {
            get 
            { 
                if (_logger == null)
                {
                    _logger = new SimpleLogger();
                }

                return _logger; 
            }
            set
            {
                _logger = value;
            }
        }
    }
}
