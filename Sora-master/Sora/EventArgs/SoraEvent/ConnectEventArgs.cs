using System;

namespace Sora.EventArgs.SoraEvent
{
    /// <summary>
    /// 客户端连接事件参数
    /// </summary>
    public sealed class ConnectEventArgs : BaseSoraEventArgs
    {
        #region 属性
        /// <summary>
        /// 连接客户端类型
        /// </summary>
        public string ClientType { get; private set; }

        /// <summary>
        /// 连接客户端版本号
        /// </summary>
        public string ClientVersionCode { get; private set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionGuid">服务器链接标识</param>
        /// <param name="eventName">事件名</param>
        /// <param name="loginUid">当前使用的QQ号</param>
        /// <param name="clientType">当前客户端类型</param>
        /// <param name="clientVersion">当前客户端版本</param>
        /// <param name="time">连接时间</param>
        internal ConnectEventArgs(Guid connectionGuid, string eventName, long loginUid, string clientType,
                                string clientVersion, long time) : base(connectionGuid, eventName, loginUid, time)
        {
            this.ClientType        = clientType;
            this.ClientVersionCode = clientVersion;
        }
        #endregion
    }
}
