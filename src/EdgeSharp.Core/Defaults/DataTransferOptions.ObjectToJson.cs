using EdgeSharp.Core.Infrastructure;
using System;
using System.IO;
using System.Text.Json;

namespace EdgeSharp.Core.Defaults
{
    public partial class DataTransferOptions
    {
        public virtual string ConvertObjectToJson(object value)
        {
            try
            {
                if (value == null)
                {
                    return null;
                }

                var stream = value as Stream;
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream, Encoding))
                    {
                        value = reader.ReadToEnd();
                    }
                }

                if (value.IsValidJson())
                {
                    return value.ToString();
                }

                return JsonSerializer.Serialize(value, Options);
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }

            return value.ToString();
        }
    }
}
