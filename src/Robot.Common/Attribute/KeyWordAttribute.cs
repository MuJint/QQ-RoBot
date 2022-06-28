using System;

namespace Robot.Common
{
    /// <summary>
    /// 匹配关键字特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class KeyWordAttribute : Attribute
    {
        /// <summary>
        /// 关键字符
        /// </summary>
        public string KeyWord { get; set; }
        /// <summary>
        /// 全字匹配
        /// </summary>
        public bool FullMatch { get; set; }

        /// <summary>
        /// 匹配关键字特性
        /// </summary>
        /// <param name="KeyWord">关键字符</param>
        /// <param name="FullMatch">全字匹配</param>
        public KeyWordAttribute(string KeyWord, bool FullMatch = false)
        {
            this.KeyWord = KeyWord;
            this.FullMatch = FullMatch;
        }
    }
}
