# Guaranteed SignalR Message Delivery


## Typed HubContext

* `IHubContext<SignalRHub, ISignalRClient> _typedHubContext;`
* **Advantage:** has a return value, can be used to determine success
* **Problem:** needs to run synchronously? Is blocking in the contract?
* `var result = await _typedHubContext.Clients.Group(...).SpecificMethod(...);`
* **Problem:** System.InvalidOperationException: InvokeAsync only works with Single clients.
* `var result = await _typedHubContext.Clients.Client(...).SpecificMethod(...);`
* **Problem:** Not easy to get connection id for single client

## HubContext Invoke(Core)Async


## Sequence Diagram

* [Hub Connection is locked!](https://github.com/dotnet/aspnetcore/blob/888c71f7f2269b6e67a24924cef66228857236f3/src/SignalR/server/Core/src/HubConnectionContext.cs#L341)

```mermaid
sequenceDiagram
    participant be as Backend
    participant ctx as HubContext
    participant hub as Hub
    participant signalr as SignalR
    participant conn as HubConnection
    participant client as Client

    be ->> ctx : SendAsync("Method")
    activate be
    activate ctx
    ctx ->> signalr : "Method"
    activate signalr
    ctx -->> be : 
    deactivate ctx
    deactivate be

    signalr ->> conn : "Method"
    activate conn
    Note over signalr,conn: queues further<br/>invocations for<br/>this connection
    conn ->> client : Method()
    activate client
    client ->> client : do stuff
    client ->> conn : SendAsync("Answer")
    activate conn

    conn ->> signalr : "Answer"
    signalr ->> hub : Answer()
    activate hub
    signalr -->> conn : 
    conn -->> client : 
    deactivate client
    deactivate conn

    hub ->> be : ProcessAnswer()

    activate be
    be ->> be : do stuff
    be -->> hub : 
    hub -->> signalr : 
    deactivate hub
    deactivate be
    deactivate signalr
```