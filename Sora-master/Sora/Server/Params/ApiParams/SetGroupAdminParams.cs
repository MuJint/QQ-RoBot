using Newtonsoft.Json;

namespace Sora.Server.Params.ApiParams
{
    /// <summary>
    /// 设置群管理员参数
    /// </summary>
    internal struct SetGroupAdminParams
    {
        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        internal long Gid { get; set; }

        /// <summary>
        /// QQ 号
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        internal long Uid { get; set; }

        /// <summary>
        /// 设置或取消
        /// </summary>
        [JsonProperty(PropertyName = "enable")]
        internal bool Enable { get; set; }
    }
}
