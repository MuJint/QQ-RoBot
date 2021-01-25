using System.Collections.Generic;
using Newtonsoft.Json;
using Sora.Server.ApiMessageParse;

namespace Sora.Server.OnebotEvent.MessageEvent
{
    /// <summary>
    /// 消息事件基类
    /// </summary>
    internal abstract class BaseMessageEventArgs : BaseApiEventArgs
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        [JsonProperty(PropertyName = "message_type")]
        internal string MessageType { get; set; }

        /// <summary>
        /// 消息子类型
        /// </summary>
        [JsonProperty(PropertyName = "sub_type")]
        internal string SubType { get; set; }

        /// <summary>
        /// 消息 ID
        /// </summary>
        [JsonProperty(PropertyName = "message_id")]
        internal int MessageId { get; set; }

        /// <summary>
        /// 发送者 QQ 号
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        internal long UserId { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        internal List<ApiMessage> MessageList { get; set; }

        /// <summary>
        /// 原始消息内容
        /// </summary>
        [JsonProperty(PropertyName = "raw_message")]
        internal string RawMessage { get; set; }

        /// <summary>
        /// 字体
        /// </summary>
        [JsonProperty(PropertyName = "font")]
        internal int Font { get; set; }
    }
}
