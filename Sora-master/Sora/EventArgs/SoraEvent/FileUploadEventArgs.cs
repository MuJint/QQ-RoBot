using System;
using Sora.Server.OnebotEvent.NoticeEvent;
using Sora.Entities;
using Sora.Entities.Info;

namespace Sora.EventArgs.SoraEvent
{
    /// <summary>
    /// 群文件上传事件参数
    /// </summary>
    public sealed class FileUploadEventArgs : BaseSoraEventArgs
    {
        #region 属性
        /// <summary>
        /// 消息源群
        /// </summary>
        public Group SourceGroup { get; private set; }

        /// <summary>
        /// 上传者
        /// </summary>
        public User Sender { get; private set; }

        /// <summary>
        /// 上传文件的信息
        /// </summary>
        public UploadFileInfo FileInfo { get; private set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionGuid">服务器链接标识</param>
        /// <param name="eventName">事件名</param>
        /// <param name="fileUploadArgs">文件上传事件参数</param>
        internal FileUploadEventArgs(Guid connectionGuid, string eventName, ApiFileUploadEventArgs fileUploadArgs) :
            base(connectionGuid, eventName, fileUploadArgs.SelfID, fileUploadArgs.Time)
        {
            this.SourceGroup = new Group(connectionGuid, fileUploadArgs.GroupId);
            this.Sender      = new User(connectionGuid, fileUploadArgs.UserId);
            this.FileInfo    = fileUploadArgs.Upload;
        }
        #endregion
    }
}
