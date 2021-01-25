using Newtonsoft.Json;
using Sora.Enumeration.EventParamsType;
using Sora.Server.Converter;

namespace Sora.Server.OnebotEvent.NoticeEvent
{
    /// <summary>
    /// 群禁言事件
    /// </summary>
    internal sealed class ApiGroupMuteEventArgs : BaseNoticeEventArgs
    {
        /// <summary>
        /// 事件子类型
        /// </summary>
        [JsonConverter(typeof(EnumDescriptionConverter))]
        [JsonProperty(PropertyName = "sub_type")]
        internal MuteActionType ActionType { get; set; }

        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        internal long GroupId { get; set; }

        /// <summary>
        /// 操作者 UID
        /// </summary>
        [JsonProperty(PropertyName = "operator_id")]
        internal long OperatorId { get; set; }

        /// <summary>
        /// 禁言时长(s)
        /// </summary>
        [JsonProperty(PropertyName = "duration")]
        internal long Duration { get; set; }
    }
}
