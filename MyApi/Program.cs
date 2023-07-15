using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MyApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create a configuration object that reads from appsettings.json
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Get the data from the Keycloak section of the config file
            var clientId = configuration["Keycloak:ClientId"];
            var clientSecret = configuration["Keycloak:ClientSecret"];
            var authority = configuration["Keycloak:Authority"];
            var tokenUrl = configuration["Keycloak:TokenUrl"];
            var audience = configuration["Keycloak:Audience"];

            // Add the JWT Bearer authentication scheme using the data from the config file
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = authority;
                    options.Audience = audience;
                    options.RequireHttpsMetadata = false;
                });

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
