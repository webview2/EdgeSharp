// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System.IO;
using System.Reflection;

namespace EdgeSharp.Core
{
    public static class OnDocumentReadyScriptLoader
    {
        private const string PromiseFilePath = "EdgeSharp.Core.postMessagePromise.js";

        public static string PostMessagePromise
        {
            get
            {
                var result = default(string);

                var resourcePath = PromiseFilePath;
                using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath))
                {
                    if (resource != null)
                    {
                        using (var reader = new StreamReader(resource))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }

                return result;
            }
        }
    }
}
