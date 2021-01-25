using System;

namespace Sora.Tool
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class Utils
    {
        #region 时间相关
        /// <summary>
        /// 获取当前时间戳
        /// 时间戳单位(秒)
        /// </summary>
        public static long GetNowTimeStamp() =>(long) (DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0, 0)).TotalSeconds;

        /// <summary>
        /// 将DateTime转换为时间戳(s)
        /// </summary>
        public static long DateTimeToTimeStamp(DateTime dateTime) =>(long)
            (dateTime - (new DateTime(1970, 1, 1, 8, 0, 0, 0))).TotalSeconds;

        /// <summary>
        /// 将时间戳(s)转换为DateTime
        /// </summary>
        public static DateTime TimeStampToDateTime(long TimeStamp) =>
            new DateTime(1970, 1, 1, 8, 0, 0, 0).AddSeconds(TimeStamp);
        #endregion
    }
}
