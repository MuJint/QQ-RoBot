using Robot.Common.Enums;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Robot.Common.Interface;

namespace Robot.Common.Helper
{
    /// <summary>
    /// log services
    /// </summary>
    public class LogsHelper: ILogsInterface
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
        public void Debug(Exception ex = null, string logs = null, EnumLogLevel logLevel = EnumLogLevel.Debug, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
        {
            if (LogLevel > logLevel) return;
            lock (_logLock)
            {
                ChangeConsoleColor(ConsoleColor.White, _consoleColor);
                Console.Write($@"[{DateTime.Now.ToString(_cultureInfo)}] | ");
                ChangeConsoleColor(ConsoleColor.Black, ConsoleColor.White);
                Console.Write(@" [Debug] ");
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
        public void Error(Exception ex, string logs, EnumLogLevel logLevel = EnumLogLevel.Debug, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
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
        private void ChangeConsoleColor(ConsoleColor fColor, ConsoleColor bColor)
        {
            Console.ForegroundColor = fColor;
            Console.BackgroundColor = bColor;
        }
        #endregion
    }
}
