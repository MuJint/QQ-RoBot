using System.Collections.Generic;

namespace Qiushui.Bot
{
    /// <summary>
    /// Lian默认配置
    /// </summary>
    internal class ConfigModel
    {
        public string NickName { get; set; }
        public string BotName { get; set; }
        public List<string> GroupIds { get; set; }
        /// <summary>
        /// 小尾巴
        /// </summary>
        public string Tail { get; set; } = "";
        public string AiPath { get; set; }
    }
}
