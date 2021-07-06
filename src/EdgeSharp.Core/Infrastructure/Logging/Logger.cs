// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using Microsoft.Extensions.Logging;

namespace EdgeSharp.Core.Infrastructure
{
    public abstract class Logger
    {
        private static Logger instance;
        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    //Ambient Context can't return null, so we assign Local Default
                    instance = new DefaultLogger();
                }

                return instance;
            }
            set
            {
                instance = (value == null) ? new DefaultLogger() : value;
            }
        }

        public virtual ILogger Log { get; set; }
    }
}

