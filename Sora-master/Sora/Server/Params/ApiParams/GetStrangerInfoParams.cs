using Newtonsoft.Json;

namespace Sora.Server.Params.ApiParams
{
    /// <summary>
    /// 获取陌生人信息请求参数
    /// </summary>
    internal struct GetStrangerInfoParams
    {
        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        internal long Uid { get;set; }

        /// <summary>
        /// 是否不使用缓存
        /// </summary>
        [JsonProperty(PropertyName = "no_cache")]
        internal bool NoCache { get; set; }
    }
}
