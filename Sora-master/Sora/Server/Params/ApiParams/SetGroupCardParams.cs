using Newtonsoft.Json;

namespace Sora.Server.Params.ApiParams
{
    /// <summary>
    /// 设置群名片参数
    /// </summary>
    internal struct SetGroupCardParams
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
        /// 群名片内容
        /// 空字符串表示删除
        /// </summary>
        [JsonProperty(PropertyName = "card")]
        internal string Card { get; set; }
    }
}
