using Fleck;
using System;
using System.Diagnostics;
using System.Text;

namespace Sora.Tool
{
    /// <summary>
    /// <para>控制台格式化Log类</para>
    /// <para>用于输出格式化Log</para>
    /// </summary>
    public static class ConsoleLog
    {
        #region Log等级设置
        private static LogLevel Level = LogLevel.Info;

        /// <summary>
        /// 设置日志等级
        /// </summary>
        /// <param name="level">LogLevel</param>
        public static void SetLogLevel(LogLevel level) => Level = level;

        /// <summary>
        /// 禁用log
        /// </summary>
        public static void SetNoLog() => Level = (LogLevel) 5;
        #endregion

        #region 格式化错误Log
        /// <summary>
        /// 生成格式化的错误Log文本
        /// </summary>
        /// <param name="e">错误</param>
        /// <returns>格式化Log</returns>
        public static string ErrorLogBuilder(Exception e)
        {
            StringBuilder errorMessageBuilder = new StringBuilder();
            errorMessageBuilder.Append("\r\n");
            errorMessageBuilder.Append("==============ERROR==============\r\n");
            errorMessageBuilder.Append("Error:");
            errorMessageBuilder.Append(e.GetType().FullName);
            errorMessageBuilder.Append("\r\n\r\n");
            errorMessageBuilder.Append("Message:");
            errorMessageBuilder.Append(e.Message);
            errorMessageBuilder.Append("\r\n\r\n");
            errorMessageBuilder.Append("Stack Trace:\r\n");
            errorMessageBuilder.Append(e.StackTrace);
            errorMessageBuilder.Append("\r\n");
            errorMessageBuilder.Append("=================================\r\n");
            return errorMessageBuilder.ToString();
        }
        #endregion

        #region 格式化控制台Log函数
        /// <summary>
        /// 向控制台发送Info信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="message">信息内容</param>
        public static void Info(object type, object message)
        {
            if (Level <= LogLevel.Info)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"[{DateTime.Now}][INFO][{type}]{message}");
            }
        }

        /// <summary>
        /// 向控制台发送Warning信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="message">信息内容</param>
        public static void Warning(object type, object message)
        {
            if (Level <= LogLevel.Warn)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"[{DateTime.Now}][");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("WARNINIG");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"][{type}]");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// 向控制台发送Error信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="message">信息内容</param>
        public static void Error(object type, object message)
        {
            if (Level <= LogLevel.Error)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"[{DateTime.Now}][");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ERROR");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"][{type}]");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// 向控制台发送Fatal信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="message">信息内容</param>
        public static void Fatal(object type, object message)
        {
            if (Level <= LogLevel.Error)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"[{DateTime.Now}][");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("FATAL");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"][{type}]");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(message);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// 向控制台发送Debug信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="message">信息内容</param>
        public static void Debug(object type, object message)
        {
            if (Level == LogLevel.Debug)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"[{DateTime.Now}][");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("DEBUG");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"][{type}]");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(message);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        #endregion

        #region 全局错误Log
        /// <summary>
        /// 全局错误Log
        /// </summary>
        /// <param name="args">UnhandledExceptionEventArgs</param>
        internal static void UnhandledExceptionLog(UnhandledExceptionEventArgs args)
        {
            StringBuilder errorLogBuilder = new StringBuilder();
            errorLogBuilder.Append("检测到未处理的异常");
            if (args.IsTerminating)
                errorLogBuilder.Append("，服务器将停止运行");
            errorLogBuilder.Append("，错误信息:");
            errorLogBuilder
                .Append(ErrorLogBuilder(args.ExceptionObject as Exception));
            Fatal("Sora",errorLogBuilder);
            //Warning("Sora","将在1S后自动重启");
            //new Process()
            //{
            //    StartInfo =
            //    {
            //        FileName = AppDomain.CurrentDomain.BaseDirectory + "Qiushui.Bot.exe",
            //        UseShellExecute = false
            //    }
            //}.Start();
            //Environment.Exit(-1);
        }
        #endregion
    }
}
