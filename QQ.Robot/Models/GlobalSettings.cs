using QQ.RoBot.Models;
using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace QQ.RoBot
{
    /// <summary>
    /// 全局静态配置
    /// </summary>
    public class GlobalSettings
    {
        public static AppSetting AppSetting = new();
        private static Dictionary<long,List<string>> _dic = null;
        private static readonly object _lock = new();
        /// <summary>
        /// 获取所有复读机的信息
        /// </summary>
        public static Dictionary<long, List<string>> ReReadDic
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
        /// AI接口调用次数限制
        /// </summary>
        public static (DateTime, int) AIRequest { get; set; } = (DateTime.Now, 0);

        /// <summary>
        /// 谁是卧底投票结果
        /// </summary>
        public static List<TpResults> TpResult = new();

        /// <summary>
        /// 所有反射加载方法
        /// </summary>
        //public static Dictionary<MethodInfo, string> AllMethods { get; set; } = new Dictionary<MethodInfo, string>();

        /// <summary>
        /// KeyWordAttribute 匹配字典
        /// </summary>
        public static Dictionary<MethodInfo, List<Regex>> KeyWordRegexs { get; set; } = new Dictionary<MethodInfo, List<Regex>>();

        /// <summary>
        /// 初始化接口对应服务
        /// 所有方法写入内存
        /// </summary>
        public static void InitalizeInterfaceToService()
        {
            //方法写入内存
            var assemblyType = Assembly.GetAssembly(typeof(ILianInterface)).ExportedTypes
                    .Where(w => w.FullName.Contains("Interface"))
                    .Select(s => new AssemblyMethod
                    {
                        Name = s.Name,
                        Methods = s.GetMethods(),
                    }).ToList();
            foreach (var assembly in assemblyType.SelectMany(s => s.Methods))
            {
                if (assembly?.GetCustomAttribute(typeof(KeyWordAttribute)) is not KeyWordAttribute attribute)
                    continue;
                //所有反射方法 SignIn -> ILianInterface
                //AllMethods.TryAdd(assembly, assembly.DeclaringType.Name);
                //正则匹配字典 SignIn -> [(签到)+]  
                KeyWordRegexs.TryAdd(assembly, attribute.KeyWord.Split(' ').Select(s => new Regex($"({s})+")).ToList());
            }
        }

        public class TpResults
        {
            public int RoomId { get; set; }
            /// <summary>
            /// 投票人
            /// </summary>
            public long TpUid { get; set; }
            /// <summary>
            /// 被票对象
            /// </summary>
            public long Uid { get; set; }
            public int Num { get; set; } = 1;
        }

        /// <summary>
        /// 反射类
        /// </summary>
        private class AssemblyMethod
        {
            public string Name { get; set; }
            public MethodInfo[] Methods { get; set; }
        }
    }
}
