using System;

namespace Robot.Common
{
    /// <summary>
    /// 匹配关键字属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class KeyWordAttribute : Attribute
    {
        public string KeyWord { get; set; }
        public KeyWordAttribute(string KeyWord)
        {
            this.KeyWord = KeyWord;
        }
    }
}
