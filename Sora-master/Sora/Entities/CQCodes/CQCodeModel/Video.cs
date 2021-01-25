using System;
using Newtonsoft.Json;
using Sora.Server.Converter;

namespace Sora.Entities.CQCodes.CQCodeModel
{
    /// <summary>
    /// 短视频
    /// </summary>
    [Obsolete]
    public struct Video
    {
        #region 属性
        /// <summary>
        /// 视频文件名
        /// </summary>
        [JsonProperty(PropertyName = "file")]
        public string VideoFile { get; internal set; }

        /// <summary>
        /// 视频 URL
        /// </summary>
        [JsonProperty(PropertyName = "url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; internal set; }

        /// <summary>
        /// 是否使用已缓存的文件
        /// </summary>
        [JsonConverter(typeof(StringConverter))]
        [JsonProperty(PropertyName = "cache", NullValueHandling = NullValueHandling.Ignore)]
        public int? Cache { get; internal set; }

        /// <summary>
        /// 是否使用已缓存的文件
        /// </summary>
        [JsonConverter(typeof(StringConverter))]
        [JsonProperty(PropertyName = "proxy", NullValueHandling = NullValueHandling.Ignore)]
        public int? Proxy { get; internal set; }

        /// <summary>
        /// 是否使用已缓存的文件
        /// </summary>
        [JsonConverter(typeof(StringConverter))]
        [JsonProperty(PropertyName = "timeout", NullValueHandling = NullValueHandling.Ignore)]
        public int? Timeout { get; internal set; }
        #endregion
    }
}
