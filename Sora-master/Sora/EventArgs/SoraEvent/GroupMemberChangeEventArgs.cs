using System;
using Sora.Server.OnebotEvent.NoticeEvent;
using Sora.Entities;
using Sora.Enumeration.EventParamsType;

namespace Sora.EventArgs.SoraEvent
{
    /// <summary>
    /// 群成员数量变更事件参数
    /// </summary>
    public sealed class GroupMemberChangeEventArgs : BaseSoraEventArgs
    {
        #region 属性
        /// <summary>
        /// 变更成员
        /// </summary>
        public User ChangedUser { get; private set; }

        /// <summary>
        /// 执行者
        /// </summary>
        public User Operator { get; private set; }

        /// <summary>
        /// 消息源群
        /// </summary>
        public Group SourceGroup { get; private set; }

        /// <summary>
        /// 事件子类型
        /// </summary>
        public MemberChangeType SubType { get; set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionGuid">服务器链接标识</param>
        /// <param name="eventName">事件名</param>
        /// <param name="groupMemberChangeArgs">群成员数量变更参数</param>
        internal GroupMemberChangeEventArgs(Guid connectionGuid, string eventName, ApiGroupMemberChangeEventArgs groupMemberChangeArgs) :
            base(connectionGuid, eventName, groupMemberChangeArgs.SelfID, groupMemberChangeArgs.Time)
        {
            this.ChangedUser   = new User(connectionGuid, groupMemberChangeArgs.UserId);
            //执行者和变动成员可能为同一人
            this.Operator = groupMemberChangeArgs.UserId == groupMemberChangeArgs.OperatorId
                ? this.ChangedUser
                : new User(connectionGuid, groupMemberChangeArgs.OperatorId);
            this.SourceGroup = new Group(connectionGuid, groupMemberChangeArgs.GroupId);
            this.SubType     = groupMemberChangeArgs.SubType;
        }
        #endregion
    }
}
