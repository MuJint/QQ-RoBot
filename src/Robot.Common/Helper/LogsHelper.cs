using Robot.Common.Enums;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Robot.Common.Interface;
using System.IO;

namespace Robot.Common.Helper
{
    /// <summary>
    /// log services
    /// </summary>
    public class LogsHelper : ILogsInterface
    {
        #region Property

        public EnumLogLevel LogLevel = EnumLogLevel.Information;
        private readonly object _logLock = new();
        private ConsoleColor _consoleColor = Console.BackgroundColor;
        private readonly CultureInfo _cultureInfo = CultureInfo.CurrentCulture;
        #endregion

        #region Method
        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="logs"></param>
        /// <param name="logLevel"></param>
        /// <param name="memberName"></param>
        /// <param name="lineNumber"></param>
        /// <param name="filePath"></param>
        public void Debug(Exception ex = null, string logs = "", EnumLogLevel logLevel = EnumLogLevel.Debug, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
        {
            if (LogLevel > logLevel) return;
            lock (_logLock)
            {
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                Console.Write($@"[{DateTime.Now.ToString(_cultureInfo)}] | ");
                ChangeConsoleColor(ConsoleColor.Black, ConsoleColor.White);
                Console.Write(@" [Debug] ");
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                Console.WriteLine($"[{ex?.GetType() ?? typeof(LogsHelper)}]{logs}");
                Console.WriteLine($"[调用方法]{memberName}");
                Console.WriteLine($"[行号]{lineNumber}");
                Console.WriteLine($"[路径]{filePath}\r\n");
            }
        }

        /// <summary>
        /// Info
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="logs"></param>
        /// <param name="logLevel"></param>
        /// <param name="memberName"></param>
        /// <param name="lineNumber"></param>
        /// <param name="filePath"></param>
        public void Info(Exception ex, string logs, EnumLogLevel logLevel = EnumLogLevel.Information, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
        {
            if (LogLevel > logLevel) return;
            lock (_logLock)
            {
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                Console.Write($@"[{DateTime.Now.ToString(_cultureInfo)}] | ");
                ChangeConsoleColor(ConsoleColor.Black, ConsoleColor.White);
                Console.Write(@" [Info] ");
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                Console.WriteLine($"[{ex.GetType()}]{logs}");
                Console.WriteLine($"[调用方法]{memberName}");
                Console.WriteLine($"[行号]{lineNumber}");
                Console.WriteLine($"[路径]{filePath}\r\n");
            }
        }

        /// <summary>
        /// Info
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="logs"></param>
        /// <param name="logLevel"></param>
        /// <param name="memberName"></param>
        /// <param name="lineNumber"></param>
        /// <param name="filePath"></param>
        public void Info(string caption, string logs, EnumLogLevel logLevel = EnumLogLevel.Information, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
        {
            if (LogLevel > logLevel) return;
            lock (_logLock)
            {
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                Console.Write($@"[ {DateTime.Now.ToString(_cultureInfo)} ] | ");
                ChangeConsoleColor(ConsoleColor.Black, ConsoleColor.White);
                Console.Write(@" [Info] ");
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                Console.WriteLine($"[{caption}]{logs}");
                Console.WriteLine($"[调用方法]{memberName}");
                Console.WriteLine($"[行号]{lineNumber}");
                Console.WriteLine($"[路径]{filePath}\r\n");
            }
        }

        /// <summary>
        /// Warn
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="logs"></param>
        /// <param name="logLevel"></param>
        /// <param name="memberName"></param>
        /// <param name="lineNumber"></param>
        /// <param name="filePath"></param>
        public void Warn(Exception ex, string logs, EnumLogLevel logLevel = EnumLogLevel.Warning, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
        {
            if (LogLevel > logLevel) return;
            lock (_logLock)
            {
                ChangeConsoleColor(ConsoleColor.DarkYellow, _consoleColor);
                Console.Write($@"[ {DateTime.Now.ToString(_cultureInfo)} ] | ");
                ChangeConsoleColor(ConsoleColor.Black, ConsoleColor.DarkYellow);
                Console.Write(@" [Warn] ");
                ChangeConsoleColor(ConsoleColor.DarkYellow, _consoleColor);
                Console.WriteLine($"[{ex.GetType()}]{logs}");
                Console.WriteLine($"[调用方法]{memberName}");
                Console.WriteLine($"[行号]{lineNumber}");
                Console.WriteLine($"[路径]{filePath}\r\n");
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                WriteLogToFile(ex, logs, logLevel, memberName, lineNumber, filePath);
            }
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="logs"></param>
        /// <param name="logLevel"></param>
        /// <param name="memberName"></param>
        /// <param name="lineNumber"></param>
        /// <param name="filePath"></param>
        public void Error(Exception ex, string logs, EnumLogLevel logLevel = EnumLogLevel.Error, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
        {
            if (LogLevel > logLevel) return;
            lock (_logLock)
            {
                ChangeConsoleColor(ConsoleColor.DarkRed, _consoleColor);
                Console.Write($@"[ {DateTime.Now.ToString(_cultureInfo)} ] | ");
                ChangeConsoleColor(ConsoleColor.Black, ConsoleColor.DarkRed);
                Console.Write(@" [Error] ");
                ChangeConsoleColor(ConsoleColor.DarkRed, _consoleColor);
                Console.WriteLine($"[{ex.GetType()}]{logs}");
                Console.WriteLine($"[调用方法]{memberName}");
                Console.WriteLine($"[行号]{lineNumber}");
                Console.WriteLine($"[路径]{filePath}\r\n");
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                WriteLogToFile(ex, logs, logLevel, memberName, lineNumber, filePath);
            }
        }

        /// <summary>
        /// Fatal
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="logs"></param>
        /// <param name="logLevel"></param>
        /// <param name="memberName"></param>
        /// <param name="lineNumber"></param>
        /// <param name="filePath"></param>
        public void Fatal(Exception ex, string logs, EnumLogLevel logLevel = EnumLogLevel.Fatal, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
        {
            if (LogLevel > logLevel) return;
            lock (_logLock)
            {
                ChangeConsoleColor(ConsoleColor.Cyan, _consoleColor);
                Console.Write($@"[ {DateTime.Now.ToString(_cultureInfo)} ] | ");
                ChangeConsoleColor(ConsoleColor.Black, ConsoleColor.Cyan);
                Console.Write(@" [Fatal] ");
                ChangeConsoleColor(ConsoleColor.Cyan, _consoleColor);
                Console.WriteLine($"[{ex.GetType()}]{logs}");
                Console.WriteLine($"[调用方法]{memberName}");
                Console.WriteLine($"[行号]{lineNumber}");
                Console.WriteLine($"[路径]{filePath}\r\n");
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                WriteLogToFile(ex, logs, logLevel, memberName, lineNumber, filePath);
            }
        }

        /// <summary>
        /// 设置日志等级
        /// </summary>
        /// <param name="logLevel"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetLogLevel(EnumLogLevel logLevel)
        {
            if (Enum.IsDefined(typeof(EnumLogLevel), logLevel))
                LogLevel = logLevel;
        }
        #endregion

        #region Private Method

        public LogsHelper ChangeColor(ConsoleColor fColor, ConsoleColor bColor)
        {
            ChangeConsoleColor(fColor, bColor);
            return this;
        }

        /// <summary>
        /// 改变颜色
        /// </summary>
        /// <param name="fColor"></param>
        /// <param name="bColor"></param>
        private static void ChangeConsoleColor(ConsoleColor fColor, ConsoleColor bColor)
        {
            Console.ForegroundColor = fColor;
            Console.BackgroundColor = bColor;
        }

        /// <summary>
        /// 写入log到文件夹
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="logs"></param>
        /// <param name="logLevel"></param>
        /// <param name="memberName"></param>
        /// <param name="lineNumber"></param>
        /// <param name="filePath"></param>
        private static void WriteLogToFile(Exception ex, string logs, EnumLogLevel logLevel = EnumLogLevel.Error, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
        {
            var logPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\Logs";
            if (Directory.Exists(logPath) is false)
                Directory.CreateDirectory(logPath);
            logPath = Path.Combine(logPath, $"{DateTime.Now:yyyyMMdd}.log");
            if (File.Exists(logPath) is false)
            {
                using var streamLog = new FileStream(logPath, FileMode.CreateNew);
            }
            using var stream = new FileStream(logPath, FileMode.Append);
            var writer = new StreamWriter(stream);
            writer.Write($"时间:[{DateTime.Now}]\t\n日志等级:[{logLevel}]\t\n异常:{ex}\t\n日志:{logs}\t\n调用方法:[{memberName}]\t\n行号:[{lineNumber}]\t\n文件路径:[{filePath}]\r\r");
            writer.Flush();
            writer.Close();
        }
        #endregion
    }
}
