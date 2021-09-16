using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YukariToolBox.FormatLog;

namespace QQ.RoBot
{
    public static class IOUtils
    {
        #region IO工具
        /// <summary>
        /// 获取错误报告路径
        /// </summary>
        private static string GetCrashLogPath()
        {
            StringBuilder pathBuilder = new StringBuilder();
#if DEBUG
            pathBuilder.Append(Environment.GetEnvironmentVariable("DebugDataPath"));
#else
            pathBuilder.Append(Environment.CurrentDirectory);
#endif
            pathBuilder.Append("/crashlog");
            //检查目录是否存在，不存在则新建一个
            Directory.CreateDirectory(pathBuilder.ToString());
            return pathBuilder.ToString();
        }

        /// <summary>
        /// 获取应用配置文件的绝对路径
        /// </summary>
        public static string GetUserConfigPath(long userId)
        {
            if (userId < 10000) return null;
            StringBuilder pathBuilder = new StringBuilder();
#if DEBUG
            pathBuilder.Append(Environment.GetEnvironmentVariable("DebugDataPath"));
#else
            pathBuilder.Append(Environment.CurrentDirectory);
#endif
            pathBuilder.Append("/config/");
            //二级文件夹
            pathBuilder.Append(userId);
            //检查目录是否存在，不存在则新建一个
            Directory.CreateDirectory(pathBuilder.ToString());
            pathBuilder.Append("/config.yaml");
            return pathBuilder.ToString();
        }

        public static string GetGlobalConfigPath()
        {
            StringBuilder pathBuilder = new StringBuilder();
#if DEBUG
            pathBuilder.Append(Environment.GetEnvironmentVariable("DebugDataPath"));
#else
            pathBuilder.Append(Environment.CurrentDirectory);
#endif
            pathBuilder.Append("/config");
            //检查目录是否存在，不存在则新建一个
            Directory.CreateDirectory(pathBuilder.ToString());
            pathBuilder.Append("/server_config.yaml");
            return pathBuilder.ToString();
        }

        /// <summary>
        /// 获取应用色图文件的绝对路径
        /// </summary>
        public static string GetHsoPath()
        {
            StringBuilder pathBuilder = new StringBuilder();
#if DEBUG
            pathBuilder.Append(Environment.GetEnvironmentVariable("DebugDataPath"));
#else
            pathBuilder.Append(Environment.CurrentDirectory);
#endif
            pathBuilder.Append("/data/image/hso");
            //检查目录是否存在，不存在则新建一个
            Directory.CreateDirectory(pathBuilder.ToString());
            return pathBuilder.ToString();
        }

        /// <summary>
        /// 创建错误报告文件
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        public static void CrashLogGen(string errorMessage)
        {
            StringBuilder pathBuilder = new StringBuilder();
            pathBuilder.Append(GetCrashLogPath());
            pathBuilder.Append("crash-");
            pathBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            pathBuilder.Append(".log");

            using StreamWriter streamWriter = File.CreateText(pathBuilder.ToString());
            streamWriter.Write(errorMessage);
        }

        /// <summary>
        /// 获取色图文件夹的大小(Byte)
        /// </summary>
        public static long GetHsoSize()
        {
            return new DirectoryInfo(GetHsoPath())
                   .GetFiles("*", SearchOption.AllDirectories)
                   .Select(file => file.Length)
                   .Sum();
        }
        #endregion

        #region 文件读取工具
        /// <summary>
        /// 读取Json文件并返回为一个JObject
        /// </summary>
        /// <param name="jsonPath">json文件路径</param>
        /// <returns>保存整个文件信息的JObject</returns>
        public static JObject LoadJsonFile(string jsonPath)
        {
            try
            {
                StreamReader jsonFile = File.OpenText(jsonPath);
                JsonTextReader reader = new JsonTextReader(jsonFile);
                JObject jsonObject = (JObject)JToken.ReadFrom(reader);
                return jsonObject;
            }
            catch (Exception e)
            {
                Log.Error("IO ERROR", $"读取文件{jsonPath}时出错，错误：\n{Log.ErrorLogBuilder(e)}");
                return null;
            }
        }

        #endregion
    }
}
