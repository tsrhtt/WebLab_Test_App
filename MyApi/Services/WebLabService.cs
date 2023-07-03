using System;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;

namespace MyApi.Services
{
    public class WebLabService
    {
        private readonly string _baseUrl = "https://web-lab-api.com"; // Replace with the actual base URL
        private readonly string _clientId = "placeholder"; // Replace with the actual client ID
        private readonly string _clientSecret = "placeholder"; // Replace with the actual client secret
        private readonly string _authUrl = "https://keycloak.com"; // Replace with the actual auth URL

        private readonly RestClient _client;

        public WebLabService()
        {
        _client = new RestClient(_baseUrl);
        _client.Authenticator = new JwtAuthenticator(_authUrl, _clientId, _clientSecret);
        }

        public async Task<string> GetDirectionsAsync()
        {
            var request = new RestRequest("directions", Method.GET); // Replace "directions" with the actual endpoint

            var response = await _client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                return response.Content;
            }
            else
            {
                return null;
            }

        }



    }
}
