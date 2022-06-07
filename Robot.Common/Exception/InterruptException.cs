using System;

namespace Robot.Common
{
    /// <summary>
    /// 中断
    /// </summary>
    public class InterruptException : Exception
    {
        /// <summary>
        /// 中断程序但不报错
        /// </summary>
        public InterruptException()
        {

        }
    }
}
