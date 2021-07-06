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
using System.Windows.Forms;

namespace EdgeSharp.WinForms
{
    public partial class BrowserForm : Form, IBrowserWindow
    {
        protected IConfiguration _config;
        protected IWindowOptions _windowOptions;
        protected BorderlessOption _borderlessOption;
        protected BorderlessController _borderlessController;

        #region WebView2
        public WebView2Control WebView2Control { get; private set; }

        public bool CanGoBack => WebView2Control.CanGoBack;

        public bool CanGoForward => WebView2Control.CanGoForward;

        public Uri Source { get => WebView2Control.Source; set => WebView2Control.Source = value; }

        public CoreWebView2 CoreWebView2 => WebView2Control.CoreWebView2;

        public double ZoomFactor { get => WebView2Control.ZoomFactor; set => WebView2Control.ZoomFactor = value; }
        public Color DefaultBackgroundColor { get => WebView2Control.DefaultBackgroundColor; set => WebView2Control.DefaultBackgroundColor = value; }

        #endregion WebView2

        public BrowserForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code


        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            Bootstrap();

            _config = ServiceLocator.Current.GetInstance<IConfiguration>();
            _windowOptions = _config?.WindowOptions ?? new WindowOptions();
            _borderlessOption = _windowOptions?.BorderlessOption ?? new BorderlessOption();

            int resizer = 0;
            var width = this.Size.Width;
            var height = this.Size.Height;

            // 
            // WebView2Control
            // 
            this.WebView2Control = new WebView2Control();

            if (_windowOptions.Borderless)
            {
                resizer = _config.GetBorderlessWindowGripSize();

                this.WebView2Control.AutoSize = true;
                this.WebView2Control.Anchor = AnchorStyles.Top;
            }
            else
            {
                this.WebView2Control.Dock = DockStyle.Fill;
            }

            this.WebView2Control.Location = new Point(resizer, resizer);
            this.WebView2Control.Size = new Size(width - (2 * resizer), height - (2 * resizer));
            this.WebView2Control.Source = new Uri(_config.StartUrl, System.UriKind.Absolute);
            this.WebView2Control.TabIndex = 0;
            this.WebView2Control.ZoomFactor = 1D;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(width, height);

            this.Controls.Add(this.WebView2Control);
            this.Name = "BrowserForm";
            this.Text = _config?.WindowOptions?.Title ?? "EdgeSharp WinForms";

            ResizeRedraw = true;

            this.ResumeLayout(false);
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (_windowOptions.Borderless)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

                var borderlessOption = _windowOptions.BorderlessOption ?? new BorderlessOption();
                _borderlessController = new BorderlessController(this.Handle, borderlessOption);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            this.SuspendLayout();

            base.OnResize(e);

             var width = this.Size.Width;
            var height = this.Size.Height;
            var resizer = _config.GetBorderlessWindowGripSize();
            WebView2Control.Location = new Point(resizer, resizer);
            WebView2Control.Size = new Size(width - 2 * resizer, height - 2 * resizer);

            this.ResumeLayout(false);
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

        #region Bootstrap

        protected virtual void Bootstrap()
        {
        }

        #endregion

        #region WinForms App forced exit.

        public void Exit()
        {
            Application.Exit();
        }

        #endregion
    }
}
