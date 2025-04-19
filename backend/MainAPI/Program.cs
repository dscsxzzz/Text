using FluentValidation;
using FluentValidation.AspNetCore;
using GenericServices.Setup;
using MainAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SharedModels.Dtos;
using SharedModels.Helpers;
using SharedModels.Mappings;
using SharedModels.Models;
using SharedModels.Validation;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DatabaseContext>();
builder.Services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining(
                typeof(UserCreateDto)
            );
builder.Services.GenericServicesSimpleSetup<DatabaseContext>(
   Assembly.GetAssembly(typeof(UserCreateDto)), Assembly.GetAssembly(typeof(MessageCreateDto)));

builder.Services.AddAutoMapper(typeof(UserReadSimpleDtoMapping));
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<CacheService>();
builder.Services.AddSingleton<JwtHelper>();
builder.Services.AddSingleton<EmailService>();
var port = Environment.GetEnvironmentVariable("SERVICE_PORT") ?? "8082";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:8000", "http://localhost:8001") // Your frontend URL
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var section = builder.Configuration.GetSection("JwtSettings");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(section["SecretKey"]!)),
            ValidIssuer = section["Issuer"],
            ValidAudience = section["Audience"],
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero 
        };
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
