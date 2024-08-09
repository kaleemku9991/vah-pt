using Newtonsoft.Json;
using proxyTask.Model;
using static proxyTask.Model.ExternalIntegrationBotExchangeRequest;
using Google.Protobuf.WellKnownTypes;
using proxyTask.Controllers;

namespace proxyTask.Manager
{
    public class DialogFlowRequestHandle
    {
        private readonly DialogflowService _dialogflowService;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CustomBotController> _logger;
        private readonly VahResponseBuilder _appResponseBuilder;
        /// <summary>
        /// Initializes a new instance of the DialogFlowRequestHandle class.
        /// Sets up the Dialogflow service and stores the provided HttpClient.
        /// </summary>
        /// <param name="httpClient">The HttpClient used for sending requests to Dialogflow.</param>
        public DialogFlowRequestHandle(HttpClient httpClient, ILogger<CustomBotController> logger)
        {
            _dialogflowService = new DialogflowService(httpClient, logger);
            _appResponseBuilder = new VahResponseBuilder();
            _httpClient = httpClient;
            _logger = logger;
        }



        /// <summary>
        /// Generates a unique session ID by combining a random number and a timestamp.
        /// </summary>
        /// <returns>A unique session ID string.</returns>
        private static string GenerateSessionId()
        {
            Random random = new Random();
            int randomNumber = random.Next(1000, 10000);
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            return $"SESSION-{randomNumber}-{timestamp}";
        }

        /// <summary>
        /// Handles the request by forwarding it to the Dialogflow service and returning the response.
        /// Initializes the session state if not provided, and processes the bot configuration.
        /// </summary>
        /// <param name="request">The incoming request containing bot configuration and session state.</param>
        /// <returns>A dynamic object representing the response from Dialogflow.</returns>
        public async Task<CustomExchangeResponse_V1> HandleDialogFlowRequest(ExternalIntegrationBotExchangeRequest request)
        {
            try
            {
                CustomExchangeResponse_V1 responseAction = new();
                var botConfig = JsonConvert.DeserializeObject<BotConfig>(request.BotConfig);
                if (botConfig == null)
                {
                    _appResponseBuilder.SetErrorDetails(responseAction, "Invalid bot configuration");
                }

                if (request.BotSessionState == null)
                {
                    request.BotSessionState = new BotSessionState
                    {
                        SessionId = GenerateSessionId()
                    };
                }
                else
                {
                    responseAction.BotSessionState = new BotSessionState
                    {
                        SessionId = request.BotSessionState?.SessionId ?? ""
                    };

                }
                var userJson = botConfig.GetEndpointParameter("userJson");
                var jsonServiceAccount = JsonConvert.DeserializeObject<object>($"{{{userJson}}}");
                var userProjectId = botConfig.GetEndpointParameter("userProjectId");
                string customPayloadSerialize = request.CustomPayload != null
                    ? JsonConvert.SerializeObject(request.CustomPayload)
                    : null;
                var uri = $"https://dialogflow.googleapis.com/v2/projects/{userProjectId}/agent/sessions/{request.BotSessionState.SessionId}:detectIntent";
                Struct payloadStruct = !string.IsNullOrEmpty(customPayloadSerialize)
                    ? Google.Protobuf.WellKnownTypes.Struct.Parser.ParseJson(customPayloadSerialize)
                    : null;
                var queryInput = new
                {
                    text = request.userInputType == UserInputType.AUTOMATED_TEXT ? null : new
                    {
                        text = request.UserInput,
                        language_code = "en-US"
                    },
                    @event = request.userInputType == UserInputType.AUTOMATED_TEXT ? new
                    {
                        name = request.UserInput,
                        language_code = "en-US"
                    } : null
                };

                var queryParams = request.userInputType == UserInputType.AUTOMATED_TEXT ? null : new
                {
                    payload = payloadStruct
                };

                var dialogflowRequest = new
                {
                    query_input = queryInput,
                    query_params = queryParams
                };

                var jsonResponse = await _dialogflowService.sendDialogFlowRequest(jsonServiceAccount, dialogflowRequest, uri);
                if (jsonResponse == null)
                {
                    // Assuming responseAction is the object you're setting the error details for
                    _appResponseBuilder.SetErrorDetails(responseAction, "Null response received from Dialogflow.");

                }
                _logger.LogInformation($"Dialog Flow Response is: ${jsonResponse}");
                
                var appResponse = _appResponseBuilder.CreateResponseForVah(request, jsonResponse);
                return appResponse;
            }
            catch (Exception ex)
            { 
                throw new ApplicationException($"An error occurred while processing the request: {ex.Message}", ex);
            }
        }


    }
}
