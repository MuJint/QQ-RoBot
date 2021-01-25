using System.Collections.Generic;
using Newtonsoft.Json;
using Sora.Server.ApiMessageParse;
using Sora.Server.Converter;
using Sora.Enumeration.EventParamsType;

namespace Sora.Server.Params.ApiParams
{
    /// <summary>
    /// 发送消息调用参数
    /// </summary>
    internal struct SendMessageParams
    {
        /// <summary>
        /// 消息类型 群/私聊
        /// </summary>
        [JsonConverter(typeof(EnumDescriptionConverter))]
        [JsonProperty(PropertyName = "message_type")]
        internal MessageType MessageType { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [JsonProperty(PropertyName = "user_id", NullValueHandling = NullValueHandling.Ignore)]
        internal long? UserId { get; set; }

        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id", NullValueHandling = NullValueHandling.Ignore)]
        internal long? GroupId { get; set; }

        /// <summary>
        /// 消息段数组
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        internal List<ApiMessage> Message { get; set; }

        /// <summary>
        /// 是否忽略CQ码
        /// </summary>
        [JsonProperty(PropertyName = "auto_escape")]
        internal bool AutoEscape { get; set; }
    }
}
