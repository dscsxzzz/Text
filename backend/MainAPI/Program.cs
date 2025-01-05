using FluentValidation;
using GenericServices.Setup;
using SharedModels.Dtos;
using SharedModels.Models;
using SharedModels.Validation;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DatabaseContext>();
builder.Services.AddValidatorsFromAssemblyContaining<UserCreateDtoValidator>();
builder.Services.GenericServicesSimpleSetup<DatabaseContext>(
   Assembly.GetAssembly(typeof(UserCreateDto)));
var port = Environment.GetEnvironmentVariable("SERVICE_PORT") ?? "8082";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
