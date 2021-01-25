namespace Qiushui.Lian.Bot.Models
{
    internal enum SetuSourceType
    {
        /// <summary>
        /// 混合源模式
        /// </summary>
        Mix = 1,
        /// <summary>
        /// 从Lolicon获取图片信息
        /// </summary>
        Lolicon = 2,
        /// <summary>
        /// 从Yukari获取图片信息
        /// </summary>
        Yukari = 3,
        /// <summary>
        /// 从本地读取图片
        /// </summary>
        Local = 4,
    }
}
