using System.Reflection;
using AIModelReceiverService;
using FluentValidation;
using FluentValidation.AspNetCore;
using GenericServices.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using SharedModels.Dtos;
using SharedModels.Models;
using SharedModels.Validation;

var builder = WebApplication.CreateBuilder(args);

var factory = new ConnectionFactory() { HostName = "rabbitmq" };
var connection = await factory.CreateConnectionAsync();
builder.Services.AddSingleton(connection);
builder.Services.AddDbContext<DatabaseContext>();
builder.Services.AddSingleton<FrontendReceiver>();
builder.Services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining(
                typeof(UserCreateDto)
            );
builder.Services.GenericServicesSimpleSetup<DatabaseContext>(
   Assembly.GetAssembly(typeof(UserCreateDto)));
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:8000")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});
var port = Environment.GetEnvironmentVariable("SERVICE_PORT") ?? "8081";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
var app = builder.Build();
app.UseCors();

app.MapHub<FrontendReceiver>("/receiverhub");

app.Run();
