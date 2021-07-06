using EdgeSharp.Core.Infrastructure;
using EdgeSharp.Core.Network;
using EdgeSharp.Core.Owin;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace EdgeSharp.Core.Defaults
{
    /// <summary>
    /// The default implementation of <see cref="ErrorHandler"/>.
    /// </summary>
    public class ErrorHandler : IErrorHandler
    {
        protected readonly IOwinPipeline _owinPipeline;

        /// <summary>
        /// Initializes a new instance of <see cref="ErrorHandler"/>.
        /// </summary>
        /// <param name="owinPipeline">The <see cref="IOwinPipeline"/> instance.</param>
        public ErrorHandler(IOwinPipeline owinPipeline)
        {
            _owinPipeline = owinPipeline;
        }

        /// <inheritdoc />
        public virtual async Task<IResponse> HandleErrorAsync(UrlSchemeType type, IRequest request, IResponse response, Exception exception)
        {
            LogException(exception);
            return await Task.FromResult(response);
        }

        /// <inheritdoc />
        public virtual IActionResponse HandleError(UrlSchemeType type, IActionRequest request,  Exception exception)
        {
            LogException(exception);

            return new ActionResponse
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                ReasonPhrase = "An error has occurred"
            };
        }

        /// <inheritdoc />
        public virtual IActionResponse HandleRouteNotFound(string routePath)
        {
            return new ActionResponse
            {
                StatusCode = HttpStatusCode.BadRequest,
                ReasonPhrase = $"Route for path = {routePath} is null or invalid."
            };
        }

        /// <inheritdoc />
        public virtual IResponse HandleError(UrlSchemeType type, IRequest request, IResponse response, Exception exception)
        {
            LogException(exception);

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                ReasonPhrase = "An error has occurred"
            };
        }

        /// <inheritdoc />
        public virtual async Task<IActionResponse> HandleRouteNotFoundAsync(string routePath)
        {
            var actionResponse = HandleRouteNotFound(routePath);
            return await Task.FromResult(actionResponse);
        }

        /// <inheritdoc />
        public virtual async Task<IActionResponse> HandleErrorAsync(UrlSchemeType type, IActionRequest request, Exception exception)
        {
            var actionResponse = HandleError(type, request, exception);
            return await Task.FromResult(actionResponse);
        }

        /// <inheritdoc />
        public virtual IResponse HandleError(UrlSchemeType type, FileInfo fileInfo, Stream fileStream, Exception exception)
        {
            var info = GetFileInfo(fileInfo, fileStream);
            bool fileExists = info.Item1;
            int fileSize = info.Item2;

            return HandleResourceError(fileExists, fileSize, exception);
        }

        /// <inheritdoc />
        public virtual async Task<IResponse> HandleErrorAsync(UrlSchemeType type, FileInfo fileInfo, Stream fileStream, Exception exception)
        {
            var actionResponse = HandleError(type, fileInfo, fileStream, exception);
            return await Task.FromResult(actionResponse);
        }


        private IResponse HandleResourceError(bool fileExists, int fileSize, Exception exception = null)
        {
            LogException(exception);

            var resource = new Response();
            if (!fileExists)
            {
                resource.StatusCode = HttpStatusCode.NotFound;
                resource.ReasonPhrase = "Resource loading error: file size is zero.";
                resource.Content = resource.ReasonPhrase.GetMemoryStream();
            }

            else if (fileSize == 0)
            {
                resource.StatusCode = HttpStatusCode.NotFound;
                resource.ReasonPhrase = "Resource loading error: file size is zero.";
                resource.Content = resource.ReasonPhrase.GetMemoryStream();
            }
            else
            {
                resource.StatusCode = HttpStatusCode.BadRequest;
                resource.ReasonPhrase = "Resource loading error";
                resource.Content = resource.ReasonPhrase.GetMemoryStream();
            }

            return resource;
        }

        private (bool, int) GetFileInfo(FileInfo fileInfo, Stream stream)
        {
            bool fileExists = false;
            int fileSize = 0;

            try
            {
                if (fileInfo != null)
                {
                    fileExists = fileInfo != null && fileInfo.Exists;
                    fileSize = (int)(fileInfo != null ? fileInfo.Length : 0);
                }
                else if (stream != null)
                {
                    fileExists = stream != null;
                    fileSize = (int)(stream != null ? stream.Length : 0);
                }

                return (fileExists, fileSize);
            }
            catch { }

            return (fileExists, fileSize);
        }

        private void LogException(Exception exception)
        {
            if (exception != null)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }
    }
}
