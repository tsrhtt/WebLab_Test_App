using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MyApi.Services;
using MyApi.Factories;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the DI container.
var configuration = builder.Configuration;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    // AddJwtBearer options
    .AddJwtBearer(options =>
    {
        options.Authority = configuration["Keycloak:Authority"];
        options.Audience = configuration["Keycloak:Audience"];
        options.RequireHttpsMetadata = false;
        options.MetadataAddress = configuration["Keycloak:Authority"] + "/.well-known/openid-configuration";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = configuration["Keycloak:Audience"],
            ValidIssuer = configuration["Keycloak:Authority"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Add controllers.
builder.Services.AddControllers();

// Register WebLabService.
builder.Services.AddScoped<WebLabService>();
builder.Services.AddScoped<WebLabServiceFactory>();

// Add CORS services.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:8080")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCors("AllowSpecificOrigins");
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
