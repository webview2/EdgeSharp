// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;
using System.Text.Json;

namespace EdgeSharp.Core.Network
{
    /// <summary>
    /// Binds request data to controller action arguments.
    /// </summary>
    public interface IActionParameterBinder
    {
        /// <summary>
        /// Binds a request property to controller action argument based on request property/contoller action argument name.
        /// </summary>
        /// <param name="parameterName">The argument name.</param>
        /// <param name="type">The action argument <see cref="Type"/>.</param>
        /// <param name="content">The request json property <see cref="JsonElement"/>.</param>
        /// <returns>The resultant object.</returns>
        object Bind(string parameterName, Type type, JsonElement content);
    }
}
