using System.ComponentModel;

namespace Sora.Enumeration.EventParamsType
{
    /// <summary>
    /// 群申请类型
    /// </summary>
    public enum GroupRequestType
    {
        /// <summary>
        /// 加群申请
        /// </summary>
        [Description("add")]
        Add,
        /// <summary>
        /// 加群邀请
        /// </summary>
        [Description("invite")]
        Invite
    }
}
