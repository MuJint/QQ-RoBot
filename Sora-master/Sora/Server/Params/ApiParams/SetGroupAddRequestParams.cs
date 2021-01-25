using Newtonsoft.Json;
using Sora.Enumeration.EventParamsType;
using Sora.Server.Converter;

namespace Sora.Server.Params.ApiParams
{
    /// <summary>
    /// 处理加群请求/邀请参数
    /// </summary>
    internal struct SetGroupAddRequestParams
    {
        /// <summary>
        /// 请求flag
        /// </summary>
        [JsonProperty(PropertyName = "flag")]
        internal string Flag { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        [JsonConverter(typeof(EnumDescriptionConverter))]
        [JsonProperty(PropertyName = "sub_type")]
        internal GroupRequestType GroupRequestType { get; set; }

        /// <summary>
        /// 是否同意
        /// </summary>
        [JsonProperty(PropertyName = "approve")]
        internal bool Approve { get; set; }

        /// <summary>
        /// 拒绝理由（仅在拒绝时有效）
        /// </summary>
        [JsonProperty(PropertyName = "reason")]
        internal string Reason { set; get; }
    }
}
