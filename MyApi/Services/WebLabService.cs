using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            //Console.WriteLine($"Access token: {_token}"); // Log the access token value
        }

        public async Task<IEnumerable<Direction>> GetLabData()
        {
            if (_token == null) throw new InvalidOperationException("Service not initialized with token.");

            Console.WriteLine($"Using token: {_token} for data fetch.");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            Console.WriteLine("Attempting to fetch lab data...");

            var response = await _client.GetAsync("Direction/");
            Console.WriteLine($"HTTP GET request sent to {_client.BaseAddress}Direction/");

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Request failed with status code {response.StatusCode}: {responseContent}");
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}, Content: {responseContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response content received, attempting to deserialize...");

            try
            {
                var responseData = JsonSerializer.Deserialize<ResponseWrapper>(content);
                if (responseData == null || responseData.Data == null)
                {
                    Console.WriteLine("No data received or failed to deserialize.");
                    return new List<Direction>(); // Return empty list if deserialization fails
                }

                Console.WriteLine($"Data successfully deserialized. Count: {responseData.Data.Count()} items received.");
                return responseData.Data;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JsonException encountered: {ex.Message}");
                throw;
            }
        }

        private class ResponseWrapper
        {
            [JsonPropertyName("data")]
            public IEnumerable<Direction>? Data { get; set; }  // Make Data nullable
        }
    }
}
