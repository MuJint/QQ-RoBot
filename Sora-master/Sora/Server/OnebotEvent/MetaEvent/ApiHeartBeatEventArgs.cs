using Newtonsoft.Json;

namespace Sora.Server.OnebotEvent.MetaEvent
{
    /// <summary>
    /// 心跳包
    /// </summary>
    internal sealed class ApiHeartBeatEventArgs : BaseMetaEventArgs
    {
        /// <summary>
        /// 状态信息
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        internal object Status { get; set; }

        /// <summary>
        /// 到下次心跳的间隔，单位毫秒
        /// </summary>
        [JsonProperty(PropertyName = "interval")]
        internal long Interval { get; set; }
    }
}
