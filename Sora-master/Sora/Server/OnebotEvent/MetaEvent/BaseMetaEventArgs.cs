using Newtonsoft.Json;

namespace Sora.Server.OnebotEvent.MetaEvent
{
    /// <summary>
    /// 元事件基类
    /// </summary>
    internal abstract class BaseMetaEventArgs : BaseApiEventArgs
    {
        /// <summary>
        /// 元事件类型
        /// </summary>
        [JsonProperty(PropertyName = "meta_event_type")]
        internal string MetaEventType { get; set; }
    }
}
