using Newtonsoft.Json;
using proxyTask.Model;

namespace proxyTask.Manager
{
    public class DialogFlowRequestHandle
    {
        private readonly DialogflowService _dialogflowService;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the DialogFlowRequestHandle class.
        /// Sets up the Dialogflow service and stores the provided HttpClient.
        /// </summary>
        /// <param name="httpClient">The HttpClient used for sending requests to Dialogflow.</param>
        public DialogFlowRequestHandle(HttpClient httpClient)
        {
            _dialogflowService = new DialogflowService(httpClient);
            _httpClient = httpClient;
        }



        /// <summary>
        /// Generates a unique session ID by combining a random number and a timestamp.
        /// </summary>
        /// <returns>A unique session ID string.</returns>
        private static string GenerateSessionId()
        {
            // Generate a random number between 1000 and 9999
            Random random = new Random();
            int randomNumber = random.Next(1000, 10000);

            // Get the current timestamp in the format yyyyMMddHHmmss
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            // Combine the random number and timestamp to form a unique session ID
            return $"SESSION-{randomNumber}-{timestamp}";
        }

        /// <summary>
        /// Handles the request by forwarding it to the Dialogflow service and returning the response.
        /// Initializes the session state if not provided, and processes the bot configuration.
        /// </summary>
        /// <param name="request">The incoming request containing bot configuration and session state.</param>
        /// <returns>A dynamic object representing the response from Dialogflow.</returns>
        public async Task<dynamic> HandleRequest(ExternalIntegrationBotExchangeRequest request)
        {
            // Deserialize BotConfig from JSON
            var botConfig = JsonConvert.DeserializeObject<BotConfig>(request.botConfig);
            if (botConfig == null)
            {
                throw new ArgumentException("Invalid bot configuration.");
            }

            // Initialize botSessionState if not provided
            if (request.botSessionState == null)
            {
                request.botSessionState = new BotSessionState
                {
                    sessionId = GenerateSessionId()
                };
            }

            // Retrieve userJson and userProjectId from BotConfig
            var userJson = botConfig.GetEndpointParameter("userJson");
            var jsonServiceAccount = JsonConvert.DeserializeObject<object>("{" + userJson + "}");
            var userProjectId = botConfig.GetEndpointParameter("userProjectId");

            // Serialize customPayload if provided
            string customPayloadSerialize = request.customPayload != null
                ? JsonConvert.SerializeObject(request.customPayload)
                : null;

            // Send the request to Dialogflow service
            var jsonResponse = await _dialogflowService.SendRequest(
                userProjectId,
                jsonServiceAccount,
                request.userInput,
                request.botSessionState.sessionId,
                customPayloadSerialize,
                request.userInputType
            );

            return jsonResponse;
        }

    }
}
