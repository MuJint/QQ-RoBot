using System;
using Sora.Server.OnebotEvent.NoticeEvent;
using Sora.Entities;

namespace Sora.EventArgs.SoraEvent
{
    /// <summary>
    /// 好友添加事件参数
    /// </summary>
    public sealed class FriendAddEventArgs : BaseSoraEventArgs
    {
        #region 属性
        /// <summary>
        /// 新好友
        /// </summary>
        public User NewFriend { get; private set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionGuid">服务器链接标识</param>
        /// <param name="eventName">事件名</param>
        /// <param name="friendAddArgs">好友添加事件参数</param>
        internal FriendAddEventArgs(Guid connectionGuid, string eventName, ApiFriendAddEventArgs friendAddArgs) :
            base(connectionGuid, eventName, friendAddArgs.SelfID, friendAddArgs.Time)
        {
            this.NewFriend = new User(connectionGuid, friendAddArgs.UserId);
        }
        #endregion
    }
}
