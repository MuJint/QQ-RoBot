using Newtonsoft.Json;

namespace Sora.Entities.CQCodes.CQCodeModel
{
    /// <summary>
    /// 自定义音乐分享
    /// </summary>
    public struct CustomMusic
    {
        [JsonProperty(PropertyName = "type")]
        internal string ShareType;

        /// <summary>
        /// 跳转URL
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; internal set; }

        /// <summary>
        /// 音乐URL
        /// </summary>
        [JsonProperty(PropertyName = "audio")]
        public string MusicUrl { get; internal set; }

        /// <summary>
        /// 标题
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; internal set; }

        /// <summary>
        /// 内容描述[可选]
        /// </summary>
        [JsonProperty(PropertyName = "content", NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; internal set; }

        /// <summary>
        /// 分享内容图片[可选]
        /// </summary>
        [JsonProperty(PropertyName = "image", NullValueHandling = NullValueHandling.Ignore)]
        public string CoverImageUrl { get; internal set; }
    }
}
