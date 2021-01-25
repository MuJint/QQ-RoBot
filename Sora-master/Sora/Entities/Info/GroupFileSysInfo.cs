using Newtonsoft.Json;

namespace Sora.Entities.Info
{
    /// <summary>
    /// 群文件系统信息
    /// </summary>
    public struct GroupFileSysInfo
    {
        /// <summary>
        /// 文件总数
        /// </summary>
        [JsonProperty(PropertyName = "file_count")]
        public int FileCount { get; internal set; }

        /// <summary>
        /// 文件数量上限
        /// </summary>
        [JsonProperty(PropertyName = "limit_count")]
        public int FileLimit { get; internal set; }

        /// <summary>
        /// 已使用空间(Byte)
        /// </summary>
        [JsonProperty(PropertyName = "used_space")]
        public long UsedSpace { get; internal set; }

        /// <summary>
        /// 总空间(Byte)
        /// </summary>
        [JsonProperty(PropertyName = "total_space")]
        public long TotalSpace { get; internal set; }
    }
}
