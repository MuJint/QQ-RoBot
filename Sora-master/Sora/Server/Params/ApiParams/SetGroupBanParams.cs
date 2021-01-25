using Newtonsoft.Json;

namespace Sora.Server.Params.ApiParams
{
    /// <summary>
    /// 群组单人禁言参数
    /// </summary>
    internal struct SetGroupBanParams
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
        /// 禁言时长(秒)
        /// 0为取消
        /// </summary>
        [JsonProperty(PropertyName = "duration")]
        internal long Duration { get; set; }
    }
}
