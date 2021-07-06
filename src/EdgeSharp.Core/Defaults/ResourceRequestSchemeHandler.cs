using EdgeSharp.Core.Infrastructure;
using EdgeSharp.Core.Network;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EdgeSharp.Core.Defaults
{
    /// <summary>
    /// The default implementation of <see cref="IResourceRequestSchemeHandler"/>.
    /// </summary>
    public class ResourceRequestSchemeHandler : IResourceRequestSchemeHandler
    {
        protected readonly IActionControllerProvider _controllerProvider;
        protected readonly IActionRouteProvider _routeProvider;
        protected readonly IDataTransferOptions _dataTransfers;
        protected readonly IErrorHandler _errorHandler;
        private string _mimeType;

        /// <summary>
        /// Initializes a new instance of <see cref="ResourceRequestSchemeHandler"/>
        /// </summary>
        /// <param name="controllerProvider">The <see cref="IActionControllerProvider"/> instance.</param>
        /// <param name="routeProvider">The <see cref="IActionRouteProvider"/> instance.</param>
        /// <param name="dataTransfers">The <see cref="IDataTransferOptions"/> instance.</param>
        /// <param name="errorHandler">The <see cref="IErrorHandler"/> instance.</param>
        public ResourceRequestSchemeHandler(IActionControllerProvider controllerProvider, 
                                                   IActionRouteProvider routeProvider,
                                                   IDataTransferOptions dataTransfers,
                                                   IErrorHandler errorHandler)
        {
            UrlScheme = new UrlScheme(UrlSchemeType.ResourceRequest);
            _controllerProvider = controllerProvider;
            _routeProvider = routeProvider;
            _dataTransfers = dataTransfers;
            _errorHandler = errorHandler;
            _mimeType = "text/plain";
        }

        /// <inheritdoc />
        public UrlScheme UrlScheme { get; }

        /// <inheritdoc />
        public virtual void ProcessRequest(IRequest request, CoreWebView2Deferral deferral, Action<IResponse, CoreWebView2Deferral> callback)
        {
            Task.Run(async () =>
            {
                FileInfo fileInfo = null;
                bool isFileResource = GetFileResource(request.Url, out fileInfo);

                if (isFileResource)
                {
                    await ProcessFileResourceAsync(fileInfo, deferral, callback);
                }
                else
                {
                    await ProcessRouteToActionRequestAsync(request, deferral, callback);
                }
            });
        }

        private async Task ProcessFileResourceAsync(FileInfo fileInfo, CoreWebView2Deferral deferral, Action<IResponse, CoreWebView2Deferral> callback)
        {
            IResponse response = new Response();

            try
            {
                if (fileInfo != null && fileInfo.Exists && fileInfo.Length > 0)
                {
                    response.Content = new MemoryStream(File.ReadAllBytes(fileInfo.FullName));
                    response.StatusCode = HttpStatusCode.OK;
                    response.ReasonPhrase = ResponseConstants.StatusOKText;

                    string extension = Path.GetExtension(fileInfo.FullName);
                    _mimeType = MimeMapper.GetMimeType(extension);
                }
                else
                {
                    response = await _errorHandler.HandleErrorAsync(UrlSchemeType.ResourceRequest, fileInfo, null, null);
                }
            }
            catch (Exception exception)
            {
                response = await _errorHandler.HandleErrorAsync(UrlSchemeType.ResourceRequest, fileInfo, null, exception);
            }
            finally
            {
                response.Headers[ResponseConstants.Header_ContentType] = GetMimeTypeHeader(response.Headers, _mimeType);

                Dispatcher.Browser.Post(() =>
                {
                    callback.Invoke(response, deferral);
                });
            }
        }

        private async Task ProcessRouteToActionRequestAsync(IRequest request, CoreWebView2Deferral deferral, Action<IResponse, CoreWebView2Deferral> callback)
        {
            IResponse response = new Response();
            try
            {
                var actionRequest = ActionRequest.CreateRequest(request);
                var actionResponse = _controllerProvider.Execute(actionRequest);

                if (actionResponse != null)
                {
                    if (!_routeProvider.RouteHasReturnValue(request.Url))
                    {
                        response = new Response(HttpStatusCode.NoContent, ResponseConstants.ReasonPhrase_PassThru, null, ResponseConstants.ReasonPhrase_PassThru.GetMemoryStream());
                    }
                    else
                    {
                        var content = actionResponse.Content as Stream;
                        if (content == null)
                        {
                            var jsonData = _dataTransfers.ConvertResponseToJson(actionResponse.Content);
                            var responseBytes = Encoding.UTF8.GetBytes(jsonData);
                            content = new MemoryStream(responseBytes);
                        }

                        response = new Response(actionResponse.StatusCode, actionResponse.ReasonPhrase, actionResponse.Headers, content);
                    }
                }
            }
            catch (Exception exception)
            {
                response = await _errorHandler.HandleErrorAsync(UrlSchemeType.ResourceRequest, request, null, exception);
            }
            finally
            {
                Dispatcher.Browser.Post(() =>
                {
                    callback.Invoke(response, deferral);
                });
            }
        }

        private bool GetFileResource(string url, out FileInfo fileInfo)
        {
            var uri = new Uri(url);
            var folder = string.IsNullOrWhiteSpace(UrlScheme?.Folder) ? uri.Authority : UrlScheme.Folder;
            var file = folder + uri.AbsolutePath;
            fileInfo = new FileInfo(file);

            if (fileInfo.Exists)
            {
                return true;
            }

            string extension = Path.GetExtension(file);
            return !string.IsNullOrWhiteSpace(extension);
        }

        private string[] GetMimeTypeHeader(IDictionary<string, string[]> headers, string mimeType)
        {
            List<string> headerValues = new List<string>();
            if (headers != null && headers.ContainsKey(ResponseConstants.Header_ContentType))
            {
                headerValues = headers[ResponseConstants.Header_ContentType].ToList();
            }

            if(!headerValues.Contains(mimeType))
            {
                headerValues.Add(mimeType);
            }

            return headerValues.ToArray();
        }
    }
}

