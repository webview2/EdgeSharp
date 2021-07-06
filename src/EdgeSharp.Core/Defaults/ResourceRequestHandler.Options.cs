// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using Microsoft.Web.WebView2.Core;

namespace EdgeSharp.Core.Defaults
{
    public partial class ResourceRequestHandler
    {
        private const string SPACER = " ";
        private const char QUOTE = '"';
        private const string CMDLINE_ARG_START1 = "--";
        private const string CMDLINE_ARG_START2 = "-";

        /// <inheritdoc />
        public CoreWebView2EnvironmentOptions EnvironmentOptions 
        { 
            get
            {
                if (_environmentOptions == null)
                {
                    _environmentOptions = new CoreWebView2EnvironmentOptions();
                    var creationOption = _config?.WebView2CreationOptions;
                    if (creationOption != null)
                    {
                        _environmentOptions.AdditionalBrowserArguments              = GetAdditionalBrowserArguments(creationOption.AdditionalBrowserArguments);
                        _environmentOptions.AllowSingleSignOnUsingOSPrimaryAccount  = creationOption.AllowSingleSignOnUsingOSPrimaryAccount;

                        if (!string.IsNullOrWhiteSpace(creationOption.Language))
                        {
                            _environmentOptions.Language                            = creationOption.Language;
                        }

                        if (!string.IsNullOrWhiteSpace(creationOption.TargetCompatibleBrowserVersion))
                        {
                            _environmentOptions.TargetCompatibleBrowserVersion = creationOption.TargetCompatibleBrowserVersion;
                        }
                    }
                }
                return _environmentOptions;
            }
            set { _environmentOptions = value;  }
        }
    }
}
