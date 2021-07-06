using EdgeSharp.Core.Infrastructure;
using System.Text;
using System.Text.Json;

namespace EdgeSharp.Core.Defaults
{
    /// <summary>
    /// The default implementation of <see cref="IDataTransferOptions"/>.
    /// </summary>
    public partial class DataTransferOptions : IDataTransferOptions
    {
        const int MAXBUFFERSIZE = 64 * 1024;

        /// <summary>
        /// Initializes a new instance of <see cref="DataTransferOptions"/>.
        /// </summary>
        /// <param name="serializerOptions">The serializer type of <see cref="JsonSerializerOptions"/>. </param>
        public DataTransferOptions(JsonSerializerOptions serializerOptions = null)
        {
            MaxBufferSize = MAXBUFFERSIZE;
            Encoding = Encoding.UTF8;
            SerializerOptions = serializerOptions.SerializerOptions();
        }

        /// <inheritdoc />
        public int MaxBufferSize { get; }
        /// <inheritdoc />
        public Encoding Encoding { get; }
        /// <inheritdoc />
        public object SerializerOptions { get; }
        /// <inheritdoc />
        protected virtual JsonSerializerOptions Options
        {
            get
            {
                return SerializerOptions.SerializerOptions();
            }
        }
    }
}
