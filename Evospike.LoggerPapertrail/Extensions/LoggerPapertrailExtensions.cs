using System;
using Evospike.LoggerPapertrail.Logging;
using Evospike.LoggerPapertrail.Settings;
using Microsoft.Extensions.Logging;

namespace Evospike.LoggerPapertrail.Extensions
{
    public static class LoggerPapertrailExtensions
    {
        public static ILoggerFactory AddLoggerPapertrail(this ILoggerFactory factory, PapertrailSetting papertrailSetting, Func<string, LogLevel, bool> filter = null)
        {
            factory.AddProvider(new LoggerPapertrailProvider(papertrailSetting.Host, papertrailSetting.Port, filter));
            return factory;
        }
    }
}