using Backend;
using Backend.SignalR;
using MediatR;
using System.Reflection;
using MassTransit;

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

builder.Services.AddSingleton<MessageSchedulerBackgroundService>();

//adding local SignalR
builder.Services.AddSignalR(hubOptions =>
    hubOptions.MaximumParallelInvocationsPerClient = 5);

//adding MediatR
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddMassTransit(x =>
{
    var assembly = Assembly.GetEntryAssembly();
    x.AddConsumers(assembly);

    x.UsingInMemory((ctx, cfg) =>
    {
        cfg.ConfigureEndpoints(ctx);
    });
});

//background service to periodically check for messages without answer
builder.Services.AddHostedService(s => s.GetService<MessageSchedulerBackgroundService>()!);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapHub<SignalRHub>("/signalrhub");

app.Run();
