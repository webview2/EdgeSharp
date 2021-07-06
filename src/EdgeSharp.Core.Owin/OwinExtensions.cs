// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EdgeSharp.Core.Owin
{
    public static class OwinExtensions
    {
        public static bool IsOwinApp(this IStartup startup)
        {
            return typeof(IOwinAppStartup).IsAssignableFrom(startup.GetType());
        }

        public static void ParseRoutes(this IOwinPipeline owinPipeline, IServiceProvider provider)
        {
            try
            {
                owinPipeline.Routes = owinPipeline.Routes ?? new List<OwinRoute>();
                var routes = new List<OwinRoute>();

                var actionDescriptorCollectionProvider = provider.GetRequiredService<IActionDescriptorCollectionProvider>();
                if (actionDescriptorCollectionProvider != null)
                {
                    var controllerActions = actionDescriptorCollectionProvider.ActionDescriptors.Items.OfType<ControllerActionDescriptor>().ToList();
                    if (!controllerActions.IsNullOrEmpty())
                    {
                        foreach (var action in controllerActions)
                        {
                            var template = action.AttributeRouteInfo?.Template;
                            if (template != null)
                            {
                                template = template.TrimStart('/');
                                routes.Add(new OwinRoute(action.DisplayName,
                                    $"/{template}"));
                            }

                            routes.Add(new OwinRoute(action.DisplayName,
                                                     $"/{action.ControllerName}/{action.ActionName}"));
                        }

                        owinPipeline.Routes.AddRange(routes);
                    }

                    var pagesActions = actionDescriptorCollectionProvider.ActionDescriptors.Items.OfType<PageActionDescriptor>().ToList();
                    if (!pagesActions.IsNullOrEmpty())
                    {
                        foreach (var action in pagesActions)
                        {
                            routes.Add(new OwinRoute(action.DisplayName,
                                                     action.ViewEnginePath,
                                                     action.RelativePath));
                        }

                        owinPipeline.Routes.AddRange(routes);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }

        public static bool IsUrlActionRoute(this IOwinPipeline owinPipeline, string url)
        {
            try
            {
                owinPipeline.Routes = owinPipeline.Routes ?? new List<OwinRoute>();
                var uri = new Uri(url);
                return owinPipeline.Routes.Any(x => x.RoutePath.Equals(uri.AbsolutePath, StringComparison.InvariantCultureIgnoreCase) ||
                                                    x.RoutePath.Equals(uri.AbsolutePath + "/Index", StringComparison.InvariantCultureIgnoreCase));

            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }

            return false;
        }

        public static bool IsUrlErrorHandlingPath(this IOwinPipeline owinPipeline, string url)
        {
            try
            {
                var uri = new Uri(url);
                return owinPipeline.ErrorHandlingPath.Equals(uri.AbsolutePath, StringComparison.InvariantCultureIgnoreCase) ||
                       owinPipeline.ErrorHandlingPath.Equals(uri.AbsolutePath + "/Index", StringComparison.InvariantCultureIgnoreCase);

            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }

            return false;
        }
    }
}
