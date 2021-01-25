using System;
using Sora.Server.OnebotEvent.NoticeEvent;
using Sora.Entities;

namespace Sora.EventArgs.SoraEvent
{
    /// <summary>
    /// 好友消息撤回事件
    /// </summary>
    public sealed class FriendRecallEventArgs : BaseSoraEventArgs
    {
        #region 属性
        /// <summary>
        /// 消息发送者
        /// </summary>
        public User Sender { get; private set; }

        /// <summary>
        /// 被撤回的消息ID
        /// </summary>
        public int MessageId { get; private set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionGuid">服务器链接标识</param>
        /// <param name="eventName">事件名</param>
        /// <param name="friendRecallArgs">私聊消息撤回事件参数</param>
        internal FriendRecallEventArgs(Guid connectionGuid, string eventName, ApiFriendRecallEventArgs friendRecallArgs) :
            base(connectionGuid, eventName, friendRecallArgs.SelfID, friendRecallArgs.Time)
        {
            this.Sender    = new User(connectionGuid, friendRecallArgs.UserId);
            this.MessageId = friendRecallArgs.MessageId;
        }
        #endregion
    }
}
