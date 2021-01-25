#nullable enable
using System;

namespace Sora.Exceptions
{
    /// <summary>
    /// 服务器已经在运行错误
    /// </summary>
    public class SoraServerIsRuningException : Exception
    {
        /// <summary>
        /// 当前连接的账号id
        /// </summary>
        public long SelfId { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        public SoraServerIsRuningException() : base("Server is running"){}

        /// <summary>
        /// 初始化
        /// </summary>
        public SoraServerIsRuningException(string message) : base(message) {}

        /// <summary>
        /// 初始化
        /// </summary>
        public SoraServerIsRuningException(string message, Exception? innerException) : base(message, innerException) {}

        /// <summary>
        /// 初始化
        /// </summary>
        public SoraServerIsRuningException(string message, long selfId, Exception? innerException) :
            base(message, innerException)
        {
            this.SelfId = selfId;
        }

        internal SoraServerIsRuningException(string message, long selfId) :
            base(message)
        {
            this.SelfId = selfId;
        }
    }
}
