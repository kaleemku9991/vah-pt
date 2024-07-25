using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using proxyTask.Manager;
using proxyTask.Model;

namespace proxyTask.Controllers
{
    public class CustomBotController : Controller
    {
        private readonly DialogFlowRequestHandle _dialogflowRequestHandle;
        private readonly VahResponseBuilder _appResponseBuilder;
        private readonly ILogger<CustomBotController> _logger;

        public CustomBotController(IHttpClientFactory httpClientFactory, ILogger<CustomBotController> logger)
        {
            var httpClient = httpClientFactory.CreateClient();
            _dialogflowRequestHandle = new DialogFlowRequestHandle(httpClient, logger);
            _appResponseBuilder = new VahResponseBuilder();
            _logger = logger;
        }

        [HttpPost("textBotExchangeCustom")]
        public async Task<IActionResult> Forward([FromBody] ExternalIntegrationBotExchangeRequest request)
        {
            var dialogflowResponse = await _dialogflowRequestHandle.HandleRequest(request);
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

