using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MyApi.Services;

namespace MyApi.Factories
{
    public class WebLabServiceFactory
    {
        private readonly IConfiguration _configuration;

        public WebLabServiceFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<WebLabService> CreateAsync()
        {
            var service = new WebLabService(_configuration);
            await service.InitializeAsync(_configuration);
            return service;
        }
    }
}