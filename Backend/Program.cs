using Backend;
using Backend.SignalR;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddSingleton<ISenderDatabase>(new SenderInMemoryDatabase());
builder.Services.AddSingleton<IPendingMessageDatabase>(new PendingMessageInMemoryDatabase());

builder.Services.AddTransient<ISignalRGateway, SignalRGateway>();
builder.Services.AddTransient<ISignalRDispatcher, SignalRDispatcher>();

builder.Services.AddSingleton(s => new MessageSchedulerBackgroundService(
    s.GetService<IPendingMessageDatabase>()!,
    s.GetService<IHubContext<SignalRHub>>()!,
    s.GetService<IMediator>()!,
    s.GetService<ILogger<MessageSchedulerBackgroundService>>()!,
    s.GetService<ILogger<PendingMessage>>()!));

//adding local SignalR
builder.Services.AddSignalR(hubOptions =>
    hubOptions.MaximumParallelInvocationsPerClient = 5);

//adding MediatR
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

//background service to periodically check for messages without answer
builder.Services.AddHostedService(s => s.GetService<MessageSchedulerBackgroundService>()!);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapHub<SignalRHub>("/signalrhub");

app.Run();
