using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MyApi.Services;
using MyApi.Factories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the DI container.
var configuration = builder.Configuration;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = configuration["Keycloak:Authority"];
        options.Audience = configuration["Keycloak:Audience"];
        options.RequireHttpsMetadata = false;
    });

// Add controllers.
builder.Services.AddControllers();

// Register WebLabService.
builder.Services.AddScoped<WebLabService>();
builder.Services.AddScoped<WebLabServiceFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
