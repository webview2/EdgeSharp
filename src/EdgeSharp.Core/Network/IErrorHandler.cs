// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Infrastructure;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EdgeSharp.Core.Network
{
    /// <summary>
    /// Error/exception handling functionalities
    /// </summary>
    public interface IErrorHandler
    {
        /// <summary>
        /// Handle no route found.
        /// </summary>
        /// <param name="routePath">The route path.</param>
        /// <returns>The <see cref="IActionResponse"/>.</returns>
        IActionResponse HandleRouteNotFound(string routePath);
        /// <summary>
        /// Handle error or execption.
        /// </summary>
        /// <param name="type">The <see cref="UrlSchemeType"/> instance.</param>
        /// <param name="fileInfo">The <see cref="FileInfo"/> instance.</param>
        /// <param name="fileStream">The <see cref="Stream"/> instance.</param>
        /// <param name="exception">The <see cref="Exception"/> instance.</param>
        /// <returns>The <see cref="IResponse"/> object.</returns>
        IResponse HandleError(UrlSchemeType type, FileInfo fileInfo, Stream fileStream, Exception exception);
        /// <summary>
        /// Handle error or execption.
        /// </summary>
        /// <param name="type">The <see cref="UrlSchemeType"/> instance.</param>
        /// <param name="request">The <see cref="IRequest"/> instance.</param>
        /// <param name="response">The <see cref="IResponse"/> instance.</param>
        /// <param name="exception">The <see cref="Exception"/> instance.</param>
        /// <returns>The <see cref="IResponse"/> object.</returns>
        IResponse HandleError(UrlSchemeType type, IRequest request, IResponse response, Exception exception);
        /// <summary>
        /// Handle error or execption.
        /// </summary>
        /// <param name="type">The <see cref="UrlSchemeType"/> instance.</param>
        /// <param name="request">The <see cref="IActionRequest"/> instance.</param>
        /// <param name="exception">The <see cref="Exception"/> instance.</param>
        /// <returns>The <see cref="IActionResponse"/> instance.</returns>
        IActionResponse HandleError(UrlSchemeType type, IActionRequest request, Exception exception);
        /// <summary>
        /// Handle no route found asynchronously.
        /// </summary>
        /// <param name="routePath">The route path.</param>
        /// <returns>The <see cref="Task"/> that returns <see cref="IActionResponse"/> object.</returns>
        Task<IActionResponse> HandleRouteNotFoundAsync(string routePath);
        /// <summary>
        /// Handle error or execption asynchronously.
        /// </summary>
        /// <param name="type">The <see cref="UrlSchemeType"/> instance.</param>
        /// <param name="fileInfo">The <see cref="FileInfo"/> instance.</param>
        /// <param name="fileStream">The <see cref="Stream"/> instance.</param>
        /// <param name="exception">The <see cref="Exception"/> instance.</param>
        /// <returns>The <see cref="Task"/> that returns <see cref="IResponse"/> object.</returns>
        Task<IResponse> HandleErrorAsync(UrlSchemeType type, FileInfo fileInfo, Stream fileStream, Exception exception);
        /// <summary>
        /// Handle error or execption asynchronously.
        /// </summary>
        /// <param name="type">The <see cref="UrlSchemeType"/> instance.</param>
        /// <param name="request">The <see cref="IRequest"/> instance.</param>
        /// <param name="response">The <see cref="IResponse"/> instance.</param>
        /// <param name="exception">The <see cref="Exception"/> instance.</param>
        /// <returns>The <see cref="Task"/> that returns <see cref="IResponse"/>.</returns>
        Task<IResponse> HandleErrorAsync(UrlSchemeType type, IRequest request, IResponse response, Exception exception);
        /// <summary>
        /// Handle error or execption asynchronously.
        /// </summary>
        /// <param name="type">The <see cref="UrlSchemeType"/> instance.</param>
        /// <param name="request">The <see cref="IActionRequest"/> instance.</param>
        /// <param name="exception">The <see cref="Exception"/> instance.</param>
        /// <returns>The <see cref="Task"/> that returns <see cref="IActionResponse"/>.</returns>
        Task<IActionResponse> HandleErrorAsync(UrlSchemeType type, IActionRequest request, Exception exception);
    }
}
