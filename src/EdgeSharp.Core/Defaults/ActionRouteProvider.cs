using System;
using System.Collections.Generic;
using System.Linq;
using EdgeSharp.Core.Infrastructure;
using EdgeSharp.Core.Network;
using Microsoft.Extensions.Logging;

namespace EdgeSharp.Core.Defaults
{
    /// <summary>
    /// The default implementation of <see cref="IActionRouteProvider"/>.
    /// </summary>
    public class ActionRouteProvider : IActionRouteProvider
    {
        protected readonly IActionParameterBinder _actionParameterBinder;
        protected readonly IDataTransferOptions _dataTransferOptions;

        /// <summary>
        /// Initializes a new instance of <see cref="ActionRouteProvider"/>.
        /// </summary>
        /// <param name="actionParameterBinder">The <see cref="IActionParameterBinder"/> instance.</param>
        /// <param name="dataTransferOptions">The <see cref="IDataTransferOptions"/> instance.</param>
        public ActionRouteProvider(IActionParameterBinder actionParameterBinder, IDataTransferOptions dataTransferOptions)
        {
            _actionParameterBinder = actionParameterBinder;
            _dataTransferOptions = dataTransferOptions;

            RouteMap = new Dictionary<string, Route>();
        }

        /// <inheritdoc />
        public IDictionary<string, Route> RouteMap { get; }

        /// <inheritdoc />
        public IList<string> RouteKeys
        {
            get
            {
                return RouteMap?.Keys?.ToList();
            }
        }

        /// <inheritdoc />
        public virtual void RegisterAllRoutes(List<ActionController> controllers)
        {
            if (controllers == null || !controllers.Any())
            {
                return;
            }

            try
            {

                foreach (var controller in controllers)
                {
                    var controllerRoutesFactory = new ControllerRoutesFactory();
                    controllerRoutesFactory.CreateAndRegisterRoutes(this, controller, _actionParameterBinder, _dataTransferOptions);
                }

            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }

        /// <inheritdoc />
        public virtual void RegisterRoute(string key, Route route)
        {
             RouteMap.Add(key, route);
        }

        /// <inheritdoc />
        public virtual void RegisterRoutes(IDictionary<string, Route> routeDMap)
        {
            if (routeDMap != null && routeDMap.Any())
            {
                foreach (var item in routeDMap)
                {
                    RegisterRoute(item.Key, item.Value);
                }
            }
        }

        /// <inheritdoc />
        public virtual Route GetRoute(string routeUrl)
        {
            var key = Network.RouteKeys.CreateActionKey(routeUrl);
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            if (RouteMap.ContainsKey(key))
            {
                return RouteMap[key];
            }

            return null;
        }

        /// <inheritdoc />
        public virtual bool RouteExists(string routeUrl)
        {
            var keys = RouteKeys;
            if (!keys.IsNullOrEmpty())
            {
                var key = Network.RouteKeys.CreateActionKey(routeUrl);
                if (string.IsNullOrWhiteSpace(key))
                {
                    return false;
                }

                return keys.Contains(key);
            }

            return false;
        }

        /// <inheritdoc />
        public virtual bool IsRouteAsync(string routeUrl)
        {
            try
            {
                var route = GetRoute(routeUrl);
                return route == null ? false : route.IsAsync;
            }
            catch {}

            return false;
        }

        /// <inheritdoc />
        public virtual bool RouteHasReturnValue(string routeUrl)
        {
            try
            {
                var route = GetRoute(routeUrl);
                return route == null ? false : route.HasReturnValue;
            }
            catch { }

            return false;
        }
    }
}