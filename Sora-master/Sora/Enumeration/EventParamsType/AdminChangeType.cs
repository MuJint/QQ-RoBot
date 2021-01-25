using System.ComponentModel;

namespace Sora.Enumeration.EventParamsType
{
    /// <summary>
    /// 管理员变动类型
    /// </summary>
    public enum AdminChangeType
    {
        /// <summary>
        /// 设置
        /// </summary>
        [Description("set")]
        Set,
        /// <summary>
        /// 取消
        /// </summary>
        [Description("unset")]
        UnSet
    }
}