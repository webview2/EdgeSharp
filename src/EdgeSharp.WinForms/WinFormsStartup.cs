// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core;

namespace EdgeSharp.WinForms
{
    /// <summary>
    /// The default implementation of WinForms <see cref="StartupBase"/> class.
    /// </summary>
    public abstract class WinFormsStartup : StartupBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="WinFormsStartup"/>.
        /// </summary>
        /// <param name="coreServices">WinForms EdgeSahrp core services registration class.</param>
        public WinFormsStartup(ICoreServices coreServices = null)
            : base(coreServices ?? new CoreServices())
        {
        }
    }
}
