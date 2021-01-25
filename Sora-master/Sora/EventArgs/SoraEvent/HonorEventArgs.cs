using System;
using Sora.Server.OnebotEvent.NoticeEvent;
using Sora.Entities;
using Sora.Enumeration.EventParamsType;

namespace Sora.EventArgs.SoraEvent
{
    /// <summary>
    /// 授予荣誉事件参数
    /// </summary>
    public sealed class HonorEventArgs : BaseSoraEventArgs
    {
        #region 属性
        /// <summary>
        /// 荣誉获得者
        /// </summary>
        public User TargetUser { get; private set; }

        /// <summary>
        /// 消息源群
        /// </summary>
        public Group SourceGroup { get; private set; }

        /// <summary>
        /// 荣誉类型
        /// </summary>
        public HonorType Honor { get; private set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionGuid">服务器链接标识</param>
        /// <param name="eventName">事件名</param>
        /// <param name="honorEventArgs">荣誉变更事件参数</param>
        internal HonorEventArgs(Guid connectionGuid, string eventName, ApiHonorEventArgs honorEventArgs) :
            base(connectionGuid, eventName, honorEventArgs.SelfID, honorEventArgs.Time)
        {
            this.TargetUser  = new User(connectionGuid, honorEventArgs.UserId);
            this.SourceGroup = new Group(connectionGuid, honorEventArgs.GroupId);
            this.Honor       = honorEventArgs.HonorType;
        }
        #endregion
    }
}
