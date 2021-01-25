using Newtonsoft.Json;

namespace Sora.Server.Params.GoApiParams
{
    /// <summary>
    /// 设置群名参数
    /// </summary>
    internal struct SetGroupNameParams
    {
        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        internal long Gid { get; set; }

        /// <summary>
        /// 群名
        /// </summary>
        [JsonProperty(PropertyName = "group_name")]
        internal string GroupName { get; set; }
    }
}
