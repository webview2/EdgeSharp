// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;

namespace EdgeSharp.Core
{
    /// <summary>
    /// Represents window controller.
    /// </summary>
    public interface IWindowController : IDisposable
    {
         /// <summary>
         /// Runs Win32 EdgeSharp application.
         /// </summary>
         /// <param name="args"></param>
         /// <returns>The success or failure code - [0: success; 1: failure].</returns>
         int Run(string[] args);
    }
}
