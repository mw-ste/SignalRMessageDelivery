using Microsoft.AspNetCore.SignalR.Client;
using SignalRClient;

var closed = false;
string? name = null;
while (string.IsNullOrEmpty(name))
{
    Console.WriteLine("Enter your name: ");
    name = Console.ReadLine();
}

var connection = new HubConnectionBuilder()
    .WithUrl(new Uri("http://localhost:5000/signalrhub"))
    .Build();

var client = new SignalREchoClient(connection, name);
await client.ConnectToHub();
await client.RegisterClient();
client.ConnectionClosed += () =>
{
    closed = true;
    Console.WriteLine("Connection closed, press enter to exit");
};

while (!closed)
{
    Console.ReadLine();
}
