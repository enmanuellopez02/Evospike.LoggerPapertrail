using System;
namespace Evospike.LoggerPapertrail.Logging
{
    public class NoopDisposable : IDisposable
    {
        public static NoopDisposable Instance = new();

        public void Dispose()
        {
        }
    }
}