using Newtonsoft.Json;

namespace Sora.Server.Params.ApiParams
{
    /// <summary>
    /// 群组全员禁言参数
    /// </summary>
    internal struct SetGroupWholeBanParams
    {
        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        internal long Gid { get; set; }

        /// <summary>
        /// 是否禁言
        /// </summary>
        [JsonProperty(PropertyName = "enable")]
        internal bool Enable { get; set; }
    }
}
