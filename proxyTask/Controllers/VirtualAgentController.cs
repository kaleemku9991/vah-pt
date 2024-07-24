using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using proxyTask.Model;

namespace proxyTask.Controllers
{
    public class VirtualAgentController : Controller
    {
        private readonly DialogflowService _dialogflowService;
        private readonly AppResponseBuilder _appResponseBuilder;
        private readonly ILogger<VirtualAgentController> _logger;

        public VirtualAgentController(IHttpClientFactory httpClientFactory, ILogger<VirtualAgentController> logger)
        {
            var httpClient = httpClientFactory.CreateClient();
            _dialogflowService = new DialogflowService(httpClient, logger);
            _appResponseBuilder = new AppResponseBuilder();
            _logger = logger;
        }

        [HttpPost("textBotExchangeCustom")]
        public async Task<IActionResult> Forward([FromBody] ExternalIntegrationBotExchangeRequest request)
        {

         
            var requestJson = JsonConvert.SerializeObject(request, Formatting.Indented);
            _logger.LogInformation("Received request: {RequestJson}", requestJson);


            var dialogflowResponse = await _dialogflowService.SendRequest(request.userInput, request.botConfig);
            if (dialogflowResponse != null)
            {
                var appResponse = _appResponseBuilder.CreateAppResponse(request, dialogflowResponse);
                return Ok(appResponse);
            }
            else
            {
                return StatusCode(500, "Dialogflow response was null.");
            }
        }
    }
}

