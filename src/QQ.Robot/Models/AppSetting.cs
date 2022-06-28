using Robot.Common.Enums;
using System.Collections.Generic;
using System.Reflection;

namespace QQ.RoBot.Models
{
    /// <summary>
    /// 配置中心
    /// </summary>
    public class AppSetting
    {
        /// <summary>
        /// 日志
        /// </summary>
        public Logging Logging { get; set; }
        /// <summary>
        /// Socket配置
        /// </summary>
        public ServerConfig ServerConfig { get; set; }
        /// <summary>
        /// 用户配置
        /// </summary>
        public UserConfig UserConfig { get; set; }
    }

    public class Logging
    {
        public LogLevel LogLevel { get; set; }
    }

    public class LogLevel
    {
        /// <summary>
        /// 日志等级
        /// </summary>
        public EnumLogLevel Default { get; set; }
    }

    public class ServerConfig
    {
        /// <summary>
        /// 监听地址
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// 鉴权Token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// API请求路径
        /// </summary>
        public string ApiPath { get; set; }

        /// <summary>
        /// Event请求路径
        /// </summary>
        public string EventPath { get; set; }

        /// <summary>
        /// Universal请求路径
        /// </summary>
        public string UniversalPath { get; set; }

        /// <summary>
        /// <para>心跳包超时设置(秒)</para>
        /// <para>此值请不要小于或等于客户端心跳包的发送间隔</para>
        /// </summary>
        public uint HeartBeatTimeOut { get; set; }

        /// <summary>
        /// <para>客户端API调用超时设置(毫秒)</para>
        /// <para>默认为1000无需修改</para>
        /// </summary>
        public uint ApiTimeOut { get; set; }
    }
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
            return string.Join("\n", ret);
        }
        #endregion
    }

    /// <summary>
    /// Lian默认配置
    /// </summary>
    public class ConfigModel
    {
        public string BotName { get; set; }
        public List<string> GroupIds { get; set; }
        /// <summary>
        /// 小尾巴
        /// </summary>
        public string Tail { get; set; } = "";
        public string AiPath { get; set; }
        public string GroupImgPath { get; set; }
    }
}
