namespace EdgeSharp.Core.Defaults
{
    public partial class DataTransferOptions
    {
        public virtual string ConvertResponseToJson(object response)
        {
            return ConvertObjectToJson(response);
        }
    }
}
