using System.Collections.Generic;

namespace QQ.RoBot
{
    public class StaticModel
    {
        private static Dictionary<long,List<string>> _dic = null;
        private static readonly object _lock = new object();
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
