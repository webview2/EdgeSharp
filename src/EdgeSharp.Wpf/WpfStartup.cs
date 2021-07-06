// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core;

namespace EdgeSharp.Wpf
{
    /// <summary>
    /// The default implementation of Wpf <see cref="StartupBase"/> class.
    /// </summary>
    public abstract class WpfStartup : StartupBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="WpfStartup"/>.
        /// </summary>
        /// <param name="coreServices">Wpf EdgeSahrp core services registration class.</param>
        public WpfStartup(ICoreServices coreServices = null)
           : base(coreServices ?? new CoreServices())
        {
        }
    }
}
