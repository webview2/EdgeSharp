// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using EdgeSharp.Core.Infrastructure;
using System;
using System.Runtime.InteropServices;

namespace EdgeSharp.Core
{
    /// <summary>EdgeSharp default HostObject.</summary>
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class RequestHandlerServer
    {
        private Delegate _sendRequestCallback;

        public RequestHandlerServer()
        {
            _sendRequestCallback = null;
        }

        public void RegisterSendCallback(IntPtr callback)
        {
            try
            {
                _sendRequestCallback = Marshal.GetDelegateForFunctionPointer(callback, typeof(HostObjectCommon.SendRequestDelegate));
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }

        public string Send(string url, object request = null)
        {
            if (string.IsNullOrWhiteSpace(url) || _sendRequestCallback == null)
            {
                return null;
            }

            object[] args = { url, request };
            var sendResponse = _sendRequestCallback.DynamicInvoke(args);
            return sendResponse?.ToString();
        }
    }
}