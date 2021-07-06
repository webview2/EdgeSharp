// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using EdgeSharp.Core.Infrastructure;

namespace EdgeSharp.Core.Network
{
    public abstract class ActionController
    {
        private string _routePath;
        private string _name;
        private string _description;

        public string RoutePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_routePath))
                {
                    SetAttributeInfo();
                }

                return _routePath;
            }
        }

        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_name))
                {
                    SetAttributeInfo();
                }

                return _name;
            }
        }

        public string Description
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_description))
                {
                    SetAttributeInfo();
                }

                return _description;
            }
        }

        private void SetAttributeInfo()
        {
            try
            {
                var attribute = GetType().GetCustomAttribute<ActionControllerAttribute>(true);
                if (attribute != null)
                {
                    _routePath = attribute.RoutePath;
                    _name = attribute.Name;
                    _description = attribute.Description;
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }
    }
}
