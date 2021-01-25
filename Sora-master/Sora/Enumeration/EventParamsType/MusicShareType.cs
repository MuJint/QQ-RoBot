using System.ComponentModel;

namespace Sora.Enumeration.EventParamsType
{
    /// <summary>
    /// 音乐分享类型
    /// </summary>
    public enum MusicShareType
    {
        /// <summary>
        /// 网易云音乐
        /// </summary>
        [Description("163")]
        Netease,
        /// <summary>
        /// QQ 音乐
        /// </summary>
        [Description("qq")]
        QQMusic,
        /// <summary>
        /// 虾米音乐
        /// </summary>
        [Description("xm")]
        Xiami
    }
}
