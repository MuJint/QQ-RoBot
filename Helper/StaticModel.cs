using System.Collections.Generic;

namespace Qiushui.Lian.Bot.Helper
{
    public class StaticModel
    {
        private static Dictionary<long,List<string>> _dic = null;
        private static readonly object _lock = new object();

        public StaticModel()
        {

        }

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
