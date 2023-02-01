using Backend.Database;
using Backend.SignalR;
using MassTransit;
using MediatR;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddSingleton<ISenderDatabase, SenderInMemoryDatabase>();
builder.Services.AddSingleton<IPendingMessageDatabase, PendingMessageInMemoryDatabase>();

builder.Services.AddScoped<IPendingMessageRepository, PendingMessageRepository>();
builder.Services.AddScoped<ISenderRepository, SenderRepository>();

builder.Services.AddScoped<ISignalRGateway, SignalRGateway>();
builder.Services.AddScoped<ISignalRDispatcher, SignalRDispatcher>();

//adding local SignalR
builder.Services.AddSignalR(hubOptions =>
    hubOptions.MaximumParallelInvocationsPerClient = 5);

builder.Services.AddMassTransit(x =>
{
    x.AddDelayedMessageScheduler();
    x.AddConsumers(Assembly.GetExecutingAssembly());

    x.UsingInMemory((ctx, cfg) =>
    {
        cfg.UseDelayedMessageScheduler();
        cfg.ConfigureEndpoints(ctx);
    });
});

//adding MediatR
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapHub<SignalRHub>("/signalrhub");

app.Run();
