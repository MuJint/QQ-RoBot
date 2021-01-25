using System;
using Newtonsoft.Json;
using Sora.Tool;

namespace Sora.Entities.Info
{
    /// <summary>
    /// 群文件夹信息
    /// </summary>
    public struct GroupFolderInfo
    {
        /// <summary>
        /// 文件夹ID
        /// </summary>
        [JsonProperty(PropertyName = "folder_id")]
        public string Id { get; internal set; }

        /// <summary>
        /// 文件夹名
        /// </summary>
        [JsonProperty(PropertyName = "folder_name")]
        public string Name { get; internal set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonIgnore]
        public DateTime CreateTime { get; private set; }

        [JsonProperty(PropertyName = "create_time")]
        private long CreateTime_
        {
            get => Utils.DateTimeToTimeStamp(CreateTime);
            set => CreateTime = Utils.TimeStampToDateTime(value);
        }

        /// <summary>
        /// 创建者UID
        /// </summary>
        [JsonProperty(PropertyName = "creator")]
        public long CreatorUserId { get; internal set; }

        /// <summary>
        /// 创建者名
        /// </summary>
        [JsonProperty(PropertyName = "creator_name")]
        public string CreatorUserName { get; internal set; }

        /// <summary>
        /// 子文件数量
        /// </summary>
        [JsonProperty(PropertyName = "total_file_count")]
        public int FileCount { get; internal set; }
    }
}
