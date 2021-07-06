// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;

namespace EdgeSharp.Core
{
    /// <summary>
    /// Creates remote host objected to be added to script.
    /// </summary>
    public interface IHostObjectProvider : IDisposable
    {
        /// <summary>
        /// Gets the host object name.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets the host object.
        /// </summary>
        object HostObject { get; }
    }
}
