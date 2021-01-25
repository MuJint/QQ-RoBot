using Newtonsoft.Json;

namespace Sora.Server.Params.ApiParams
{
    /// <summary>
    /// 获取群成员信息请求参数
    /// </summary>
    internal struct GetGroupInfoParams
    {
        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        internal long Gid { get; set; }

        /// <summary>
        /// 是否不使用缓存
        /// </summary>
        [JsonProperty(PropertyName = "no_cache")]
        internal bool NoCache { get; set; }
    }
}
