using System.ComponentModel;

namespace Qiushui.Lian.Bot.Models
{ 
    /// <summary>
    /// 正则触发
    /// 正则表达式在Description中，仅在初始化时读取
    /// </summary>
    internal enum RegexCommand
    {
        /// <summary>
        /// 切噜编码
        /// </summary>
        [Description(@"^切噜一下")]
        CheruEncode,
        /// <summary>
        /// 切噜翻译
        /// </summary>
        [Description(@"^切噜(?:~|～)")]
        CheruDecode,
        /// <summary>
        /// 查询排名
        /// </summary>
        [Description(@"^查询公会排名\S*$")]
        GetGuildRank,
        /// <summary>
        /// 随机数
        /// </summary>
        [Description(@"^dice$")]
        Dice,
        [Description(@"^debug")]
        Debug
    }
}
