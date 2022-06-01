using System.Collections.Generic;

namespace QQ.RoBot
{
    /// <summary>
    /// 色图 model
    /// </summary>
    public class HsoResult
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 次数 lolicon
        /// </summary>
        public int Quota { get; set; }
        /// <summary>
        /// 时间限制lolicon
        /// </summary>
        public int Quota_min_ttl { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public List<Info> Data { get; set; }

        /// <summary>
        /// 信息不一定有
        /// </summary>
        public class Info
        {
            /// <summary>
            /// Pid
            /// </summary>
            public int Pid { get; set; }
            /// <summary>
            /// 作品所在 P
            /// </summary>
            public int P { get; set; }
            /// <summary>
            /// 画师作者Uid
            /// </summary>
            public int Uid { get; set; }
            /// <summary>
            /// 作品标题
            /// </summary>
            public string Title { get; set; }
            public string Author { get; set; }
            public string Url { get; set; }
            /// <summary>
            /// 是否R18
            /// </summary>
            public bool R18 { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            /// <summary>
            /// 标签
            /// </summary>
            public string[] Tags { get; set; }
        }

    }
}
