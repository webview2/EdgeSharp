// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System;

namespace EdgeSharp.Core.Infrastructure
{
    public static class AppBuilderExtensions
    {
        public static void EnsureIsDerivedType(this Type derivedType, Type baseType)
        {
            if (baseType == derivedType)
            {
                throw new Exception($"Cannot specify the base type {baseType.Name} itself as generic type parameter.");
            }

            if (!baseType.IsAssignableFrom(derivedType))
            {
                throw new Exception($"Type {derivedType.Name} must implement {baseType.Name}.");
            }

            if (derivedType.IsAbstract || derivedType.IsInterface)
            {
                throw new Exception($"Type {derivedType.Name} cannot be an interface or abstract class.");
            }
        }
    }
}
