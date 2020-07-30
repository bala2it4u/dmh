using System;

namespace LuckyHome.Common
{
    public interface ILogger
    {
        bool IsDebugEnabled { get; }
        //
        // Summary:
        //     Checks if this logger is enabled for the log4net.Core.Level.Fatal level.
        //
        // Remarks:
        //     For more information see log4net.ILog.IsDebugEnabled.
        bool IsFatalEnabled { get; }
        //
        // Summary:
        //     Checks if this logger is enabled for the log4net.Core.Level.Warn level.
        //
        // Remarks:
        //     For more information see log4net.ILog.IsDebugEnabled.
        bool IsWarnEnabled { get; }
        //
        // Summary:
        //     Checks if this logger is enabled for the log4net.Core.Level.Info level.
        //
        // Remarks:
        //     For more information see log4net.ILog.IsDebugEnabled.
        bool IsInfoEnabled { get; }
        
        //
        // Summary:
        //     Checks if this logger is enabled for the log4net.Core.Level.Error level.
        //
        // Remarks:
        //     For more information see log4net.ILog.IsDebugEnabled.
        bool IsErrorEnabled { get; }

        void Debug(object message, Exception ex = null);
        void Error(object message, Exception ex = null);
        void Fatal(object message, Exception ex = null);
        void Info(object message, Exception ex = null);
        void Warn(object message, Exception ex = null);
    }
}