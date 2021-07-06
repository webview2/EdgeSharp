// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

namespace EdgeSharp.Core.Infrastructure
{
    public abstract class AppUser
    {
        private static AppUser instance;
        public static AppUser App
        {
            get
            {
                if (instance == null)
                {
                    //Ambient Context can't return null, so we assign Local Default
                    instance = new CurrentAppSettings();
                }

                return instance;
            }
            set
            {
                instance = (value == null) ? new CurrentAppSettings() : value;
            }
        }

        public virtual IAppSettings Properties { get; set; }
    }
}

