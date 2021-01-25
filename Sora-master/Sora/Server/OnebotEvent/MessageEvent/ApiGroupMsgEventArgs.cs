using Newtonsoft.Json;
using Sora.Entities.Info;

namespace Sora.Server.OnebotEvent.MessageEvent
{
    /// <summary>
    /// 群组消息事件
    /// </summary>
    internal sealed class ApiGroupMsgEventArgs : BaseMessageEventArgs
    {
        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        internal long GroupId { get; set; }

        /// <summary>
        /// 匿名信息
        /// </summary>
        [JsonProperty(PropertyName = "anonymous")]
        internal object Anonymous { get; set; }

        /// <summary>
        /// 发送人信息
        /// </summary>
        [JsonProperty(PropertyName = "sender")]
        internal GroupSenderInfo SenderInfo { get; set; }
    }
}
