using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyApi.Models;
using Newtonsoft.Json;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public TestController(ILogger<TestController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IEnumerable<Test>> Get()
        {
            var tests = await GetTestsAsync();
            return tests;
        }

        private async Task<string> GetTokenAsync()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://keycloak.com"); // Need to change this to the Keycloak URL
            if (disco.IsError) throw new Exception(disco.Error);

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client-id", // change this to client id
                ClientSecret = "client-secret", // change this to client secret
                Scope = "scope" // change this to the needed scope
            });

            if (tokenResponse.IsError) throw new Exception(tokenResponse.Error);

            return tokenResponse.AccessToken;
        }

        private async Task<IEnumerable<Test>> GetTestsAsync()
        {
            var client = _httpClientFactory.CreateClient("ThirdParty");
            var token = await GetTokenAsync();
            client.SetBearerToken(token);
            var response = await client.GetAsync("/tests");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var tests = JsonConvert.DeserializeObject<IEnumerable<Test>>(content);
            return tests;
        }
    }
}