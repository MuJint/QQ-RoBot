using Newtonsoft.Json;

namespace Sora.Server.Params.GoApiParams
{
    /// <summary>
    /// 获取合并转发消息
    /// </summary>
    internal struct GetForwardParams
    {
        /// <summary>
        /// 消息id
        /// </summary>
        [JsonProperty(PropertyName = "message_id")]
        internal string MessageId { get; set; }
    }
}
