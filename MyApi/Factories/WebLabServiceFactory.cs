using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MyApi.Services;
using MyApi.Data;

namespace MyApi.Factories
{
    public class WebLabServiceFactory
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;

        // Inject AppDbContext here
        public WebLabServiceFactory(IConfiguration configuration, AppDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<WebLabService> CreateAsync()
        {
            var service = new WebLabService(_configuration, _dbContext);
            await service.InitializeAsync(_configuration);
            return service;
        }
    }
}