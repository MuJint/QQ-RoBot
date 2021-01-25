using Newtonsoft.Json;

namespace Sora.Server.Params.ApiParams
{
    /// <summary>
    /// 处理加好友请求参数
    /// </summary>
    internal struct SetFriendAddRequestParams
    {
        /// <summary>
        /// 请求flag
        /// </summary>
        [JsonProperty(PropertyName = "flag")]
        internal string Flag { get; set; }

        /// <summary>
        /// 是否同意
        /// </summary>
        [JsonProperty(PropertyName = "approve")]
        internal bool Approve { get; set; }

        /// <summary>
        /// 好友备注
        /// </summary>
        [JsonProperty(PropertyName = "remark")]
        internal string Remark { get; set; }
    }
}
