using System.ComponentModel;

namespace Sora.Enumeration.EventParamsType
{
    /// <summary>
    /// 群成员变动类型
    /// </summary>
    public enum MemberChangeType
    {
        /// <summary>
        /// 主动退群
        /// </summary>
        [Description("leave")]
        Leave,
        /// <summary>
        /// 成员被踢
        /// </summary>
        [Description("kick")]
        Kick,
        /// <summary>
        /// 登录号被踢
        /// </summary>
        [Description("kick_me")]
        KickMe,
        /// <summary>
        /// 管理员同意入群
        /// </summary>
        [Description("approve")]
        Approve,
        /// <summary>
        /// 管理员邀请入群
        /// </summary>
        [Description("invite")]
        Invite
    }
}
