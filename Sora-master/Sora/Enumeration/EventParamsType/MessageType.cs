using System.ComponentModel;

namespace Sora.Enumeration.EventParamsType
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 私聊消息
        /// </summary>
        [Description("private")]
        Private,

        /// <summary>
        /// 群消息
        /// </summary>
        [Description("group")]
        Group
    }
}
