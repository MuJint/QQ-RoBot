namespace Qiushui.Lian.Bot.Models
{
    internal enum LenType
    {
        /// <summary>
        /// 不合法长度
        /// </summary>
        Illegal = 1,
        /// <summary>
        /// 合法长度
        /// </summary>
        Legitimate = 2,
        /// <summary>
        /// 合法长度且允许额外参数
        /// </summary>
        Extra = 3
    }
}
