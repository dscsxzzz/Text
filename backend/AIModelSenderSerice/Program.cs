using System.Reflection;
using AIModelSenderSerice;
using FluentValidation;
using GenericServices.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
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
builder.Services.AddValidatorsFromAssemblyContaining<UserCreateDtoValidator>();
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

// Add RabbitMQ Listener
builder.Services.AddSingleton<FrontendSender>();
var port = Environment.GetEnvironmentVariable("SERVICE_PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
var app = builder.Build();
app.UseCors(); // Enable CORS for the app

// Start RabbitMQ Listener
var listener = app.Services.GetRequiredService<FrontendSender>();
await listener.CreateChannel();
await listener.StartListeningToEventQueue();

// Map SignalR hub
app.MapHub<FrontendSender>("/senderhub");

app.Run();
