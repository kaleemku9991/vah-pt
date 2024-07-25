using System;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using proxyTask.Controllers;
using proxyTask.Model;
using System.Net.Http.Headers;



public class DialogflowService
    {
        private readonly HttpClient _httpClient;
    private readonly ILogger<CustomBotController> _logger;

    public DialogflowService(HttpClient httpClient, ILogger<CustomBotController> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

    }

    public async Task<dynamic> SendRequest(string userProjectId, dynamic jsonServiceAccount, string userInput, string sessionId)
    {
            //Create Google credentials using
            var credential = GoogleCredential.FromJson(jsonServiceAccount.ToString())
               .CreateScoped("https://www.googleapis.com/auth/cloud-platform");

            var uri = $"https://dialogflow.googleapis.com/v2/projects/{userProjectId}/agent/sessions/{sessionId}:detectIntent";

            var dialogflowRequest = new
            {
                query_input = new
                {
                    text = new
                    {
                        text = userInput,
                        language_code = "en-US"
                    }
                }
            };

            var jsonRequest = JsonConvert.SerializeObject(dialogflowRequest);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json")
            };

            var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var httpResponse = await _httpClient.SendAsync(httpRequest);

            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorResponse = await httpResponse.Content.ReadAsStringAsync();
                _logger.LogError("Error response from Dialogflow: {ErrorResponse}", errorResponse);
                return null;
            }

            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<dynamic>(jsonResponse);
        
    }

}

