using System;
using Sora.Entities;
using Sora.Entities.Info;
using Sora.Server.OnebotEvent.NoticeEvent;

namespace Sora.EventArgs.SoraEvent
{
    /// <summary>
    /// 接收到离线文件事件参数
    /// </summary>
    public class OfflineFileEventArgs : BaseSoraEventArgs
    {
        #region 属性
        /// <summary>
        /// 文件发送者
        /// </summary>
        public User Sender { get; private set; }

        /// <summary>
        /// 离线文件信息
        /// </summary>
        public OfflineFileInfo OfflineFileInfo { get; private set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionGuid">服务器链接标识</param>
        /// <param name="eventName">事件名</param>
        /// <param name="offlineFileArgs">离线文件事件参数</param>
        internal OfflineFileEventArgs(Guid connectionGuid, string eventName, ApiOfflineFileEventArgs offlineFileArgs) :
            base(connectionGuid, eventName, offlineFileArgs.SelfID, offlineFileArgs.Time)
        {
            this.Sender          = new User(connectionGuid, offlineFileArgs.UserId);
            this.OfflineFileInfo = offlineFileArgs.Info;
        }
        #endregion
    }
}
