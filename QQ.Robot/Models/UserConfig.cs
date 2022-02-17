using System.Collections.Generic;
using System.Reflection;

namespace QQ.RoBot
{
    /// <summary>
    /// 单用户配置文件定义
    /// </summary>
    public class UserConfig
    {
        /// <summary>
        /// 各模块的控制开关
        /// </summary>
        public ModuleSwitch ModuleSwitch { set; get; }
        /// <summary>
        /// <summary>
        /// 色图相关设置
        /// </summary>
        public Hso HsoConfig { set; get; }
        /// <summary>
        /// 莲配置
        /// </summary>
        public ConfigModel ConfigModel { get; set; }
    }

    /// <summary>
    /// 各模块开关
    /// </summary>
    public class ModuleSwitch
    {
        /// <summary>
        /// 复读模式
        /// </summary>
        public bool Reread { get; set; }
        /// <summary>
        /// 打劫
        /// </summary>
        public bool Rob { get; set; }
        /// <summary>
        /// 抽奖
        /// </summary>
        public bool Raffle { get; set; }
        /// <summary>
        /// 劫狱
        /// </summary>
        public bool Rescur { get; set; }
        /// <summary>
        /// 莲正常功能
        /// </summary>
        public bool LianBot { get; set; }
        /// <summary>
        /// 来点色图
        /// </summary>
        public bool Hso { set; get; }
        /// <summary>
        /// 启用AI自动对话
        /// </summary>
        public bool IsAI { get; set; }
        /// <summary>
        /// 是否撤回
        /// </summary>
        public bool Recal { get; set; }

        #region 将已启用的模块名转为字符串
        public override string ToString()
        {
            List<string> ret = new();
            //遍历使能设置中的所有属性
            foreach (PropertyInfo property in typeof(ModuleSwitch).GetProperties())
            {
                if (property.GetValue(this, null) is bool isEnable && isEnable)
                {
                    ret.Add(property.Name);
                }
            }
            return string.Join("\n",ret);
        }
        #endregion
    }

    /// <summary>
    /// Lian默认配置
    /// </summary>
    public class ConfigModel
    {
        public string NickName { get; set; }
        public string BotName { get; set; }
        public List<string> GroupIds { get; set; }
        /// <summary>
        /// 小尾巴
        /// </summary>
        public string Tail { get; set; } = "";
        public string AiPath { get; set; }
        public string GroupImgPath { get; set; }
    }

    public class Hso
    {
        /// <summary>
        /// Pximy代理
        /// </summary>
        public string PximyProxy { set; get; }
        /// <summary>
        /// 是否启用本地缓存
        /// </summary>
        public bool UseCache { set; get; }
        /// <summary>
        /// 是否使用装逼大图
        /// </summary>
        public bool CardImage { set; get; }
        /// <summary>
        /// 色图文件夹大小限制
        /// </summary>
        public long SizeLimit { set; get; }
        /// <summary>
        /// LoliconToken
        /// </summary>
        public string LoliconApiKey { set; get; }
        /// <summary>
        /// YukariToken
        /// </summary>
        public string YukariApiKey { set; get; }
        /// <summary>
        /// R18
        /// </summary>
        public bool R18 { get; set; } = false;
    }
}
