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
        private readonly string _token;

        public WebLabService(IConfiguration configuration)
        {
            // Initialize the HttpClient with some settings
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://public.ehealth.by/lab-staging/api/integration/");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Make the request to the third party API to get the token
            var request = new HttpRequestMessage(HttpMethod.Post, "https://public.ehealth.by/lab-staging/keycloak/realms/laboratory/protocol/openid-connect/token");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"client_id", configuration["ClientId"]},
                {"client_secret", configuration["ClientSecret"]},
                {"grant_type", "client_credentials"}
            });
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content);
            _token = tokenResponse.AccessToken;
        }

        public async Task<IEnumerable<Direction>> GetDirectionsAsync()
        {
            // Add the access token as an Authorization header
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            // Make the request to the third party API to get the directions
            var response = await _client.GetAsync("direction/");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var directions = JsonSerializer.Deserialize<IEnumerable<Direction>>(content);
            return directions;
        }
    }
}
