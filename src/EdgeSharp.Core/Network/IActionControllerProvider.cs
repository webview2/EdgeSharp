// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

namespace EdgeSharp.Core.Network
{
    /// <summary>
    /// Action controller provider - provides execution of controller actions.
    /// </summary>
    public interface IActionControllerProvider
    {
        /// <summary>
        /// Executes controller action.
        /// </summary>
        /// <param name="request">The <see cref="IActionRequest"/>.</param>
        /// <returns>The <see cref="IActionResponse"/> object.</returns>
        IActionResponse Execute(IActionRequest request);
    }
}
