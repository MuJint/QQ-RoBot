using Newtonsoft.Json;

namespace Sora.Entities.Info
{
    /// <summary>
    /// 离线文件信息
    /// </summary>
    public class OfflineFileInfo
    {
        /// <summary>
        /// 文件名
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; internal set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [JsonProperty(PropertyName = "size")]
        public long Size { get; internal set; }

        /// <summary>
        /// 文件链接
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; internal set; }
    }
}
