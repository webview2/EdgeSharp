// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Network;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EdgeSharp.Core.Infrastructure
{
    public static class AssembliesExtensions
    {
        public static void RegisterAssembly(this IServiceCollection services, Assembly assemby, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var types = from type in assemby.GetLoadableTypes()
                        where Attribute.IsDefined(type, typeof(ActionControllerAttribute))
                        select type;

            foreach (var type in types)
            {
                if (typeof(ActionController).IsAssignableFrom(type.BaseType))
                {
                    services.Add(new ServiceDescriptor(typeof(ActionController), type, lifetime));
                }
            }
        }

        public static void RegisterAssemblies(this IServiceCollection services, Assembly[] assemblies, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            foreach (var assembly in assemblies)
            {
                RegisterAssembly(services, assembly,lifetime);
            }
        }

        private static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }
}
