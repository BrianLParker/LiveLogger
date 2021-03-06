﻿namespace LiveLogger
{
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;

    public class SignalRLoggerConfiguration
    {
        public IHubContext<BlazorChatSampleHub> HubContext { get; set; }
        public string GroupName { get; set; } = "LogMonitor";
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
        public int EventId { get; set; } = 0;
    }
}
