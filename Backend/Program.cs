using Backend;
using System.Reflection;
using Backend.SignalR;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddSingleton<ISenderDatabase>(new SenderInMemoryDatabase());

//adding local SignalR
builder.Services.AddSignalR();

//adding MediatR
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapHub<SignalRHub>("/signalrhub");

app.Run();
