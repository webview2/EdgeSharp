// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core;
using EdgeSharp.Core.Borderless;
using EdgeSharp.Core.Configuration;
using EdgeSharp.Core.Infrastructure;
using Microsoft.Web.WebView2.Core;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Shell;

namespace EdgeSharp.Wpf
{
    public partial class BrowserWindow : Window, IBrowserWindow
    {
        protected IConfiguration _config;
        protected IWindowOptions _windowOptions;
        protected BorderlessOption _borderlessOption;
        protected BorderlessController _borderlessController;
        protected IntPtr _handle;

        private Border _webview2ControlContainer;

        #region WebView2
        public WebView2Control WebView2Control { get; private set; }

        public bool CanGoBack => WebView2Control.CanGoBack;

        public bool CanGoForward => WebView2Control.CanGoForward;

        public Uri Source { get => WebView2Control.Source; set => WebView2Control.Source = value; }

        public CoreWebView2 CoreWebView2 => WebView2Control.CoreWebView2;

        public double ZoomFactor { get => WebView2Control.ZoomFactor; set => WebView2Control.ZoomFactor = value; }
        public Color DefaultBackgroundColor { get => WebView2Control.DefaultBackgroundColor; set => WebView2Control.DefaultBackgroundColor = value; }

        #endregion WebView2

        public BrowserWindow()
        {
            Bootstrap();

            _config = ServiceLocator.Current.GetInstance<IConfiguration>();
            _windowOptions = _config?.WindowOptions ?? new WindowOptions();
            _borderlessOption = _windowOptions?.BorderlessOption ?? new BorderlessOption();

            int resizer = _config.GetBorderlessWindowGripSize();

            _webview2ControlContainer = new Border();
            _webview2ControlContainer.HorizontalAlignment = HorizontalAlignment.Stretch;
            _webview2ControlContainer.VerticalAlignment = VerticalAlignment.Stretch;

            WebView2Control = new WebView2Control();
            WebView2Control.Source = new Uri(_config.StartUrl, System.UriKind.Absolute);
            _webview2ControlContainer.Child = WebView2Control;
            _webview2ControlContainer.Margin = new Thickness(resizer);

            Content = _webview2ControlContainer;

           Title =  _config?.WindowOptions?.Title ?? "EdgeSharp Wpf";
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            _handle = new WindowInteropHelper(this).Handle;
            if (source != null)
            {
                _handle = new WindowInteropHelper(this).Handle;

                if (ForceSoftwareRendering)
                {
                    HwndTarget hwndTarget = source.CompositionTarget;
                    hwndTarget.RenderMode = RenderMode.SoftwareOnly;
                }

                if (_windowOptions.Borderless)
                {
                    this.ResizeMode = ResizeMode.CanResizeWithGrip;
                    this.WindowStyle = WindowStyle.None;

                    /* Set Borderless Chrome to this Window */
                    WindowChrome Resizable_BorderLess_Chrome = new WindowChrome();
                    Resizable_BorderLess_Chrome.GlassFrameThickness = new Thickness(0);
                    Resizable_BorderLess_Chrome.CornerRadius = new CornerRadius(0);
                    Resizable_BorderLess_Chrome.CaptionHeight = 5.0;
                    WindowChrome.SetWindowChrome(this, Resizable_BorderLess_Chrome);

                    _borderlessController = new BorderlessController(_handle, _borderlessOption);
                }
     
                source.AddHook(WndProc);
            }
        }

        #region WebView2

        public async Task<string> ExecuteScriptAsync(string script)
        {
            return await WebView2Control?.ExecuteScriptAsync(script);
        }

        public void GoBack()
        {
            WebView2Control?.GoBack();
        }

        public void GoForward()
        {
            WebView2Control?.GoForward();
        }

        public void NavigateToString(string htmlContent)
        {
            WebView2Control?.NavigateToString(htmlContent);
        }

        public void Reload()
        {
            try
            {
                // Single Page Application may have changed the start url, ensure that is not the case.
                if (Source.AbsoluteUri.IsValidSourceUrl(_config.StartUrl))
                {
                    WebView2Control?.Reload();
                }
                else
                {
                    WebView2Control.Source = new Uri(_config.StartUrl);
                }
            }
            catch
            {
                WebView2Control?.Reload();
            }
        }

        public void Stop()
        {
            WebView2Control?.Stop();
        }

        #endregion WebView2

        // https://stackoverflow.com/questions/4951058/software-rendering-mode-wpf
        public bool ForceSoftwareRendering
        {
            get
            {
                int renderingTier = (System.Windows.Media.RenderCapability.Tier >> 16);
                return renderingTier == 0;
            }
        }

        #region Bootstrap

        protected virtual void Bootstrap()
        {
        }

        #endregion


        #region App forced exit.

        public void Exit()
        {
            Application.Current.Shutdown();
        }

        #endregion
    }
}
