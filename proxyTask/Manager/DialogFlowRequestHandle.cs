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
        public async Task<dynamic> handleDialogFlowRequest(ExternalIntegrationBotExchangeRequest request)
        {
            var botConfig = JsonConvert.DeserializeObject<BotConfig>(request.BotConfig);
            if (botConfig == null)
            {
                throw new ArgumentException("Invalid bot configuration.");
            }
            if (request.BotSessionState == null)
            {
                request.BotSessionState = new BotSessionState
                {
                    SessionId = GenerateSessionId()
                };
            }
            var userJson = botConfig.GetEndpointParameter("userJson");
            var jsonServiceAccount = JsonConvert.DeserializeObject<object>("{" + userJson + "}");
            var userProjectId = botConfig.GetEndpointParameter("userProjectId");
            string customPayloadSerialize = request.CustomPayload != null
                ? JsonConvert.SerializeObject(request.CustomPayload)
                : null;
            var jsonResponse = await _dialogflowService.sendDialogFlowRequest(
                userProjectId,
                jsonServiceAccount,
                request.UserInput,
                request.BotSessionState.SessionId,
                customPayloadSerialize,
                request.userInputType
            );

            return jsonResponse;
        }

    }
}
