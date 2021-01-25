using Newtonsoft.Json;
using Sora.Entities.Info;

namespace Sora.Server.OnebotEvent.NoticeEvent
{
    /// <summary>
    /// 群文件上传事件
    /// </summary>
    internal sealed class ApiFileUploadEventArgs : BaseNoticeEventArgs
    {
        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        internal long GroupId { get; set; }

        /// <summary>
        /// 上传的文件信息
        /// </summary>
        [JsonProperty(PropertyName = "file")]
        internal UploadFileInfo Upload { get; set; }
    }
}
