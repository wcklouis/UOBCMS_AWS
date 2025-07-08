using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Humanizer;
using Microsoft.Identity.Client;

namespace UOBCMS.Classes
{
    public class CloudswayApiClient
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public CloudswayApiClient(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<string> ExtractAddressLinesFromBill(byte[] imageData)
        {
            
            var prompt = @"
            Can you extract the client address into 4 lines and no need to capture client from the image?
            ";

            try
            {
                var value = new
                {
                    type = "image",
                    image = new
                    {
                        base64 = imageData
                    }
                };

                var payload = new
                {
                    model = "Maas 4o 2024-1120", // Replace with the appropriate model name
                    messages = new[]
                    {
                        new { role = "user", content = value }
                    },
                    prompt = prompt,
                    stream = false,
                    stream_options = new
                    {
                        include_usage = true
                    }
                };

                /*var payload = new
                {
                    model = "Maas 4o 2024-1120", // Replace with the appropriate model name
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    },
                    image_data = Convert.ToBase64String(imageData), // Pass the image data as base64
                    prompt = prompt,
                    temperature = 0.7 // Adjust for creativity
                };*/



                string jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpResponseMessage response = await _httpClient.PostAsync(
                    "https://genaiapi.cloudsway.net/v1/ai/ibucTXViBwDinjvD/chat/completions", content
                );

                if (!response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Error: " + response.StatusCode);
                    Console.WriteLine("Response Body: " + responseBody);
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return null;
            }
        }

        public async Task<string> ChangeAddressTo4LinesInJson(string fullAddress)
        {
            try
            {
                var payload = new
                {
                    model = "Maas-DS-V3-Limited",  // Replace with the appropriate model name
                    messages = new[]
                    {
                        new { role = "user", content = $@"
                            Please divide the following address into 4 lines:
                            {fullAddress}

                            JSON fields required:
                            - Address_Line_1
                            - Address_Line_2
                            - Address_Line_3
                            - Address_Line_4
                        " }
                    },
                    output_format = "json"
                };

                string jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpResponseMessage response = await _httpClient.PostAsync(
                    "https://genaiapi.cloudsway.net/v1/ai/RqWjzAxTwlSxVsyw/chat/completions", content
                );

                if (!response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Error: " + response.StatusCode);
                    Console.WriteLine("Response Body: " + responseBody);
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return null;
            }
        }
    }
}
