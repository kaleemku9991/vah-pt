using System;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using proxyTask.Controllers;
using proxyTask.Model;
using System.Net.Http.Headers;
using static Google.Apis.Requests.BatchRequest;
using Newtonsoft.Json.Linq;
using Google.Protobuf.WellKnownTypes;
using static proxyTask.Model.ExternalIntegrationBotExchangeRequest;



public class DialogflowService
    {
        private readonly HttpClient _httpClient;
    private readonly ILogger<CustomBotController> _logger;

    public DialogflowService(HttpClient httpClient, ILogger<CustomBotController> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

    }

    public async Task<dynamic> SendRequest(string userProjectId, dynamic jsonServiceAccount, string userInput, string sessionId, string customPayload, UserInputType userInputType)
    {
        string jsonResponse = null;
        try
        {
            // Create Google credentials using the provided service account JSON
            var credential = GoogleCredential.FromJson(jsonServiceAccount.ToString())
                .CreateScoped("https://www.googleapis.com/auth/cloud-platform");

            // Construct the Dialogflow API endpoint URL
            var uri = $"https://dialogflow.googleapis.com/v2/projects/{userProjectId}/agent/sessions/{sessionId}:detectIntent";

            // Parse the custom payload JSON into a Struct object if it is not null or empty
            Struct payloadStruct = null;
            if (!string.IsNullOrEmpty(customPayload))
            {
                payloadStruct = Google.Protobuf.WellKnownTypes.Struct.Parser.ParseJson(customPayload);
            }

            // Define the request object to be sent to Dialogflow
            object dialogflowRequest;

            // Determine the type of user input and structure the request accordingly
            if (userInputType == UserInputType.AUTOMATED_TEXT)
            {
                // For automated text, use an event to trigger the specific intent
                dialogflowRequest = new
                {
                    query_input = new
                    {
                        @event = new
                        {
                            name = userInput, // The name of the event to trigger the intent
                            language_code = "en-US"
                        }
                    }
                };

            }
            else
            {
                // For normal user text, send the text input as usual
                dialogflowRequest = new
                {
                    query_input = new
                    {
                        text = new
                        {
                            text = userInput,
                            language_code = "en-US"
                        }
                    },
                    query_params = new
                    {
                        payload = payloadStruct
                    }
                };
            }

            // Serialize the request object to JSON
            var jsonRequest = JsonConvert.SerializeObject(dialogflowRequest);

            // Create the HTTP request message
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json")
            };

            // Add the authorization header with the access token
            var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Send the HTTP request and await the response
            var httpResponse = await _httpClient.SendAsync(httpRequest);

            // Handle the HTTP response
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorResponse = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine("Error response from Dialogflow: " + errorResponse);
                return null;
            }

            // Read and deserialize the response content
            jsonResponse = await httpResponse.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        // Return the deserialized response
        return JsonConvert.DeserializeObject<dynamic>(jsonResponse);
    }

}

