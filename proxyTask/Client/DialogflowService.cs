﻿using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using proxyTask.Controllers;
using System.Net.Http.Headers;
using Google.Protobuf.WellKnownTypes;
using static proxyTask.Model.ExternalIntegrationBotExchangeRequest;



public class DialogflowService
    {
        private readonly HttpClient _httpClient;
    private readonly ILogger<CustomBotController> _logger;

    /// <summary>
    /// Initializes a new instance of the DialogflowService class.
    /// </summary>
    /// <param name="httpClient">The HttpClient used for sending HTTP requests.</param>
    public DialogflowService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    /// <summary>
    /// Sends a request to the Dialogflow API to detect user intent based on the provided input.
    /// </summary>
    /// <param name="userProjectId">The project ID associated with the Dialogflow agent.</param>
    /// <param name="jsonServiceAccount">The service account credentials in JSON format.</param>
    /// <param name="userInput">The user's input or the event name for automated text.</param>
    /// <param name="sessionId">The session ID for the conversation.</param>
    /// <param name="customPayload">The custom payload to be sent with the request.</param>
    /// <param name="userInputType">The type of user input (automated text or normal text).</param>
    /// <returns>A dynamic object representing the response from Dialogflow.</returns>
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
            Struct payloadStruct = !string.IsNullOrEmpty(customPayload)
                ? Google.Protobuf.WellKnownTypes.Struct.Parser.ParseJson(customPayload)
                : null;

            // Define the request object based on the type of user input
            object dialogflowRequest = userInputType == UserInputType.AUTOMATED_TEXT
                ? new
                {
                    query_input = new
                    {
                        @event = new
                        {
                            name = userInput, // The name of the event to trigger the intent
                            language_code = "en-US"
                        }
                    }
                }
                : new
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
            Console.WriteLine($"Error sending request to Dialogflow: {ex.Message}");
        }

        // Return the deserialized response
        return JsonConvert.DeserializeObject<dynamic>(jsonResponse);
    }

}

