using System;
using System.Diagnostics;
using System.Text;

namespace TransactionalNodeService.Proxy.Universal
{
    public class DebugLogger
    {
        private StringBuilder _logBuilder = null;

        private DebugLogger()
        {
        }

        #region Singleton Code
        private static readonly DebugLogger _logger = new DebugLogger();

        static DebugLogger()
        {
        }

        public static DebugLogger Instance
        {
            get
            {
                return _logger;
            }
        }
        #endregion

        private StringBuilder LogBuilder
        {
            get
            {
                if (_logBuilder == null)
                {
                    _logBuilder = new StringBuilder();

                    _logBuilder.AppendFormat(@"""Timestamp"",""Class"",""Method"",""Message""");
                    _logBuilder.AppendLine();
                }

                return _logBuilder;
            }
        }

        public string Log
        {
            get
            {
                return LogBuilder.ToString();
            }
        }

        public void LogMsg(string message)
        {
            StackFrame stackFrame = new StackFrame(1);

            DateTime timestamp = DateTime.Now.ToUniversalTime();

            string csvFormattedMessage = message.Replace(@"""", @"""""");

            LogBuilder.AppendFormat(@"""[{0}]"",""[{1}]"",""[{2}]"",""{3}""", timestamp.ToLongTimeString(), stackFrame.GetMethod().DeclaringType.ToString(), stackFrame.GetMethod().Name, csvFormattedMessage);
            LogBuilder.AppendLine();
        }

        public void LogMsg(string message, params object[] args)
        {
            StackFrame stackFrame = new StackFrame(1);

            DateTime timestamp = DateTime.Now.ToUniversalTime();

            string csvFormattedMessage = string.Format(message, args).Replace(@"""", @"""""");

            LogBuilder.AppendFormat(@"""[{0}]"",""[{1}]"",""[{2}]"",""{3}""", timestamp.ToLongTimeString(), stackFrame.GetMethod().DeclaringType.ToString(), stackFrame.GetMethod().Name, csvFormattedMessage);
            LogBuilder.AppendLine();
        }
    }
}
