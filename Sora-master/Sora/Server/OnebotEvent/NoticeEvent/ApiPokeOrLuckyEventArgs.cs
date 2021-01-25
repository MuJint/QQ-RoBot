using Newtonsoft.Json;

namespace Sora.Server.OnebotEvent.NoticeEvent
{
    /// <summary>
    /// 戳一戳或运气王事件
    /// </summary>
    internal sealed class ApiPokeOrLuckyEventArgs : BaseNotifyEventArgs
    {
        /// <summary>
        /// 目标UID
        /// </summary>
        [JsonProperty(PropertyName = "target_id")]
        internal long TargetId { get; set; }
    }
}
