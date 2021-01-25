using Newtonsoft.Json;
using Sora.Server.Converter;

namespace Sora.Entities.CQCodes.CQCodeModel
{
    /// <summary>
    /// 语音消息
    /// </summary>
    public struct Record
    {
        #region 属性
        /// <summary>
        /// 文件名/绝对路径/URL/base64
        /// </summary>
        [JsonProperty(PropertyName = "file")]
        public string RecordFile { get; internal set; }

        /// <summary>
        /// 表示变声
        /// </summary>
        [JsonConverter(typeof(StringConverter))]
        [JsonProperty(PropertyName = "magic")]
        public int? Magic { get; internal set; }

        /// <summary>
        /// 语音 URL
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
        /// 是否使用代理
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
