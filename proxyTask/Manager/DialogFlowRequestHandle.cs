using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using proxyTask.Controllers;
using proxyTask.Model;
using System.Net.Http.Headers;

namespace proxyTask.Manager
{
    public class DialogFlowRequestHandle
    {
        private readonly DialogflowService _dialogflowService;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CustomBotController> _logger;

        public DialogFlowRequestHandle(HttpClient httpClient, ILogger<CustomBotController> logger)
        {
            _dialogflowService = new DialogflowService(httpClient, logger);
            _httpClient = httpClient;
            _logger = logger;

        }


        private static string GenerateSessionId()
        {
            Random random = new Random();
            int randomNumber = random.Next(1000, 10000);
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            return $"SESSION-{randomNumber}-{timestamp}";
        }

        public async Task<dynamic> HandleRequest(ExternalIntegrationBotExchangeRequest request)
        {

            // Deserialize BotConfig from JSON using Newtonsoft.Json
            var botConfig = JsonConvert.DeserializeObject<BotConfig>(request.botConfig);
            if (botConfig == null)
            {
                throw new ArgumentException("Invalid bot configuration.");
            }

            if (request.botSessionState == null)
            {
                request.botSessionState = new BotSessionState();
                request.botSessionState.sessionId = GenerateSessionId();
            }
            // Retrieve userJson and userProjectId from BotConfig
            var userJson = botConfig.GetEndpointParameter("userJson");

            var jsonServiceAccount = JsonConvert.DeserializeObject<object>("{" + userJson + "}");

            var userProjectId = botConfig.GetEndpointParameter("userProjectId");

            var customPalyloadSerialize = JsonConvert.SerializeObject(request.customPayload);
            var jsonResponse = await _dialogflowService.SendRequest(userProjectId, jsonServiceAccount, request.userInput, request.botSessionState.sessionId, customPalyloadSerialize);
            return jsonResponse;

        }


    }
}
