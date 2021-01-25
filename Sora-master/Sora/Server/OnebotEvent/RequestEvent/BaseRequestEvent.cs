using Newtonsoft.Json;
using Sora.Enumeration.EventParamsType;
using Sora.Server.Converter;

namespace Sora.Server.OnebotEvent.RequestEvent
{
    /// <summary>
    /// 请求事件基类
    /// </summary>
    internal abstract class BaseRequestEvent : BaseApiEventArgs
    {
        /// <summary>
        /// 请求类型
        /// </summary>
        [JsonProperty(PropertyName = "request_type")]
        [JsonConverter(typeof(EnumDescriptionConverter))]
        internal RequestType RequestType { get; set; }

        /// <summary>
        /// 发送请求的 QQ 号
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        internal long UserId { get; set; }

        /// <summary>
        /// 验证信息
        /// </summary>
        [JsonProperty(PropertyName = "comment")]
        internal string Comment { get; set; }

        /// <summary>
        /// 请求 flag
        /// </summary>
        [JsonProperty(PropertyName = "flag")]
        internal string Flag { get; set; }
    }
}
