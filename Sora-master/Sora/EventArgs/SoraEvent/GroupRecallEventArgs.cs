using System;
using Sora.Server.OnebotEvent.NoticeEvent;
using Sora.Entities;

namespace Sora.EventArgs.SoraEvent
{
    /// <summary>
    /// 群消息撤回事件参数
    /// </summary>
    public sealed class GroupRecallEventArgs : BaseSoraEventArgs
    {
        #region 属性
        /// <summary>
        /// 消息发送者
        /// </summary>
        public User MessageSender { get; private set; }

        /// <summary>
        /// 撤回执行者
        /// </summary>
        public User Operator { get; private set; }

        /// <summary>
        /// 消息源群
        /// </summary>
        public Group SourceGroup { get; private set; }

        /// <summary>
        /// 被撤消息ID
        /// </summary>
        public int MessageId { get; private set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionGuid">服务器链接标识</param>
        /// <param name="eventName">事件名</param>
        /// <param name="groupRecallArgs">群聊撤回事件参数</param>
        internal GroupRecallEventArgs(Guid connectionGuid, string eventName, ApiGroupRecallEventArgs groupRecallArgs) :
            base(connectionGuid, eventName, groupRecallArgs.SelfID, groupRecallArgs.Time)
        {
            this.MessageSender = new User(connectionGuid, groupRecallArgs.UserId);
            //执行者和发送者可能是同一人
            this.Operator = groupRecallArgs.UserId == groupRecallArgs.OperatorId
                ? this.MessageSender
                : new User(connectionGuid, groupRecallArgs.OperatorId);
            this.SourceGroup = new Group(connectionGuid, groupRecallArgs.GroupId);
            this.MessageId   = groupRecallArgs.MessageId;
        }
        #endregion
    }
}
