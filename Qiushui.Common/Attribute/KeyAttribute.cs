using System;

namespace Robot.Common
{
    /// <summary>
    /// 匹配关键字属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class KeyAttribute : Attribute
    {
        public string KeyWord { get; set; }
        public KeyAttribute(string KeyWord)
        {
            this.KeyWord = KeyWord;
        }
    }
}
