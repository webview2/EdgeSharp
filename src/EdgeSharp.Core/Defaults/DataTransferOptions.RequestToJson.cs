using EdgeSharp.Core.Infrastructure;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace EdgeSharp.Core.Defaults
{
    public partial class DataTransferOptions
    {
        private const BindingFlags BindFlags = BindingFlags.Instance
                           | BindingFlags.Public
                           | BindingFlags.NonPublic
                           | BindingFlags.Static;

        private const int FallbackMaxBufferSize = 64 * 1024;

        public virtual string ConvertRequestToJson(object request)
        {
            if (request.IsValidJson())
            {
                return request.ToString();
            }

            var outStream = request as Stream;
            if (outStream != null)
            {
                try
                {
                    var iStream = GetIStream(request);
                    if (iStream != null)
                    {
                        byte[] buffer = new byte[] { };
                        int bufferSize = GetBufferSize(request);

                        while (true)
                        {
                            IntPtr bytesRead = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)));
                            
                            int cb = bufferSize;
                            byte[] bufferRead = new byte[cb];
                            int read = 0;
                            try
                            {
                                iStream.Read(bufferRead, cb, bytesRead);
                                read = Marshal.ReadInt32(bytesRead);
                            }
                            finally
                            {
                                Marshal.FreeCoTaskMem(bytesRead);
                            }

                            if (read > 0)
                            {
                                buffer = CombineBuffers(buffer, bufferRead, read);
                                continue;
                            }

                            break;
                        }

                        outStream = new MemoryStream(buffer, 0, (int)buffer.Length);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Instance.Log.LogError(exception);
                }
            }

            // Convert stream to Json
            if (outStream != null)
            {
                using (StreamReader reader = new StreamReader(outStream))
                {
                    return reader.ReadToEnd();
                }
            }

            // Default option
            // We should not get here ..
            return request.ToString();
        }

        private IStream GetIStream(object content)
        {
            var istreamField = content.GetType().GetField("istream", BindFlags);
            if (istreamField != null)
            {
                return istreamField.GetValue(content) as IStream;
            }

            return null;
        }

        private int GetBufferSize(object content)
        {
            var maxBufferSize = (MaxBufferSize > 0) ? MaxBufferSize : FallbackMaxBufferSize;

            try
            {
                var lengthProp = content.GetType().GetProperty("Length", BindFlags);
                if (lengthProp != null)
                {
                    var bufferSize = (long)lengthProp.GetValue(content);
                    if (bufferSize > 0 && bufferSize < (long)int.MaxValue)
                    {
                        return Math.Min((int)bufferSize, maxBufferSize);
                    }
                }
            }
            catch  {}

            return maxBufferSize;
        }

        private byte[] CombineBuffers(byte[] main, byte[] current, int currentSize)
        {
            byte[] bytes = new byte[main.Length + currentSize];
            Buffer.BlockCopy(main, 0, bytes, 0, main.Length);
            Buffer.BlockCopy(current, 0, bytes, main.Length, currentSize);
            return bytes;
        }
    }
}
