using System;
using Microsoft.Extensions.Logging;

namespace Evospike.LoggerPapertrail.Logging
{
    public class LoggerPapertrailProvider : ILoggerProvider
    {
        private string _host;
        private int _port;
        private readonly Func<string, LogLevel, bool> _filter;

        public LoggerPapertrailProvider(string host, int port, Func<string, LogLevel, bool> filter)
        {
            _host = host;
            _port = port;
            _filter = filter;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new LoggerPapertrail(categoryName, _host, _port, _filter);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}