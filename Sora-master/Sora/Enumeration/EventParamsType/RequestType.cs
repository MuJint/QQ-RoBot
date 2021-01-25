using System.ComponentModel;

namespace Sora.Enumeration.EventParamsType
{
    /// <summary>
    /// 请求类型
    /// </summary>
    public enum RequestType
    {
        /// <summary>
        /// 群组请求
        /// </summary>
        [Description("group")]
        Group,
        /// <summary>
        /// 好友请求
        /// </summary>
        [Description("friend")]
        Friend
    }
}
