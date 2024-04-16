using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MyApi.Models;

namespace MyApi.Services
{
    public class WebLabService
    {
        private readonly HttpClient _client;
        private string? _token; // Make _token nullable

        public WebLabService(IConfiguration configuration)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://public.ehealth.by/lab-test/api/integration/")
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task InitializeAsync(IConfiguration configuration)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://public.ehealth.by/lab-test/keycloak/realms/laboratory/protocol/openid-connect/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"client_id", configuration["Keycloak:ClientId"]}, // Adjusted for appsettings
                    {"client_secret", configuration["Keycloak:ClientSecret"]}, // Adjusted for appsettings
                    {"grant_type", "client_credentials"}
                })
            };
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content);
            _token = tokenResponse?.AccessToken; // Handle possible null deserialization
        }

        public async Task<IEnumerable<Direction>> GetLabData()
        {
            if (_token == null) throw new InvalidOperationException("Service not initialized with token.");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await _client.GetAsync("direction/");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var directions = JsonSerializer.Deserialize<IEnumerable<Direction>>(content) ?? new List<Direction>(); // Handle possible null deserialization
            return directions;
        }
    }
}
