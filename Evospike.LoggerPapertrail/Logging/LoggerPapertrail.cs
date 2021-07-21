using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Evospike.LoggerPapertrail.Enums;
using Microsoft.Extensions.Logging;

namespace Evospike.LoggerPapertrail.Logging
{
    public class LoggerPapertrail : ILogger
    {
        private const int SyslogFacility = 16;
        private string _categoryName;
        private string _host;
        private int _port;
        private readonly Func<string, LogLevel, bool> _filter;

        public LoggerPapertrail(string categoryName, string host, int port, Func<string, LogLevel, bool> filter)
        {
            _categoryName = categoryName;
            _host = host;
            _port = port;
            _filter = filter;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (_filter == null || _filter(_categoryName, logLevel));
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = $"{ logLevel }: {message}";

            if (exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + exception.ToString();
            }

            var syslogLevel = MapToSyslogLevel(logLevel);
            Send(syslogLevel, message);
        }

        internal void Send(LogLevelPapertrail logLevel, string message)
        {
            if (string.IsNullOrWhiteSpace(_host) || _port <= 0)
            {
                return;
            }

            var hostName = Dns.GetHostName();
            var level = SyslogFacility * 8 + (int)logLevel;
            var logMessage = string.Format("<{0}>{1} {2}", level, hostName, message);
            var bytes = Encoding.UTF8.GetBytes(logMessage);

            using (var client = new UdpClient())
            {
                client.SendAsync(bytes, bytes.Length, _host, _port).Wait();
            }
        }

        private LogLevelPapertrail MapToSyslogLevel(LogLevel level)
        {
            if (level == LogLevel.Critical)
                return LogLevelPapertrail.Critical;
            if (level == LogLevel.Debug)
                return LogLevelPapertrail.Debug;
            if (level == LogLevel.Error)
                return LogLevelPapertrail.Error;
            if (level == LogLevel.Information)
                return LogLevelPapertrail.Info;
            if (level == LogLevel.None)
                return LogLevelPapertrail.Info;
            if (level == LogLevel.Trace)
                return LogLevelPapertrail.Info;
            if (level == LogLevel.Warning)
                return LogLevelPapertrail.Warn;

            return LogLevelPapertrail.Info;
        }
    }
}