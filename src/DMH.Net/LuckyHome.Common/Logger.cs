using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyHome.Common
{
    internal class Logger : ILogger
    {
        private readonly log4net.ILog _log;

        internal Logger(string name = null)
        {
            if (name != null)
                _log = log4net.LogManager.GetLogger(name);
            else
            {
                _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        internal Logger(Type type)
        {
            _log = log4net.LogManager.GetLogger(type);
        }

        public bool IsDebugEnabled => _log.IsDebugEnabled;

        public bool IsFatalEnabled => _log.IsFatalEnabled;

        public bool IsWarnEnabled => _log.IsWarnEnabled;

        public bool IsInfoEnabled => _log.IsInfoEnabled;

        public bool IsErrorEnabled => _log.IsErrorEnabled;

        public void Debug(object message, Exception ex = null)
        {
            if (_log.IsDebugEnabled)
            {
                if (ex == null)
                {
                    _log.Debug(message);
                }
                else
                {
                    _log.Debug(message, ex);
                }
            }
        }

        public void Info(object message, Exception ex = null)
        {
            if (_log.IsInfoEnabled)
            {
                if (ex == null)
                {
                    _log.Info(message);
                }
                else
                {
                    _log.Info(message, ex);
                }
            }
        }

        public void Warn(object message, Exception ex = null)
        {
            if (_log.IsWarnEnabled)
            {
                if (ex == null)
                {
                    _log.Warn(message);
                }
                else
                {
                    _log.Warn(message, ex);
                }
            }
        }

        public void Error(object message, Exception ex = null)
        {
            if (_log.IsErrorEnabled)
            {
                if (ex == null)
                {
                    _log.Error(message);
                }
                else
                {
                    _log.Error(message, ex);
                }
            }
        }

        public void Fatal(object message, Exception ex = null)
        {
            if (_log.IsFatalEnabled)
            {
                if (ex == null)
                {
                    _log.Fatal(message);
                }
                else
                {
                    _log.Fatal(message, ex);
                }
            }
        }
    }
}
