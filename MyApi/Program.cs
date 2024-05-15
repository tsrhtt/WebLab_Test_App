using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MyApi.Services;
using MyApi.Factories;
using MyApi.Data;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the DI container
var configuration = builder.Configuration;

// Setup HttpClient for manual fetching of the OpenID Connect configuration
var httpClient = new HttpClient();
var metadataUrl = configuration["Keycloak:Authority"] + "/.well-known/openid-configuration";
Console.WriteLine($"Keycloak Metadata URL: {metadataUrl}");

OpenIdConnectConfiguration openIdConfig = null;

try
{
    Console.WriteLine($"Attempting to fetch OpenID Connect configuration from: {metadataUrl}");
    var response = await httpClient.GetAsync(metadataUrl);
    var content = await response.Content.ReadAsStringAsync();

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine("Successfully fetched OpenID Connect configuration:");
        Console.WriteLine(content);
        openIdConfig = JsonConvert.DeserializeObject<OpenIdConnectConfiguration>(content);
        Console.WriteLine($"Issuer: {openIdConfig.Issuer}");
    }
    else
    {
        Console.WriteLine($"Failed to fetch OpenID Connect configuration. Status Code: {response.StatusCode}");
        Console.WriteLine("Response Content:");
        Console.WriteLine(content);
        throw new Exception($"Failed to fetch OpenID Connect configuration. Status Code: {response.StatusCode}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Exception occurred while fetching OpenID Connect configuration: {ex.Message}");
    throw; // Throw to stop the application if configuration cannot be loaded
}

if (openIdConfig == null)
{
    throw new InvalidOperationException("OpenID Configuration could not be loaded.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Configuration = openIdConfig;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = configuration["Keycloak:Authority"],
            ValidAudience = configuration["Keycloak:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            // TODO: I don't know if this is needed, found on the internet, maybe I should try it later
            // ValidateIssuerSigningKey = true,
            // IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
            // {
            //     var key = openIdConfig.JsonWebKeySet.GetSigningKeys().FirstOrDefault();
            //     return new[] { key };
            // }
        };

        // Event handlers for detailed logging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed!");
                Console.WriteLine(context.Exception.ToString());
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated!");
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                Console.WriteLine("Message received!");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine("Challenge issued!");
                return Task.CompletedTask;
            }
        };
    });

Console.WriteLine("Authentication added");

// Controllers with JSON options to handle circular references
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register WebLabService and WebLabServiceFactory
builder.Services.AddScoped<WebLabService>();
builder.Services.AddScoped<WebLabServiceFactory>();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:8080")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials(); // Consider if cookies/tokens are involved
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCors("AllowSpecificOrigins");
}

app.UseRouting();

// Log request headers for debugging
app.Use(async (context, next) =>
{
    Console.WriteLine("Request headers:");
    foreach (var header in context.Request.Headers)
    {
        Console.WriteLine($"{header.Key}: {header.Value}");
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
