using Newtonsoft.Json;
using Sora.Server.Converter;

namespace Sora.Entities.CQCodes.CQCodeModel
{
    /// <summary>
    /// 图片
    /// </summary>
    public struct Image
    {
        #region 属性
        /// <summary>
        /// 文件名/绝对路径/URL/base64
        /// </summary>
        [JsonProperty(PropertyName = "file")]
        public string ImgFile { get; internal set; }

        /// <summary>
        /// 图片类型
        /// </summary>
        [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
        public string ImgType { get; internal set; }

        /// <summary>
        /// 图片链接
        /// </summary>
        [JsonProperty(PropertyName = "url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; internal set; }

        /// <summary>
        /// 只在通过网络 URL 发送时有效，表示是否使用已缓存的文件
        /// </summary>
        [JsonConverter(typeof(StringConverter))]
        [JsonProperty(PropertyName = "cache", NullValueHandling = NullValueHandling.Ignore)]
        public int? UseCache { get; internal set; }

        /// <summary>
        /// 只在通过网络 URL 发送时有效，表示是否通过代理下载文件
        /// </summary>
        [JsonConverter(typeof(StringConverter))]
        [JsonProperty(PropertyName = "proxy", NullValueHandling = NullValueHandling.Ignore)]
        public int? UseProxy { get; internal set; } 

        /// <summary>
        /// 只在通过网络 URL 发送时有效（s）
        /// </summary>
        [JsonConverter(typeof(StringConverter))]
        [JsonProperty(PropertyName = "timeout", NullValueHandling = NullValueHandling.Ignore)]
        public int? Timeout { get; internal set; }
        #endregion
    }
}
