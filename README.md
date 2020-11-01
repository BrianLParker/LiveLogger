# LiveLogger
Somebody wanted live logging

This can be achieved by injecting a Signalr HubContext into a custom Logger. `IHubContext<BlazorChatSampleHub>`

Note: As this was a Blazor server project. The code in the .razo pages executes on the server so logging worked here as well.
To achieve the same on a blazor wasm client the client would have to inject a hub connection not a context.
