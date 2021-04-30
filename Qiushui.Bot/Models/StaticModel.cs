using System.Collections.Generic;

namespace Qiushui.Bot
{
    public class StaticModel
    {
        private static Dictionary<long,List<string>> _dic = null;
        private static Dictionary<long,bool> _systemLog = null;
        private static readonly object _lock = new object();

        /// <summary>
        /// 获取所有当日已发送的系统公告
        /// </summary>
        public static Dictionary<long, bool> GetSystemLog
        { 
            get
            {
                lock (_lock)
                {
                    return _systemLog ??= new Dictionary<long, bool>();
                }
            }
        }

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
    }
}
