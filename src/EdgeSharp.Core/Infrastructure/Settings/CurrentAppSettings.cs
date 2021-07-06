// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Defaults;

namespace EdgeSharp.Core.Infrastructure
{
    public class CurrentAppSettings : AppUser
    {
        private IAppSettings appSettings;

        public override IAppSettings Properties
        {
            get
            {
                if (appSettings == null)
                {
                    appSettings = new AppSettings();
                }

                return appSettings;
            }
            set
            {
                appSettings = value;
            }
        }

    }
}
