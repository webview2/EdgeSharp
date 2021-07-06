using Microsoft.Extensions.Logging;
using System;
using System.Runtime.InteropServices;
using EdgeSharp.Core.Network;
using EdgeSharp.Core.Infrastructure;

namespace EdgeSharp.Core.Defaults
{
    /// <summary>
    /// The default implementation of <see cref="IHostObjectProvider"/>.
    /// </summary>
    public partial class HostObjectProvider : IHostObjectProvider
    {
        private const string ObjectName = "execute";

        protected readonly IActionControllerProvider _controllerProvider;
        protected readonly IDataTransferOptions _dataTransferOptions;
        protected RequestHandlerServer _hostObject;

        /// <summary>
        ///  Initializes a new instance of <see cref="HostObjectProvider"/>.
        /// </summary>
        /// <param name="controllerProvider">The <see cref="IActionControllerProvider"/> instance.</param>
        /// <param name="dataTransferOptions">The <see cref="IDataTransferOptions"/> instance.</param>
        public HostObjectProvider(IActionControllerProvider controllerProvider, IDataTransferOptions dataTransferOptions)
        {
            _controllerProvider = controllerProvider;
            _dataTransferOptions = dataTransferOptions;
        }

        /// <inheritdoc />
        public string Name => ObjectName;

        /// <inheritdoc />
        public object HostObject
        {
            get
            {
                try
                {
                    _hostObject = new RequestHandlerServer();
                    if (_hostObject != null)
                    {
                        HostObjectCommon.SendRequestDelegate sendRequestDelegate = new HostObjectCommon.SendRequestDelegate(Send);
                        var sendRequestPtr = Marshal.GetFunctionPointerForDelegate(sendRequestDelegate);

                        _hostObject.RegisterSendCallback(sendRequestPtr);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Instance.Log.LogError(exception);
                }

                return _hostObject;

            }
        }

        private string Send(string requestUrl, object requestContent)
        {
            try
            {
                var request = ActionRequest.CreateRequest(requestUrl, requestContent);
                IActionResponse response = _controllerProvider?.Execute(request);
                return _dataTransferOptions.ConvertResponseToJson(response?.Content);
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
                return "An error occurs";
            }
        }

        #region Disposal

        private bool _disposed;

        ~HostObjectProvider()
        {
            Dispose(false);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            // If there are managed resources
            if (disposing)
            {
                if (_hostObject != null && Marshal.IsComObject(_hostObject))
                {
                    Marshal.ReleaseComObject(_hostObject);
                }

                _hostObject = null;
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
