// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using System;
using System.Drawing;
using System.Threading.Tasks;
using EdgeSharp.Core;
using EdgeSharp.Core.Network;
using EdgeSharp.Core.Configuration;
using EdgeSharp.Core.Infrastructure;

namespace EdgeSharp.Browser
{
    public partial class BrowserWindow : IWebView2, IBrowserWindow, IDisposable
    {
        protected IConfiguration _config;
        protected INativeHost _nativeHost;
        protected IResourceRequestHandler _resourceRequestHandler;
        protected IntPtr _hostHandle;
        private CoreWebView2 _coreWebView2;
        protected CoreWebView2Controller _coreWebView2Controller;
        protected CoreWebView2Environment _coreWebView2Environment;

        private static readonly Uri _aboutBlank = new Uri("about:blank");
        private double _zoomFactor = 1.0;
        private Color _defaultBackgroundColor = Color.White;
        private Uri _source = _aboutBlank;
        private bool _sourceChangingByCore;
        private bool _isInitialized;
        private string _browserVersion;

        public virtual void Initialize()
        {
            WebView2Loader.Load();

            _config = ServiceLocator.Current.GetInstance<IConfiguration>();
            _nativeHost = ServiceLocator.Current.GetInstance<INativeHost>();
            _resourceRequestHandler = ServiceLocator.Current.GetInstance<IResourceRequestHandler>();
        }

        public virtual void InitCoreWebView2(IntPtr hostHandle)
        {
            var asynTask = new Task(async () =>
            {
                await InitCoreWebView2Async(hostHandle);
            });

            asynTask.RunSynchronously();
        }

        public async Task InitCoreWebView2Async(IntPtr hostHandle)
        {
            _hostHandle = hostHandle;
            BrowserWindow webView2 = this;
            var creationOption = _config?.WebView2CreationOptions ?? new WebView2CreationOptions();

            try
            {
                webView2._coreWebView2Environment = await CoreWebView2Environment.CreateAsync(creationOption.BrowserExecutableFolder,
                                                                                              creationOption.UserDataFolder,
                                                                                              webView2._resourceRequestHandler.EnvironmentOptions);

                CoreWebView2Controller view2ControllerAsync = await webView2._coreWebView2Environment.CreateCoreWebView2ControllerAsync(_hostHandle);

                webView2._browserVersion = webView2._coreWebView2Environment.BrowserVersionString;
                webView2._config.RuntimeVersion = webView2._browserVersion;

                webView2._coreWebView2Controller = view2ControllerAsync;
                webView2._coreWebView2Controller.ZoomFactor = webView2._zoomFactor;
                webView2._coreWebView2Controller.DefaultBackgroundColor = webView2._defaultBackgroundColor;
                webView2._coreWebView2Controller.Bounds = GetBounds(_config.BrowserBounds.Width, _config.BrowserBounds.Height);

                webView2._coreWebView2 = webView2._coreWebView2Controller.CoreWebView2;
                webView2.CoreWebView2.SourceChanged += new EventHandler<CoreWebView2SourceChangedEventArgs>(webView2.BrowserWindow_SourceChanged);
                webView2.CoreWebView2.WebMessageReceived += new EventHandler<CoreWebView2WebMessageReceivedEventArgs>(webView2.BrowserWindow_WebMessageReceived);

                webView2.CoreWebView2.AddWebResourceRequestedFilter(creationOption.UriFilter, creationOption.ResourceContext);

                webView2.CoreWebView2.WebResourceRequested += new EventHandler<CoreWebView2WebResourceRequestedEventArgs>(webView2.BrowserWindow_WebResourceRequested);

                #region Settings
                webView2.CoreWebView2.Settings.IsScriptEnabled = creationOption.IsScriptEnabled;
                webView2.CoreWebView2.Settings.IsWebMessageEnabled = creationOption.IsWebMessageEnabled;
                webView2.CoreWebView2.Settings.IsZoomControlEnabled = creationOption.IsZoomControlEnabled;
                webView2.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = creationOption.IsBuiltInErrorPageEnabled;
                webView2.CoreWebView2.Settings.IsStatusBarEnabled = creationOption.IsStatusBarEnabled;

                webView2.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = creationOption.AreDefaultScriptDialogsEnabled;
                webView2.CoreWebView2.Settings.AreDevToolsEnabled = creationOption.AreDevToolsEnabled;
                webView2.CoreWebView2.Settings.AreDefaultContextMenusEnabled = creationOption.AreDefaultContextMenusEnabled;
                webView2.CoreWebView2.Settings.AreHostObjectsAllowed = creationOption.AreHostObjectsAllowed;

                try
                {
                    // If not added yet
                    webView2.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = creationOption.AreBrowserAcceleratorKeysEnabled;
                }
                catch { }


                if (!string.IsNullOrWhiteSpace(creationOption.UserAgent))
                {
                    webView2.CoreWebView2.Settings.UserAgent = creationOption.UserAgent;
                }

                #endregion Settings

                webView2.OnInitializationCompleted((object)webView2.CoreWebView2, new CoreWebView2InitializationCompletedEventArgs());
                webView2.CoreWebView2.Navigate(webView2._source.AbsoluteUri);

                _isInitialized = true;
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
                throw;
            }
        }

        public virtual void Resize(int width, int height)
        {
            if (_isInitialized && _coreWebView2Controller != null)
            {
                _coreWebView2Controller.Bounds = GetBounds(width, height);
            }
        }

        public CoreWebView2 CoreWebView2 => _coreWebView2;

        public CoreWebView2EnvironmentOptions EnvironmentOptions
        {
            get { return _resourceRequestHandler.EnvironmentOptions; }
            set { _resourceRequestHandler.EnvironmentOptions = value; }
        }

        public double ZoomFactor
        {
            get
            {
                if (_coreWebView2Controller != null)
                    return _coreWebView2Controller.ZoomFactor;
                return _zoomFactor;
            }
            set
            {
                if (_coreWebView2Controller != null)
                    _coreWebView2Controller.ZoomFactor = value;
                else
                    _zoomFactor = value;
            }
        }

        public Uri Source
        {
            get
            {
                return _source;
            }
            set
            {
                int? nullable;
                if ((object)value == null)
                {
                    nullable = new int?();
                }
                else
                {
                    string absoluteUri = value.AbsoluteUri;
                    nullable = absoluteUri != null ? new int?(absoluteUri.Length) : new int?();
                }
                if (nullable.GetValueOrDefault() <= 0)
                {
                    return;
                }

                _source = value;
                if (!_isInitialized)
                {
                    return;
                }
                else
                {
                    if (_sourceChangingByCore)
                    {
                        return;
                    }

                    BrowserWindow webView2 = this;
                    webView2.VerifyInitializedGuard();
                    webView2.CoreWebView2.Navigate(_source.AbsoluteUri);
                }
            }
        }

        public Color DefaultBackgroundColor
        {
            get
            {
                if (_coreWebView2Controller != null)
                    return _coreWebView2Controller.DefaultBackgroundColor;
                return _defaultBackgroundColor;
            }
            set
            {
                if (_coreWebView2Controller != null)
                    _coreWebView2Controller.DefaultBackgroundColor = value;
                else
                    _defaultBackgroundColor = value;
            }
        }

        public bool CanGoForward
        {
            get
            {
                if (CoreWebView2 == null)
                    return false;
                return CoreWebView2.CanGoForward;
            }
        }

        public bool CanGoBack
        {
            get { return (CoreWebView2 == null) ? false : CoreWebView2.CanGoBack; }
        }

        public async Task<string> ExecuteScriptAsync(string script)
        {
            BrowserWindow webView2 = this;
            webView2.VerifyInitializedGuard();
            return await webView2.CoreWebView2.ExecuteScriptAsync(script);
        }

        public void Reload()
        {
            VerifyInitializedGuard();

            try
            {
                // Single Page Application may have changed the start url, ensure that is not the case.
                if (_isInitialized && Source.AbsoluteUri.IsValidSourceUrl(_config.StartUrl))
                {
                    CoreWebView2.Reload();
                }
                else
                {
                    Source = new Uri(_config.StartUrl);
                }
            }
            catch
            {
                CoreWebView2.Reload();
            }
        }

        public void GoForward()
        {
            CoreWebView2?.GoForward();
        }

        public void GoBack()
        {
            CoreWebView2?.GoBack();
        }

        public void NavigateToString(string htmlContent)
        {
            VerifyInitializedGuard();
            CoreWebView2.NavigateToString(htmlContent);
        }

        public void Stop()
        {
            if (CoreWebView2 == null)
                return;
            CoreWebView2.Stop();
        }

        protected virtual void OnInitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs eventArgs)
        {
            Initialize(sender as CoreWebView2);
        }

        public void Initialize(CoreWebView2 coreWebView2)
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

        public virtual void Exit()
        {
            _nativeHost?.Exit();
        }

        private Rectangle GetBounds(int width, int height)
        {
            int gripSize = _config.GetBorderlessWindowGripSize();
            var left = gripSize;
            var top = gripSize;
            width = width - (2 * gripSize);
            height = height - (2 * gripSize);
            return new Rectangle(left, top, width, height);
        }


        private void BrowserWindow_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs eventArgs)
        {
            _sourceChangingByCore = true;
            Source = new Uri(CoreWebView2.Source);
            _sourceChangingByCore = false;
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

        private void VerifyInitializedGuard()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("The instance of $CoreWebView2 is uninitialized and unable to complete this operation. See InitializeAsync.");
        }
    }
}