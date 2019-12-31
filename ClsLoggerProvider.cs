using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace MicroFeel.CLS
{
    public class ClsLoggerProvider : ILoggerProvider
    {
        private readonly ClsSetting settings;
        private readonly ConcurrentDictionary<string, CLSLogger> _loggers = new ConcurrentDictionary<string, CLSLogger>();
        private IExternalScopeProvider _scopeProvider;

        public ClsLoggerProvider(ClsSetting clsSetting)
        {
            settings = clsSetting;
        }

        public ILogger CreateLogger(string categoryName)
        {
            _ = new ClsClient(settings.Endpoint, settings.SecretId, settings.SecretKey);
            return _loggers.GetOrAdd(categoryName, name => new CLSLogger(categoryName, settings, _scopeProvider));
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            ClsClient.Client = null;
            _loggers.Clear();
        }
    }
}
