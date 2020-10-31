namespace LiveLogger.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    using System.Net.Http;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.AspNetCore.Components.Routing;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.JSInterop;
    using LiveLogger;
    using LiveLogger.Shared;
    using Microsoft.AspNetCore.SignalR.Client;
    using Microsoft.Extensions.Logging;

    public partial class ChatRoom
    {
        [Inject]
        ILogger<ChatRoom> logger { get; set; }
        // flag to indicate chat status
        private bool _isChatting = false;

        // name of the user who will be chatting
        private string _username;

        // on-screen message
        private string _message;

        // new message input
        private string _newMessage;

        // list of messages in chat
        private List<Message> _messages = new List<Message>();

        private string _hubUrl;
        private HubConnection _hubConnection;

        public async Task Chat()
        {
            // check username is valid
            if (string.IsNullOrWhiteSpace(_username))
            {
                _message = "Please enter a name";
                return;
            };

            try
            {
                // Start chatting and force refresh UI.
                _isChatting = true;
                await Task.Delay(1);

                // remove old messages if any
                _messages.Clear();

                // Create the chat client
                string baseUrl = navigationManager.BaseUri;

                _hubUrl = baseUrl.TrimEnd('/') + BlazorChatSampleHub.HubUrl;

                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_hubUrl)
                    .Build();

                _hubConnection.On<string, string>("Broadcast", BroadcastMessage);

                await _hubConnection.StartAsync();

                await SendAsync($"[Notice] {_username} joined chat room.");
            }
            catch (Exception e)
            {
                _message = $"ERROR: Failed to start chat client: {e.Message}";
                _isChatting = false;
            }
        }

        private void BroadcastMessage(string name, string message)
        {
            bool isMine = name.Equals(_username, StringComparison.OrdinalIgnoreCase);

            _messages.Add(new Message(name, message, isMine));

            // Inform blazor the UI needs updating
            StateHasChanged();
        }

        private async Task DisconnectAsync()
        {
            if (_isChatting)
            {
                await SendAsync($"[Notice] {_username} left chat room.");

                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();

                _hubConnection = null;
                _isChatting = false;
            }
        }

        private async Task SendAsync(string message)
        {
            if (_isChatting && !string.IsNullOrWhiteSpace(message))
            {
                await _hubConnection.SendAsync("Broadcast", _username, message);
                logger.LogInformation($"{_username} : {message}");
                _newMessage = string.Empty;
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

            public string CSS => Mine ? "sent" : "received";
        }
    }
}