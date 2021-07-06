// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using EdgeSharp.Core.Network;
using EdgeSharp.Core.Configuration;
using EdgeSharp.Core.Infrastructure;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EdgeSharp.Core.Defaults
{
    /// <summary>
    /// The default implementation of <see cref="IResourceRequestHandler"/>.
    /// </summary>
    public partial class ResourceRequestHandler : IResourceRequestHandler
    {
        protected readonly IConfiguration _config;
        protected readonly IScriptExecutor _scriptExecutor;
        protected readonly IHostObjectProvider _hostObjectHandler;
        protected readonly IActionControllerProvider _controllerProvider;
        protected readonly IActionRouteProvider _routeProvider;
        protected readonly IDataTransferOptions _dataTransferOptions;

        protected IDictionary<string, ISchemeHandler> _schemeHandlerMap = new Dictionary<string, ISchemeHandler>();
        private CoreWebView2EnvironmentOptions _environmentOptions;
        private List<string> _hostObjectNames;

        /// <summary>
        /// Initializes a new instance of <see cref="ResourceRequestHandler"/>
        /// </summary>
        /// <param name="config">EdgeSharp <see cref="IConfiguration"/> instance.</param>
        /// <param name="scriptExecutor">The <see cref="IScriptExecutor"/> instance.</param>
        /// <param name="hostObjectHandler">The <see cref="IHostObjectProvider"/> instance.</param>
        /// <param name="controllerProvider">The <see cref="IActionControllerProvider"/> instance.</param>
        /// <param name="routeProvider">The <see cref="IActionRouteProvider"/> instance.</param>
        /// <param name="dataTransferOptions">The <see cref="IDataTransferOptions"/> instance.</param>
        public ResourceRequestHandler(IConfiguration config, IScriptExecutor scriptExecutor, IHostObjectProvider hostObjectHandler,
                           IActionControllerProvider controllerProvider, IActionRouteProvider routeProvider,
                            IDataTransferOptions dataTransferOptions)
        {
            _config = config;
            _scriptExecutor = scriptExecutor;
            _hostObjectHandler = hostObjectHandler;
            _controllerProvider = controllerProvider;
            _routeProvider = routeProvider;
            _dataTransferOptions = dataTransferOptions;
        }

        /// <inheritdoc />
        public CoreWebView2 CoreWebView2 { get; private set; }

        /// <inheritdoc />
        public virtual void Initialize(CoreWebView2 coreWebView2)
        {
            CacheAllRegisteredSchemeHandlers();
            CoreWebView2 = coreWebView2;

            RegisterHostObjects();
            MapHostToLocalFolder();
            AddScriptToExecuteOnDocumentCreatedAsync();
        }

        /// <inheritdoc />
        public virtual void HandleResourceRequest(CoreWebView2WebResourceRequestedEventArgs eventArgs)
        {
            void callback(IResponse response, CoreWebView2Deferral deferral)
            {
                try
                {
                    if (response != null)
                    {
                        var createdResponse = CoreWebView2.Environment.CreateWebResourceResponse(response.Content as Stream,
                                                                                                (int)response.StatusCode,
                                                                                                response.ReasonPhrase,
                                                                                                response.ResponseHeadersToString());

                        if (createdResponse != null)
                        {
                            eventArgs.Response = createdResponse;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logger.Instance.Log.LogError(exception);
                }
                finally
                {
                    deferral?.Complete();
                    deferral = null;
                }
            }

            var request = new Request(eventArgs.Request.Uri, eventArgs.Request.Method, eventArgs.Request.Headers.RequestHeadersToDictionary(), eventArgs.Request.Content);
            var schemeType = GetResourceRequestScheme(request.Url);
            switch (schemeType)
            {
                case UrlSchemeType.None:
                    break;

                case UrlSchemeType.ResourceRequest:
                case UrlSchemeType.RouteToAction:
                case UrlSchemeType.Owin:
                    var resourceSchemeHandler = GetSchemeHandler(request.Url);
                    if (resourceSchemeHandler != null)
                    {
                        resourceSchemeHandler.ProcessRequest(request, eventArgs.GetDeferral(), callback);
                    }
                    break;

                case UrlSchemeType.ExternalBrowser:
                    SetNoContentResponse(ResponseConstants.ReasonPhrase_PassThru, eventArgs.GetDeferral(), callback);
                    LaunchBrowser(request);
                    break;

                case UrlSchemeType.Blocked:
                    SetNoContentResponse(ResponseConstants.ReasonPhrase_Blocked, eventArgs.GetDeferral(), callback);
                    break;

                default:
                    break;
            }
        }

        /// <inheritdoc />
        public virtual void HandleWebMessageReceived(CoreWebView2WebMessageReceivedEventArgs eventArgs)
        {
            async void callback(string script)
            {
                try
                {
                    var result = await CoreWebView2.ExecuteScriptAsync(script);
                }
                catch (Exception exception)
                {
                    Logger.Instance.Log.LogError(exception);
                }
            }

            IActionRequest actionRequest = ActionRequest.CreateRequest(eventArgs.TryGetWebMessageAsString(), eventArgs.WebMessageAsJson);
            if (_routeProvider.RouteExists(actionRequest.RoutePath))
            {
                _scriptExecutor?.ExecuteScript(actionRequest, callback);
            }
        }

        /* https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/204#:~:text=The%20HTTP%20204%20No%20Content%20success%20status%20response,ETag%20header%20is%20included%20in%20such%20a%20response.
            The HTTP 204 No Content success status response code indicates that the request has succeeded, 
            but that the client doesn't need to go away from its current page. 
            A 204 response is cacheable by default. 
            An ETag header is included in such a response.
        */
        /// <inheritdoc />
        public virtual void SetNoContentResponse(string reasonPhrase, CoreWebView2Deferral deferral, Action<IResponse, CoreWebView2Deferral> callback)
        {
            reasonPhrase ??= ResponseConstants.ReasonPhrase_NoContent;
            var response = new Response(HttpStatusCode.NoContent, reasonPhrase, null, reasonPhrase.GetMemoryStream());
            SetResponse(response, deferral, callback);
        }

        /// <inheritdoc />
        public void SetResponse(IResponse response, CoreWebView2Deferral deferral, Action<IResponse, CoreWebView2Deferral> callback)
        {
            callback?.Invoke(response, deferral);
        }

        /// <inheritdoc />
        public virtual void AddHostObject(string name, object rawObject)
        {
            try
            {
                var coreWebView2 = this.CoreWebView2;
                if (coreWebView2 != null)
                {
                    coreWebView2.AddHostObjectToScript(name, rawObject);
                    CacheHostObjectName(name);
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }

        /// <inheritdoc />
        public virtual void RemoveHostObject(string name)
        {
            try
            {
                var coreWebView2 = this.CoreWebView2;
                if (coreWebView2 != null)
                {
                    coreWebView2.RemoveHostObjectFromScript(name);
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }

        /// <inheritdoc />
        public virtual void RemoveAllHostObjects()
        {
            if (_hostObjectNames != null)
            {
                foreach (var name in _hostObjectNames)
                {
                    RemoveHostObject(name);
                }
            }
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            _hostObjectHandler.Dispose();
        }

        #region Helpers

        private UrlSchemeType GetResourceRequestScheme(string url)
        {
            var schemeType = UrlSchemeType.None;

            var scheme = _config?.UrlSchemes?.GetScheme(url);
            if (scheme != null)
            {
                return scheme.SchemeType;
            }

            return schemeType;
        }

        private void LaunchBrowser(IRequest request)
        {
            try
            {
                var requestUri = request.Url;
                try
                {
                    Process.Start(requestUri);
                }
                catch
                {
                    try
                    {
                        // hack because of this: https://github.com/dotnet/corefx/issues/10361
                        requestUri = requestUri.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo("cmd", $"/c start {requestUri}") { CreateNoWindow = true });
                    }
                    catch (Exception exception)
                    {
                        Logger.Instance.Log.LogError(exception);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }

        private ISchemeHandler GetSchemeHandler(string requestUri)
        {
            var schemeKey = UrlScheme.CreateKey(requestUri);
            EnsureSchemeHandlingContainersInitialized();
            if (_schemeHandlerMap.ContainsKey(schemeKey))
            {
                return _schemeHandlerMap[schemeKey];
            }

            return null;
        }


        private void CacheAllRegisteredSchemeHandlers()
        {
            EnsureSchemeHandlingContainersInitialized();

            // Get all custom resource or request scheme handlers
            var customSchemeHandlers = ServiceLocator.Current.GetInstances(typeof(ISchemeHandler)) as IList<ISchemeHandler>;
            if (!customSchemeHandlers.IsNullOrEmpty())
            {
                foreach (var handler in customSchemeHandlers)
                {
                    if (handler != null && handler?.UrlScheme != null)
                    {
                        var key = handler.UrlScheme?.Key;
                        if (!string.IsNullOrWhiteSpace(key) && !_schemeHandlerMap.ContainsKey(key))
                        {
                            _schemeHandlerMap[key] = handler;
                            _config.UrlSchemes.AddScheme(handler.UrlScheme);
                        }
                    }
                }
            }

            // If no custom scheme handler exist 
            // Replace with default handlers
            var defaultSchemeHandlerTypes = new Type[] { typeof(IResourceRequestSchemeHandler),
                                                         typeof(IRouteToActionSchemeHandler),
                                                         typeof(IOwinSchemeHandler) };

            foreach (var handlerType in defaultSchemeHandlerTypes)
            {
                var defaultSchemeHandlerObjs = ServiceLocator.Current.GetInstances(handlerType);
                if (!defaultSchemeHandlerObjs.IsNullOrEmpty())
                {
                    foreach (var defaultSchemeHandler in defaultSchemeHandlerObjs)
                    {
                        var handler = defaultSchemeHandler as ISchemeHandler;
                        if (handler != null && handler?.UrlScheme != null)
                        {
                            var key = handler.UrlScheme?.Key;
                            if (!string.IsNullOrWhiteSpace(key) && !_schemeHandlerMap.ContainsKey(key))
                            {
                                _schemeHandlerMap[key] = handler;
                                _config.UrlSchemes.AddScheme(handler.UrlScheme);
                            }

                            // Get similar schemes not yet added to the scheme handler map
                            var schemes = _config.UrlSchemes.GetSchemes(handler.UrlScheme.SchemeType);
                            if (!schemes.IsNullOrEmpty())
                            {
                                foreach (var scheme in schemes)
                                {
                                    key = scheme.Key;
                                    if (!string.IsNullOrWhiteSpace(key) && !_schemeHandlerMap.ContainsKey(key))
                                    {
                                        _schemeHandlerMap[key] = handler;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void EnsureSchemeHandlingContainersInitialized()
        {
            if (_schemeHandlerMap == null)
            {
                _schemeHandlerMap = new Dictionary<string, ISchemeHandler>();
            }

            if (_config.UrlSchemes == null)
            {
                _config.UrlSchemes = new List<UrlScheme>();
            }
        }

        private void RegisterHostObjects()
        {
            try
            {
                if (_hostObjectHandler != null)
                {
                    var remotObject = _hostObjectHandler.HostObject;
                    if (remotObject != null)
                    {
                        AddHostObject(_hostObjectHandler.Name, remotObject);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }

        private void MapHostToLocalFolder()
        {
            try
            {
                if (CoreWebView2 != null)
                {
                    var schemes = _config.UrlSchemes.GetSchemes(UrlSchemeType.HostToFolder);
                    if (!schemes.IsNullOrEmpty())
                    {
                        foreach (var scheme in schemes)
                        {
                            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                            var folder = Path.Combine(appDirectory, scheme.Folder);
                            if (Directory.Exists(folder))
                            {
                                CoreWebView2.SetVirtualHostNameToFolderMapping(scheme.Host, scheme.Folder, CoreWebView2HostResourceAccessKind.Allow);
                            }
                            else
                            {
                                Logger.Instance.Log.LogWarning($"Host name to folder map: folder: {folder} does not exist.");
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }

        private async Task AddScriptToExecuteOnDocumentCreatedAsync()
        {
            try
            {
                if (CoreWebView2 != null)
                {
                    var scripts = this._scriptExecutor.OnDocumentCreatedScripts;
                    if (scripts != null)
                    {
                        foreach (var script in scripts)
                        {
                            await CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(script).ConfigureAwait(true);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }

        private void CacheHostObjectName(string name)
        {
            if (_hostObjectNames == null)
            {
                _hostObjectNames = new List<string>();
            }

            _hostObjectNames.Add(name);
        }

        private string GetAdditionalBrowserArguments(string additionalBrowserArguments)
        {
            var builder = new StringBuilder();

            var commandLineOptions = _config?.CommandLineOptions;
            if (commandLineOptions != null)
            {
                foreach (var lineOption in commandLineOptions)
                {
                    var currOption = lineOption;
                    if (!currOption.StartsWith(CMDLINE_ARG_START1) && !currOption.StartsWith(CMDLINE_ARG_START2))
                    {
                        currOption = $"{CMDLINE_ARG_START1}{currOption}";
                    }

                    builder.Append($"{currOption}{SPACER}");
                }
            }

            var commandLineArgs = _config?.CommandLineArgs;
            if (commandLineArgs != null)
            {
                foreach (var arg in commandLineArgs)
                {
                    var key = arg.Key;
                    if (!key.StartsWith(CMDLINE_ARG_START1) && !key.StartsWith(CMDLINE_ARG_START2))
                    {
                        key = $"{CMDLINE_ARG_START1}{key}";
                    }

                    var value = arg.Value.Trim().TrimStart(QUOTE).TrimEnd(QUOTE);
                    builder.Append($"{key}={QUOTE}{value}{QUOTE}{SPACER}");
                }
            }

            if (!string.IsNullOrWhiteSpace(additionalBrowserArguments))
            {
                builder.Append($"{additionalBrowserArguments}{SPACER}");
            }

            return builder.ToString();
        }

        #endregion Helpers
    }
}
