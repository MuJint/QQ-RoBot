namespace Sora.EventArgs.WSSeverEvent
{
    /// <summary>
    /// 服务器连接事件
    /// </summary>
    public sealed class ConnectionEventArgs : System.EventArgs
    {
        #region 属性
        /// <summary>
        /// 客户端类型
        /// </summary>
        public string Role { get; private set; }

        /// <summary>
        /// 机器人登录账号UID
        /// </summary>
        public long SelfId { get; private set; }
        #endregion

        #region 构造函数
        internal ConnectionEventArgs(string role, long selfId)
        {
            this.SelfId = selfId;
            this.Role   = role;
        }
        #endregion
    }
}
