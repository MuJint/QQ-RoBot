using System.Collections.Generic;
using System.Reflection;

namespace Qiushui.Lian.Bot.Helper.ConfigModule
{
    /// <summary>
    /// 单用户配置文件定义
    /// </summary>
    internal class UserConfig
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
    internal class ModuleSwitch
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
        /// 娱乐模块
        /// </summary>
        public bool HaveFun { set; get; }
        /// <summary>
        /// 来点色图
        /// </summary>
        public bool Hso { set; get; }
        /// <summary>
        /// 切噜翻译
        /// </summary>
        public bool Cheru { set; get; }
        /// <summary>
        /// 启用AI自动对话
        /// </summary>
        public bool IsAI { get; set; }

        #region 将已启用的模块名转为字符串
        public override string ToString()
        {
            List<string> ret = new List<string>();
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
}
