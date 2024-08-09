using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using proxyTask.Controllers;
using System.Net.Http.Headers;



public class DialogflowService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CustomBotController> _logger;

    /// <summary>
    /// Initializes a new instance of the DialogflowService class.
    /// </summary>
    /// <param name="httpClient">The HttpClient used for sending HTTP requests.</param>
    public DialogflowService(HttpClient httpClient, ILogger<CustomBotController> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

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
    public async Task<dynamic> SendDialogFlowRequest( dynamic jsonServiceAccount, object dialogflowRequest, string uri)
    {
        string jsonResponse = null;
        try
        {
            var credential = GoogleCredential.FromJson(jsonServiceAccount.ToString())
                .CreateScoped("https://www.googleapis.com/auth/cloud-platform");
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
                _logger.LogInformation("Error response from Dialogflow: " + errorResponse);
                return null;
            }
            jsonResponse = await httpResponse.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
                _logger.LogInformation($"Error sending request to Dialogflow: ${ex.Message}");
        }
        return JsonConvert.DeserializeObject<dynamic>(jsonResponse);
    }

}

