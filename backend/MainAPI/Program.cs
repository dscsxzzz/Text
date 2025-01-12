using FluentValidation;
using GenericServices.Setup;
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
builder.Services.AddValidatorsFromAssemblyContaining<UserCreateDtoValidator>();
builder.Services.GenericServicesSimpleSetup<DatabaseContext>(
   Assembly.GetAssembly(typeof(UserCreateDto)), Assembly.GetAssembly(typeof(MessageCreateDto)));

builder.Services.AddAutoMapper(typeof(UserReadSimpleDtoMapping));

builder.Services.AddSingleton<JwtHelper>();

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
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // The key you used to sign the token
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKeyHere")),  // Replace with your key
            ValidateIssuer = false, // You can set this to true and specify a valid issuer if needed
            ValidateAudience = false, // You can set this to true and specify a valid audience if needed
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // Set clock skew to zero to prevent token expiration issues
        };
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
