using System;
using Sora.Server.OnebotEvent.NoticeEvent;
using Sora.Entities;
using Sora.Enumeration.EventParamsType;

namespace Sora.EventArgs.SoraEvent
{
    /// <summary>
    /// 管理员变动事件参数
    /// </summary>
    public sealed class GroupAdminChangeEventArgs : BaseSoraEventArgs
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
        /// 动作类型
        /// </summary>
        public AdminChangeType SubType { get; private set; }
        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionGuid">服务器链接标识</param>
        /// <param name="eventName">事件名</param>
        /// <param name="adminChangeArgs">管理员变动事件参数</param>
        internal GroupAdminChangeEventArgs(Guid connectionGuid, string eventName, ApiAdminChangeEventArgs adminChangeArgs) :
            base(connectionGuid, eventName, adminChangeArgs.SelfID, adminChangeArgs.Time)
        {
            this.SourceGroup = new Group(connectionGuid, adminChangeArgs.GroupId);
            this.Sender      = new User(connectionGuid, adminChangeArgs.UserId);
            this.SubType     = adminChangeArgs.SubType;
        }
        #endregion
    }
}
