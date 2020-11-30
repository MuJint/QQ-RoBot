namespace Qiushui.Lian.Bot.Models
{
    /// <summary>
    /// 成员状态标识
    /// </summary>
    internal enum FlagType
    {
        /// <summary>
        /// 未知成员
        /// </summary>
        UnknownMember = -1,
        /// <summary>
        /// 空闲
        /// </summary>
        IDLE = 0,
        /// <summary>
        /// 出刀中
        /// </summary>
        EnGage = 1,
        /// <summary>
        /// 在树上
        /// </summary>
        OnTree = 3
    }
}
