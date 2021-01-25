using Newtonsoft.Json;

namespace Sora.Server.OnebotEvent
{
    /// <summary>
    /// OneBot事件基类
    /// </summary>
    internal abstract class BaseApiEventArgs
    {
        /// <summary>
        /// 事件发生的时间戳
        /// </summary>
        [JsonProperty(PropertyName = "time")]
        internal long Time { get; set; }
        /// <summary>
        /// 收到事件的机器人 QQ 号
        /// </summary>
        [JsonProperty(PropertyName = "self_id")]
        internal long SelfID { get; set; }
        /// <summary>
        /// 事件类型
        /// </summary>
        [JsonProperty(PropertyName = "post_type")]
        internal string PostType { get; set; }
    }
}
