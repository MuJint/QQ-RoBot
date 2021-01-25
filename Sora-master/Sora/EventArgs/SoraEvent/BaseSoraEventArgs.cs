using System;
using Sora.Entities.Base;
using Sora.Tool;

namespace Sora.EventArgs.SoraEvent
{
    /// <summary>
    /// 框架事件基类
    /// </summary>
    public abstract class BaseSoraEventArgs : System.EventArgs
    {
        #region 属性
        /// <summary>
        /// 当前事件的API执行实例
        /// </summary>
        public SoraApi SoraApi { get; private set; }

        /// <summary>
        /// 当前事件名
        /// </summary>
        public string EventName { get; private set; }

        /// <summary>
        /// 事件产生时间
        /// </summary>
        public DateTime Time { get; private set; }

        /// <summary>
        /// 接收当前事件的机器人UID
        /// </summary>
        public long LoginUid { get; private set; }

        /// <summary>
        /// 事件产生时间戳
        /// </summary>
        internal long TimeStamp { get; private set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionGuid">服务器链接标识</param>
        /// <param name="eventName">事件名</param>
        /// <param name="loginUid">当前使用的QQ号</param>
        /// <param name="time">连接时间</param>
        internal BaseSoraEventArgs(Guid connectionGuid, string eventName, long loginUid, long time)
        {
            this.SoraApi   = new SoraApi(connectionGuid);
            this.EventName = eventName;
            this.LoginUid  = loginUid;
            this.TimeStamp = time;
            this.Time      = Utils.TimeStampToDateTime(time);
        }
        #endregion
    }
}
