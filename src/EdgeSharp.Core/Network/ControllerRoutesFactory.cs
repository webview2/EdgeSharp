// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace EdgeSharp.Core.Network
{
    public class ControllerRoutesFactory
    {
        /// <summary>
        /// Creates an instance of a controller of given ControllerBase type.
        /// Ctor dependency injection is done using the global IoC container.
        /// </summary>
        /// <param name="type">Controller type to be created.</param>
        /// <returns>Instance reference or null if failed.</returns>
        public ActionController CreateControllerInstance(Type type)
        {
            var instance = CreateType(type);
            return instance as ActionController;
        }

        public void CreateAndRegisterRoutes(IActionRouteProvider routeProvider, ActionController controller, IActionParameterBinder actionParameterBinder, IDataTransferOptions dataTransferOptions)
        {
            if (routeProvider == null || controller == null)
            {
                return;
            }

            var methodInfos = controller.GetType().GetMethods()
             .Where(m => m.GetCustomAttributes(typeof(ActionRouteAttribute), false).Length > 0)
             .ToArray();

            foreach (var methodInfo in methodInfos)
            {
                var attribute = methodInfo.GetCustomAttribute<ActionRouteAttribute>();
                var key = RouteKeys.CreateActionKey(controller.RoutePath, attribute.Path);
                if (!routeProvider.RouteExists(key))
                {
                    routeProvider.RegisterRoute(key, CreateDelegate(controller, methodInfo, actionParameterBinder, dataTransferOptions));
                }
            }
        }

        private object CreateType(Type type)
        {
            object instance = null;
            foreach (var constructor in type.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length == 0)
                {
                    instance = Activator.CreateInstance(type);
                    break;
                }

                var paramValues = new object[parameters.Length];

                for (var ix = 0; ix < parameters.Length; ix++)
                {
                    var parameterInfo = parameters[ix];
                    var parameterInstance = CreateType(parameterInfo.ParameterType);

                    paramValues[ix] = parameterInstance;
                }

                try
                {
                    instance = Activator.CreateInstance(type, paramValues);
                    break;
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log.LogError(ex);
                }
            }

            return instance;
        }

        private static readonly Type VoidType = typeof(void);
        private Route CreateDelegate(object instance, MethodInfo method, IActionParameterBinder actionParameterBinder, IDataTransferOptions dataTransferOptions)
        {
            var args = method
                .GetParameters();

            var arguments = new List<RouteArgument>();

            int lenght = args.Length;

            for (int i = 0; i < lenght; i++)
            {
                arguments.Add(new RouteArgument(args[i].Name, args[i].ParameterType, i));
            }

            var argTypes = args
                .Select(p => p.ParameterType)
                .Concat(new[] { method.ReturnType })
                .ToArray();

            var newDelType = Expression.GetDelegateType(argTypes);
            var newDel = Delegate.CreateDelegate(newDelType, instance, method);

            bool isAsync = method.ReturnType.IsSubclassOf(typeof(Task));
            bool hasReturn = method.ReturnType != VoidType;

            // It is async method without return (void - System.Threading.Tasks.VoidTaskResult)
            if (method.ReturnType == typeof(Task))
            {
                isAsync = true;
                hasReturn = false;
            }

            return new Route(method.Name, newDel, arguments, actionParameterBinder, dataTransferOptions,isAsync, hasReturn);
        }
    }
}