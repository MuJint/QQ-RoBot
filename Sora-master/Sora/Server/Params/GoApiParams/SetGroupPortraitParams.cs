using Newtonsoft.Json;

namespace Sora.Server.Params.GoApiParams
{
    /// <summary>
    /// 设置群头像参数
    /// </summary>
    internal struct SetGroupPortraitParams
    {
        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        internal long Gid { get; set; }

        /// <summary>
        /// 图片文件
        /// </summary>
        [JsonProperty(PropertyName = "file")]
        internal string ImageFile { get; set; }

        /// <summary>
        /// 是否使用已缓存的文件
        /// </summary>
        [JsonProperty(PropertyName = "cache")]
        internal int UseCache { get; set; }
    }
}
