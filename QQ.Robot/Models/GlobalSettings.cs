using Robot.Common;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace QQ.RoBot
{
    /// <summary>
    /// 全局静态配置
    /// </summary>
    public class GlobalSettings
    {
        private static Dictionary<long,List<string>> _dic = null;
        private static readonly object _lock = new();
        /// <summary>
        /// 获取所有复读机的信息
        /// </summary>
        public static Dictionary<long, List<string>> GetDic
        {
            get
            {
                lock (_lock)
                {
                    return _dic ??= new Dictionary<long, List<string>>();
                }
            }
        }

        /// <summary>
        /// 所有反射加载方法
        /// </summary>
        public static Dictionary<string, MethodInfo> Methods { get; set; } = new Dictionary<string, MethodInfo>();

        /// <summary>
        /// KeyWordAttribute 匹配字典
        /// </summary>
        public static Dictionary<MethodInfo, List<Regex>> KeyWordRegexs { get; set; } = new Dictionary<MethodInfo, List<Regex>>();

        /// <summary>
        /// 接口对应服务字典
        /// </summary>
        public static Dictionary<string, object> MatchDic { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// 初始化接口对应服务字典
        /// </summary>
        public static void GlobalSettingsInit()
        {
            MatchDic.Add("ILianInterface", "LianService");
            MatchDic.Add("IHsoInterface", "HsoService");
        }
    }
}
