namespace LiveLogger
{
    using System.Collections.Concurrent;
    using Microsoft.Extensions.Logging;

    public class SignalRLoggerProvider : ILoggerProvider
    {
        private readonly SignalRLoggerConfiguration _config;
        private readonly ConcurrentDictionary<string, SignalRLogger> _loggers = new ConcurrentDictionary<string, SignalRLogger>();

        public SignalRLoggerProvider(SignalRLoggerConfiguration config)
            => this._config = config;

        public ILogger CreateLogger(string categoryName)
            => this._loggers.GetOrAdd(categoryName, name => new SignalRLogger(name, this._config));

        public void Dispose() 
            => this._loggers.Clear();
    }
}
