using System;
using Sora.Server.OnebotEvent.NoticeEvent;
using Sora.Entities;

namespace Sora.EventArgs.SoraEvent
{
    /// <summary>
    /// 群戳一戳事件参数
    /// </summary>
    public sealed class GroupPokeEventArgs : BaseSoraEventArgs
    {
        #region 属性
        /// <summary>
        /// 发送者
        /// </summary>
        public User SendUser { get; private set; }

        /// <summary>
        /// 被戳者
        /// </summary>
        public User TargetUser { get; private set; }

        /// <summary>
        /// 消息源群
        /// </summary>
        public Group SourceGroup { get; private set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionGuid">服务器链接标识</param>
        /// <param name="eventName">事件名</param>
        /// <param name="pokeEventArgs">戳一戳事件参数</param>
        internal GroupPokeEventArgs(Guid connectionGuid, string eventName, ApiPokeOrLuckyEventArgs pokeEventArgs) :
            base(connectionGuid, eventName, pokeEventArgs.SelfID, pokeEventArgs.Time)
        {
            this.SendUser    = new User(connectionGuid, pokeEventArgs.UserId);
            this.TargetUser  = new User(connectionGuid, pokeEventArgs.TargetId);
            this.SourceGroup = new Group(connectionGuid, pokeEventArgs.GroupId);
        }
        #endregion
    }
}
