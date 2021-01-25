using Newtonsoft.Json;

namespace Sora.Server.Params.ApiParams
{
    /// <summary>
    /// 群踢人参数
    /// </summary>
    internal struct SetGroupKickParams
    {
        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        internal long Gid { get; set; }

        /// <summary>
        /// 成员UID
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        internal long Uid { get; set; }

        /// <summary>
        /// 拒绝此人的加群请求
        /// </summary>
        [JsonProperty(PropertyName = "reject_add_request")]
        internal bool RejectAddRequest { get; set; }
    }
}
