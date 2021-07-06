// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;

namespace EdgeSharp.Core.Infrastructure
{
    public abstract class Dispatcher
    {
        private static Dispatcher instance;
        public static Dispatcher Browser
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public abstract void Execute(string actionName);
        public abstract void Post(Action action);
        public abstract void Dispatch();
    }
}

