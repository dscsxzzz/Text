using System.Reflection;
using AIModelSenderSerice;
using FluentValidation;
using FluentValidation.AspNetCore;
using GenericServices.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using SharedModels.Dtos;
using SharedModels.Models;
using SharedModels.Validation;

var builder = WebApplication.CreateBuilder(args);

// RabbitMQ Connection
var factory = new ConnectionFactory() { HostName = "rabbitmq" };
var connection = await factory.CreateConnectionAsync();
builder.Services.AddSingleton(connection);
builder.Services.AddDbContext<DatabaseContext>();
builder.Services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining(
                typeof(UserCreateDto)
            );
builder.Services.GenericServicesSimpleSetup<DatabaseContext>(
   Assembly.GetAssembly(typeof(UserCreateDto)));

// Add SignalR
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:8000") // Your frontend URL
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials(); // Required for SignalR
    });
});

builder.Services.AddSingleton<FrontendSender>();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);
var port = Environment.GetEnvironmentVariable("SERVICE_PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
var app = builder.Build();
app.UseCors(); // Enable CORS for the app

var listener = app.Services.GetRequiredService<FrontendSender>();
await listener.CreateChannel();
await listener.StartListeningToEventQueue();

app.MapHub<FrontendSender>("/senderhub");

app.Run();
