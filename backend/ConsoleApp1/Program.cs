using System.Reflection;
using AIModelReceiverService;
using ConsoleApp1;
using FluentValidation;
using GenericServices.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SharedModels.Dtos;
using SharedModels.Models;
using SharedModels.Validation;

var builder = WebApplication.CreateBuilder(args);

// RabbitMQ Connection
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
builder.Services.AddSingleton<Class1>();
var app = builder.Build();
app.UseCors(); // Enable CORS for the app

// Map SignalR hub
var listener = app.Services.GetRequiredService<Class1>();
listener.log();
app.MapHub<Class1>("/receiverhub");

app.Run();
