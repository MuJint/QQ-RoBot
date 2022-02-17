using QQ.RoBot.Source;
using Microsoft.Extensions.DependencyInjection;
using Robot.Common.Interface;
using SharpYaml.Serialization;
using System;
using System.IO;
using System.Text;
using System.Threading;


namespace QQ.RoBot
{
    public class Config
    {
        #region 属性
        private string UserConfigPath { set; get; }
        private string GlobalConfigPath { get; set; }
        private UserConfig LoadedUserConfig { set; get; }
        private GlobalConfig LoadedGlobalConfig { set; get; }
        private readonly ILogsInterface _logs = Dependcy.Provider.GetService<ILogsInterface>();
        #endregion

        #region 构造函数
        /// <summary>
        /// Configs实例构造函数
        /// </summary>
        /// <param name="loginUid">uid</param>
        public Config(long loginUid)
        {
            //获取文件存储地址
            UserConfigPath = IOUtils.GetUserConfigPath(loginUid);
            GlobalConfigPath = IOUtils.GetGlobalConfigPath();
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 加载用户配置文件
        /// 读取成功后写入到私有属性中，第二次读取则不需要重新读取
        /// </summary>
        /// <param name="userConfig">读取到的配置文件数据</param>
        /// <param name="reload">重新读取</param>
        public bool LoadUserConfig(out UserConfig userConfig, bool reload = true)
        {
            if (!reload)
            {
                userConfig = LoadedUserConfig;
                return LoadedUserConfig != null;
            }
            _logs.Debug(new Exception(), "读取用户配置");
            try
            {
                //反序列化配置文件
                Serializer serializer = new();
                using TextReader reader = File.OpenText(UserConfigPath);
                LoadedUserConfig = serializer.Deserialize<UserConfig>(reader);
                //参数合法性检查
                if (LoadedUserConfig.HsoConfig.SizeLimit < 1)
                {
                    _logs.Error(new Exception(), "参数值超出合法范围，重新生成配置文件");
                    userConfig = null;
                    return false;
                }
                userConfig = LoadedUserConfig;
                return true;
            }
            catch (Exception e)
            {
                _logs.Error(e, "");
                userConfig = null;
                return false;
            }
        }

        /// <summary>
        /// 加载服务器全局配置文件
        /// 读取成功后写入到私有属性中，第二次读取则不需要重新读取
        /// </summary>
        /// <param name="globalConfig">读取到的配置文件数据</param>
        /// <param name="reload">重新读取</param>
        public bool LoadGlobalConfig(out GlobalConfig globalConfig, bool reload = true)
        {
            if (!reload)
            {
                globalConfig = LoadedGlobalConfig;
                return LoadedGlobalConfig != null;
            }
            _logs.Debug(new Exception(), "读取全局配置");
            try
            {
                //反序列化配置文件
                Serializer serializer = new();
                using TextReader reader = File.OpenText(GlobalConfigPath);
                LoadedGlobalConfig = serializer.Deserialize<GlobalConfig>(reader);
                //参数合法性检查
                if ((int)LoadedGlobalConfig.LogLevel is < 0 or > 3 ||
                    LoadedGlobalConfig.HeartBeatTimeOut == 0 ||
                    LoadedGlobalConfig.ApiTimeOut == 0 ||
                    LoadedGlobalConfig.Port is 0 or > 65535)
                {
                    _logs.Error(new Exception(), "参数值超出合法范围，重新生成配置文件");
                    globalConfig = null;
                    return false;
                }
                globalConfig = LoadedGlobalConfig;
                return true;
            }
            catch (Exception e)
            {
                _logs.Error(e, "");
                globalConfig = null;
                return false;
            }
        }

        /// <summary>
        /// 初始化用户配置文件并返回当前配置文件内容
        /// 初始化成功后写入到私有属性中，第二次读取则不需要重新读取
        /// </summary>
        public void UserConfigFileInit()
        {
            try
            {
                //当读取到文件时直接返回
                if (File.Exists(UserConfigPath) && LoadUserConfig(out _))
                {
                    _logs.Debug(new Exception(), "读取配置文件");
                    return;
                }
                //没读取到文件时创建新的文件
                _logs.Error(new Exception(), "未找到配置文件");
                _logs.Warn(new Exception(), "创建新的配置文件");
                string initConfigText = Encoding.UTF8.GetString(InitRes.InitUserConfig);
                using (TextWriter writer = File.CreateText(UserConfigPath))
                {
                    writer.Write(initConfigText);
                    writer.Close();
                }
                //读取生成的配置文件
                if (!LoadUserConfig(out _)) throw new IOException("无法读取生成的配置文件");
            }
            catch (Exception e)
            {
                _logs.Fatal(e, "");
                Thread.Sleep(5000);
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// 初始化全局配置文件并返回当前配置文件内容
        /// 初始化成功后写入到私有属性中，第二次读取则不需要重新读取
        /// </summary>
        public void GlobalConfigFileInit()
        {
            try
            {
                //当读取到文件时直接返回
                if (File.Exists(GlobalConfigPath) && LoadGlobalConfig(out _))
                {
                    _logs.Debug(new Exception(), "读取配置文件");
                    return;
                }
                //没读取到文件时创建新的文件
                _logs.Error(new Exception(), "未找到配置文件");
                _logs.Warn(new Exception(), "创建新的配置文件");
                string initConfigText = Encoding.UTF8.GetString(InitRes.InitGlobalConfig);
                using (TextWriter writer = File.CreateText(GlobalConfigPath))
                {
                    writer.Write(initConfigText);
                    writer.Close();
                }
                //读取生成的配置文件
                if (!LoadGlobalConfig(out _)) throw new IOException("无法读取生成的配置文件");
            }
            catch (Exception e)
            {
                _logs.Fatal(e, "ConfigIO ERROR");
                Thread.Sleep(5000);
                Environment.Exit(-1);
            }
        }
        #endregion
    }
}
