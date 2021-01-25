using System;
using Newtonsoft.Json;
using Sora.Tool;

namespace Sora.Entities.Info
{
    /// <summary>
    /// 群文件信息
    /// </summary>
    public struct GroupFileInfo
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        [JsonProperty(PropertyName = "file_id")]
        public string Id { get; internal set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [JsonProperty(PropertyName = "file_name")]
        public string Name { get; internal set; }

        /// <summary>
        /// 文件类型ID
        /// </summary>
        [JsonProperty(PropertyName = "busid")]
        public int BusId { get; internal set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [JsonProperty(PropertyName = "file_size")]
        public long Size { get; internal set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        [JsonIgnore]
        public DateTime UploadTime { get; private set; }

        [JsonProperty(PropertyName = "upload_time")]
        private long UploadTime_
        {
            get => Utils.DateTimeToTimeStamp(UploadTime);
            set => UploadTime = Utils.TimeStampToDateTime(value);
        }

        /// <summary>
        /// <para>过期时间</para>
        /// <para>永久文件为0</para>
        /// </summary>
        [JsonIgnore]
        public DateTime DeadTime { get; private set; }

        [JsonProperty(PropertyName = "dead_time")]
        private long DeadTime_
        {
            get => Utils.DateTimeToTimeStamp(DeadTime);
            set => DeadTime = Utils.TimeStampToDateTime(value);
        }

        /// <summary>
        /// 修改时间
        /// </summary>
        [JsonIgnore]
        public DateTime ModifyTime { get; private set; }

        [JsonProperty(PropertyName = "modify_time")]
        private long ModifyTime_
        {
            get => Utils.DateTimeToTimeStamp(ModifyTime);
            set => ModifyTime = Utils.TimeStampToDateTime(value);
        }

        /// <summary>
        /// 下载次数
        /// </summary>
        [JsonProperty(PropertyName = "download_times")]
        public int DownloadCount { get; internal set; }

        /// <summary>
        /// 上传者UID
        /// </summary>
        [JsonProperty(PropertyName = "uploader")]
        public long UploadUserId { get; internal set; }

        /// <summary>
        /// 上传者名
        /// </summary>
        [JsonProperty(PropertyName = "uploader_name")]
        public string UploadUserName { get; internal set; }
    }
}
