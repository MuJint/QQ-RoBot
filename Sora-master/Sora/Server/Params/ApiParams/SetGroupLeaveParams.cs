using Newtonsoft.Json;

namespace Sora.Server.Params.ApiParams
{
    /// <summary>
    /// 群退出请求参数
    /// </summary>
    internal struct SetGroupLeaveParams
    {
        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        internal long Gid { get; set; }

        /// <summary>
        /// 是否解散
        /// 仅在bot为群主时有效
        /// </summary>
        [JsonProperty(PropertyName = "is_dismiss")]
        internal bool Dismiss { get; set; }
    }
}
