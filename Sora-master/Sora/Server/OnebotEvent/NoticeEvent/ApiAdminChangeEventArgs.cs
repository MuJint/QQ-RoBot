using Newtonsoft.Json;
using Sora.Enumeration.EventParamsType;
using Sora.Server.Converter;

namespace Sora.Server.OnebotEvent.NoticeEvent
{
    /// <summary>
    /// 群管理员变动事件
    /// </summary>
    internal sealed class ApiAdminChangeEventArgs : BaseNoticeEventArgs
    {
        /// <summary>
        /// 事件子类型
        /// </summary>
        [JsonConverter(typeof(EnumDescriptionConverter))]
        [JsonProperty(PropertyName = "sub_type")]
        internal AdminChangeType SubType { get; set; }

        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        internal long GroupId { get; set; }
    }
}
