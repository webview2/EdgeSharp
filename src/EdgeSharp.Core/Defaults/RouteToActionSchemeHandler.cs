using EdgeSharp.Core.Infrastructure;
using EdgeSharp.Core.Network;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EdgeSharp.Core.Defaults
{
    /// <summary>
    /// The default implementation of <see cref="IRouteToActionSchemeHandler"/>.
    /// </summary>
    public class RouteToActionSchemeHandler : IRouteToActionSchemeHandler 
    {
        protected readonly IActionControllerProvider _controllerProvider;
        protected readonly IActionRouteProvider _routeProvider;
        protected readonly IDataTransferOptions _dataTransfers;
        protected readonly IErrorHandler _errorHandler;

        /// <summary>
        /// Initializes a new instance of <see cref="RouteToActionSchemeHandler"/>
        /// </summary>
        /// <param name="controllerProvider">The <see cref="IActionControllerProvider"/> instance</param>
        /// <param name="routeProvider">The <see cref="IActionRouteProvider"/> instance</param>
        /// <param name="dataTransfers">The <see cref="IDataTransferOptions"/> instance</param>
        /// <param name="errorHandler">The <see cref="IErrorHandler"/> instance</param>
        public RouteToActionSchemeHandler(IActionControllerProvider controllerProvider,
                                                   IActionRouteProvider routeProvider,
                                                   IDataTransferOptions dataTransfers,
                                                   IErrorHandler errorHandler)
        {
            UrlScheme = new UrlScheme(UrlSchemeType.RouteToAction);
            _controllerProvider = controllerProvider;
            _routeProvider = routeProvider;
            _dataTransfers = dataTransfers;
            _errorHandler = errorHandler;
        }

        /// <inheritdoc />
        public UrlScheme UrlScheme { get; }

        /// <inheritdoc />
        public virtual void ProcessRequest(IRequest request, CoreWebView2Deferral deferral, Action<IResponse, CoreWebView2Deferral> callback)
        {
            Task.Run(async () =>
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
            });
        }
    }
}

