using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace MyApi.Services
{
    public class WebLabService : IWebLabService
    {
        private readonly HttpClient _client;

        public WebLabService(HttpClient client)
        {
            _client = client;
        }

        public async Task<LabData> GetLabData()
        {
            // Create a request object for the resource you want to access
            var request = new HttpRequestMessage(HttpMethod.Get, "lab-data");

            // Execute the request and get the response
            var response = await _client.SendAsync(request);

            // Check if the response is successful
            if (response.IsSuccessStatusCode)
            {
                // Return the data from the response
                return await response.Content.ReadAsAsync<LabData>();
            }
            else
            {
                // Handle the error
                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
