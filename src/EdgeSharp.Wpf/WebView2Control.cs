// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core;
using EdgeSharp.Core.Configuration;
using EdgeSharp.Core.Infrastructure;
using EdgeSharp.Core.Network;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;

namespace EdgeSharp.Wpf
{
    public partial class WebView2Control : WebView2, IWebView2
    {
        protected IConfiguration _config;
        protected IResourceRequestHandler _resourceRequestHandler;

        private string _preInitializeStartUrl;
        private bool _disposed;

        public WebView2Control()
        {
            InitializeAsync();
        }

        public new Uri Source
        {
            get { return base.Source; }
            set 
            {
                base.Source = value;
                var statrtUrl = base.Source?.AbsoluteUri;
                if (_config != null)
                {
                    _config.StartUrl = statrtUrl;
                    _preInitializeStartUrl = null;
                }
                else
                {
                    _preInitializeStartUrl = statrtUrl;
                }
            }
        }

        public CoreWebView2EnvironmentOptions EnvironmentOptions
        {
            get { return _resourceRequestHandler.EnvironmentOptions; }
            set { _resourceRequestHandler.EnvironmentOptions = value; }
        }

        public virtual async void InitializeAsync()
        {
            try
            {
                Core.Infrastructure.Dispatcher.Browser = new BrowserDispatcher(this);
                CoreWebView2InitializationCompleted += OnInitializationCompleted;

                Bootstrap();

                _config = ServiceLocator.Current.GetInstance<IConfiguration>();
                _resourceRequestHandler = ServiceLocator.Current.GetInstance<IResourceRequestHandler>();
                var creationOption = _config?.WebView2CreationOptions ?? new WebView2CreationOptions();

                if (!string.IsNullOrWhiteSpace(_preInitializeStartUrl))
                {
                    _config.StartUrl = _preInitializeStartUrl;
                    _preInitializeStartUrl = null;
                }

                var environment = await CoreWebView2Environment.CreateAsync(BrowserExecutableFolder, UserDataFolder, EnvironmentOptions);
                await EnsureCoreWebView2Async(environment);

                _config.RuntimeVersion = CoreWebView2.Environment.BrowserVersionString;

                CoreWebView2.AddWebResourceRequestedFilter(creationOption.UriFilter, creationOption.ResourceContext);

                CoreWebView2.WebMessageReceived += new EventHandler<CoreWebView2WebMessageReceivedEventArgs>(BrowserWindow_WebMessageReceived);
                CoreWebView2.WebResourceRequested += new EventHandler<CoreWebView2WebResourceRequestedEventArgs>(BrowserWindow_WebResourceRequested);
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
                throw;
            }
        }

        protected virtual void OnInitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs eventArgs)
        {
            CoreWebView2InitializationCompleted -= OnInitializationCompleted;
            Initialize(CoreWebView2);
            Source = new Uri(_config.StartUrl);
        }

        public virtual void Initialize(CoreWebView2 coreWebView2)
        {
            _resourceRequestHandler?.Initialize(coreWebView2);
        }

        public virtual void HandleWebMessageReceived(CoreWebView2WebMessageReceivedEventArgs eventArgs)
        {
            _resourceRequestHandler?.HandleWebMessageReceived(eventArgs);
        }

        public virtual void HandleResourceRequest(CoreWebView2WebResourceRequestedEventArgs eventArgs)
        {
            _resourceRequestHandler?.HandleResourceRequest(eventArgs);
        }

        public virtual void SetNoContentResponse(string reasonPhrase, CoreWebView2Deferral deferral, Action<IResponse, CoreWebView2Deferral> callback)
        {
            _resourceRequestHandler?.SetNoContentResponse(reasonPhrase, deferral, callback);
        }

        public virtual void SetResponse(IResponse response, CoreWebView2Deferral deferral, Action<IResponse, CoreWebView2Deferral> callback)
        {
            _resourceRequestHandler?.SetResponse(response, deferral, callback);
        }

        public virtual void AddHostObject(string name, object rawObject)
        {
            _resourceRequestHandler?.AddHostObject(name, rawObject);
        }

        public virtual void RemoveHostObject(string name)
        {
            _resourceRequestHandler?.RemoveHostObject(name);
        }

        public virtual void RemoveAllHostObjects()
        {
            _resourceRequestHandler?.RemoveAllHostObjects();
        }

        private void BrowserWindow_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs eventArgs)
        {
            if (eventArgs != null)
            {
                HandleWebMessageReceived(eventArgs);
            }
        }

        private void BrowserWindow_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs eventArgs)
        {
            HandleResourceRequest(eventArgs);
        }

        #region Bootstrap
        protected virtual void Bootstrap()
        {
        }

        #endregion Bootstrap

        #region Creation Options
        private string BrowserExecutableFolder
        {
            get
            {
                if (CreationProperties == null)
                {
                    return _config?.WebView2CreationOptions?.BrowserExecutableFolder;
                }

                return CreationProperties.BrowserExecutableFolder;
            }
        }

        private string UserDataFolder
        {
            get
            {
                if (CreationProperties == null)
                {
                    return _config?.WebView2CreationOptions?.UserDataFolder;
                }

                return CreationProperties.UserDataFolder;
            }
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (CoreWebView2 != null)
            {
                CoreWebView2.WebMessageReceived -= new EventHandler<CoreWebView2WebMessageReceivedEventArgs>(BrowserWindow_WebMessageReceived);
                CoreWebView2.WebResourceRequested -= new EventHandler<CoreWebView2WebResourceRequestedEventArgs>(BrowserWindow_WebResourceRequested);
            }

            base.Dispose(disposing);
            _disposed = true;
        }

        #endregion
    }
}
