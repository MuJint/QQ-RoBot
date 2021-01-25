using Newtonsoft.Json;

namespace Sora.Entities.CQCodes.CQCodeModel
{
    /// <summary>
    /// 链接分享
    /// </summary>
    public struct Share
    {
        #region 属性
        /// <summary>
        /// URL
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; internal set; }

        /// <summary>
        /// 标题
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; internal set; }

        /// <summary>
        /// 可选，内容描述
        /// </summary>
        [JsonProperty(PropertyName = "content",NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; internal set; }

        /// <summary>
        /// 可选，图片 URL
        /// </summary>
        [JsonProperty(PropertyName = "image",NullValueHandling = NullValueHandling.Ignore)]
        public string ImageUrl { get; internal set; }
        #endregion
    }
}
