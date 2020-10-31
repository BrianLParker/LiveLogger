namespace LiveLogger.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    using LiveLogger;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.SignalR.Client;

    public partial class LogMonitor
    {
        [Inject]
        private ILogger<ChatRoom> logger { get; set; }
        // flag to indicate chat status
        private bool _isChatting = false;

        // on-screen message
        private string _message;

        // list of messages in chat
        private readonly List<Message> _messages = new List<Message>();

        private string _hubUrl;
        private HubConnection _hubConnection;
        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await Chat();
        }

        public async Task Chat()
        {


            try
            {
                // Start chatting and force refresh UI.
                this._isChatting = true;
                await Task.Delay(1);

                // remove old messages if any
                this._messages.Clear();

                // Create the chat client
                string baseUrl = navigationManager.BaseUri;

                this._hubUrl = baseUrl.TrimEnd('/') + BlazorChatSampleHub.HubUrl;

                this._hubConnection = new HubConnectionBuilder()
                    .WithUrl(this._hubUrl)
                    .Build();

                this._hubConnection.On<string, string>("Broadcast", BroadcastMessage);

                await this._hubConnection.StartAsync();
                await this._hubConnection.SendAsync("JoinGroup", "LogMonitor");

            }
            catch (Exception e)
            {
                this._message = $"ERROR: Failed to start chat client: {e.Message}";
                logger.LogInformation(this._message);
                this._isChatting = false;
            }
        }



        private void BroadcastMessage(string name, string message)
        {
            if (name != "LOGGER") return;

            this._messages.Insert(0,new Message(name, message, false));
            // Inform blazor the UI needs updating
            StateHasChanged();
        }

        private async Task DisconnectAsync()
        {
            if (this._isChatting)
            {
                await this._hubConnection.StopAsync();
                await this._hubConnection.DisposeAsync();

                this._hubConnection = null;
                this._isChatting = false;
            }
        }

        //private async Task SendAsync(string message)
        //{
        //    if (this._isChatting && !string.IsNullOrWhiteSpace(message))
        //    {
        //        await this._hubConnection.SendAsync("Broadcast", this._username, message);
        //        this._newMessage = string.Empty;
        //    }
        //}

        private class Message
        {
            public Message(string username, string body, bool mine)
            {
                Username = username;
                Body = body;
                Mine = mine;
            }

            public string Username { get; set; }
            public string Body { get; set; }
            public bool Mine { get; set; }
            public bool IsNotice => Body.StartsWith("[Notice]");
            public bool IsLog => Username == "LOGGER";
            public string CSS => Mine ? "sent" : (IsLog ? "log" : "received");
        }
    }
}