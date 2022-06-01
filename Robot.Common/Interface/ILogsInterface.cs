using Robot.Common.Enums;
using System;
using System.Runtime.CompilerServices;

namespace Robot.Common.Interface
{
    /// <summary>
    /// log interface
    /// </summary>
    public interface ILogsInterface
    {
        void Debug(Exception ex=null, string logs=null, EnumLogLevel logLevel = EnumLogLevel.Debug, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "");
        void Info(string caption, string logs, EnumLogLevel logLevel = EnumLogLevel.Information, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "");
        void Info(Exception ex, string logs, EnumLogLevel logLevel = EnumLogLevel.Information, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = ""); void Warn(Exception ex, string logs, EnumLogLevel logLevel = EnumLogLevel.Warning, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "");
        void Error(Exception ex, string logs, EnumLogLevel logLevel = EnumLogLevel.Debug, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "");
        void Fatal(Exception ex, string logs, EnumLogLevel logLevel = EnumLogLevel.Fatal, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "");
        void SetLogLevel(EnumLogLevel logLevel);
    }
}
