namespace LiveLogger.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LiveLogger;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.SignalR.Client;
    using Microsoft.Extensions.Logging;

    public partial class ChatRoom
    {
        [Inject]
        private ILogger<ChatRoom> logger { get; set; }
        // flag to indicate chat status
        private bool _isChatting = false;

        // name of the user who will be chatting
        private string _username;

        // on-screen message
        private string _message;

        // new message input
        private string _newMessage;

        // list of messages in chat
        private readonly List<Message> _messages = new List<Message>();

        private string _hubUrl;
        private HubConnection _hubConnection;

        public async Task Chat()
        {
            // check username is valid
            if (string.IsNullOrWhiteSpace(this._username))
            {
                this._message = "Please enter a name";
                return;
            };

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
                await SendAsync($"[Notice] {this._username} joined chat room.");
                logger.LogInformation("Join chat room {0}", this._username);
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
            bool isMine = name.Equals(this._username, StringComparison.OrdinalIgnoreCase);

            this._messages.Add(new Message(name, message, isMine));

            // Inform blazor the UI needs updating
            StateHasChanged();
        }

        private async Task DisconnectAsync()
        {
            if (this._isChatting)
            {
                await SendAsync($"[Notice] {this._username} left chat room.");

                await this._hubConnection.StopAsync();
                await this._hubConnection.DisposeAsync();

                this._hubConnection = null;
                this._isChatting = false;
            }
        }

        private async Task SendAsync(string message)
        {
            if (this._isChatting && !string.IsNullOrWhiteSpace(message))
            {
                await this._hubConnection.SendAsync("Broadcast", this._username, message);
                this._newMessage = string.Empty;
            }
        }

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
            public string CSS => Mine ? "sent" : ( IsLog ? "log" : "received");
        }
    }
}