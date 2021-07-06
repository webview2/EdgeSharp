// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;

namespace EdgeSharp.Core.Network
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ActionRouteAttribute : Attribute
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ActionControllerAttribute : Attribute
    {
        public string Name { get; set; }
        public string RoutePath { get; set; }
        public string Description { get; set; }
    }
}