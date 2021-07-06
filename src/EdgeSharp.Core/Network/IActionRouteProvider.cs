// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System.Collections.Generic;

namespace EdgeSharp.Core.Network
{
    /// <summary>
    /// Collection and provider of controller action routes.
    /// </summary>
    public interface IActionRouteProvider
    {
        /// <summary>
        /// Gets the action route keys. 
        /// </summary>
        IList<string> RouteKeys { get; }
        /// <summary>
        /// Gets action routes map.
        /// </summary>
        IDictionary<string, Route> RouteMap { get; }

        /// <summary>
        /// Register all action routes.
        /// </summary>
        /// <param name="controllers">List of <see cref="ActionController"/> instance.</param>
        void RegisterAllRoutes(List<ActionController> controllers);
        /// <summary>
        /// Register a single action route.
        /// </summary>
        /// <param name="key">The route key.</param>
        /// <param name="route">The <see cref="Route"/> instance.</param>
        void RegisterRoute(string key, Route route);
        /// <summary>
        /// Register multiple action routes.
        /// </summary>
        /// <param name="routeMap">The route map.</param>
        void RegisterRoutes(IDictionary<string, Route> routeMap);
        /// <summary>
        /// Gets a single action route based on route url.
        /// </summary>
        /// <param name="routeUrl">The route url.</param>
        /// <returns>The <see cref="Route"/> object.</returns>
        Route GetRoute(string routeUrl);
        /// <summary>
        /// Checks if a route exists using the route url.
        /// </summary>
        /// <param name="routeUrl">The route url.</param>
        /// <returns>True or false.</returns>
        bool RouteExists(string routeUrl);
        /// <summary>
        /// Checks if the associated controller action is asynchronous.
        /// </summary>
        /// <param name="routeUrl">The route url.</param>
        /// <returns>True or false.</returns>
        bool IsRouteAsync(string routeUrl);
        /// <summary>
        /// Checks if the associated controller action has return.
        /// </summary>
        /// <param name="routeUrl">The route url.</param>
        /// <returns>True or false.</returns>
        bool RouteHasReturnValue(string routeUrl);
    }
}
