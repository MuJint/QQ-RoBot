using Newtonsoft.Json;

namespace Sora.Server.Params.ApiParams
{
    /// <summary>
    /// 设置群成员头衔参数
    /// </summary>
    internal struct SetGroupSpecialTitleParams
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
        /// 专属头衔
        /// 空字符串表示删除
        /// </summary>
        [JsonProperty(PropertyName = "special_title")]
        internal string Title { get; set; }

        /// <summary>
        /// 专属头衔有效期(秒)
        /// 无效字段
        /// </summary>
        [JsonProperty(PropertyName = "duration")]
        internal int Duration { get; set; }
    }
}
