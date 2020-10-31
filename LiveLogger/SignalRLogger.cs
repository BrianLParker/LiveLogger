namespace LiveLogger
{
    using System;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;

    public class SignalRLogger : ILogger
    {
        private readonly string _name;
        private readonly SignalRLoggerConfiguration _config;
        public SignalRLogger(string name, SignalRLoggerConfiguration config)
        {
            this._name = name;
            this._config = config;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel == this._config.LogLevel;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                        Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (this._config.EventId == 0 || this._config.EventId == eventId.Id)
            {
                try
                {
                    this._config.HubContext?.Clients.Group(_config.GroupName).SendAsync("Broadcast", "LOGGER", $"{DateTimeOffset.UtcNow:T}-UTC : {formatter(state, exception)}");
                }
                catch { } // todo
            }
        }
    }
}

