using System;
using log4net;
using Trader.Domain.Infrastucture;

namespace Trader.Client.Infrastucture
{
    public class Log4NetLogger : ILogger
    {
        private readonly ILog _log;

        public Log4NetLogger(Type type)
        {
            _log = LogManager.GetLogger(type);
        }

        public void Debug(string message, params object[] values)
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat(message, values);
            }

        }

        public void Info(string message, params object[] values)
        {
            if (_log.IsInfoEnabled)
            {
                _log.InfoFormat(message, values);
            }
        }

        public void Warn(string message, params object[] values)
        {
            if (_log.IsWarnEnabled)
            {
                _log.WarnFormat(message, values);
            }
        }

        public void Error(Exception ex, string message, params object[] values)
        {
            if (_log.IsErrorEnabled)
            {
                _log.Error(string.Format(message, values), ex);
            }
        }

        public void Fatal(string message, params object[] values)
        {
            if (_log.IsFatalEnabled)
            {
                _log.FatalFormat(message, values);
            }
        }

        // other logging methods here...

    }
}