using System.ComponentModel;

namespace Sora.Enumeration.ApiEnum
{
    /// <summary>
    /// API返回值
    /// </summary>
    [DefaultValue(OK)]
    public enum APIStatusType
    {
        /// <summary>
        /// API执行成功
        /// </summary>
        OK       = 0,
        /// <summary>
        /// API执行失败
        /// </summary>
        Failed    = 100,
        /// <summary>
        /// 404
        /// </summary>
        NotFound = 404,
        /// <summary>
        /// API执行发生内部错误
        /// </summary>
        Error    = 502,
        /// <summary>
        /// API执行失败
        /// </summary>
        Failed_   = 102,
        /// <summary>
        /// API超时
        /// </summary>
        TimeOut = -1
    }
}
