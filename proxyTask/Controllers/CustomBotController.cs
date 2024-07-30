using Google.Cloud.Dialogflow.V2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            var requestJson = JsonConvert.SerializeObject(request, Formatting.Indented);
            _logger.LogInformation("Received request: {RequestJson}", requestJson);

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
        //[HttpPost("webhook")]
        //public async Task<IActionResult> Post([FromBody] dynamic request)
        //{
        //    var requestBody = JsonConvert.SerializeObject(request, Formatting.Indented);
        //    //_logger.LogInformation("Request: {RequestBody}", request);

        //    // Retrieve the echo value from the request
        //    var echoValue = "test";


        //    // Create a response that echoes back the received value
        //    var response = new JObject
        //    {
        //        ["fulfillmentText"] = $"{echoValue}"
        //    };

        //    return Ok(response);
        //}

        [HttpPost("webhook")]
        public IActionResult GetCustomInput([FromBody] object request)
        {
            try
            {
                dynamic requestData = JObject.Parse(request.ToString());
                if (requestData.queryResult.outputContexts == null || requestData.queryResult.outputContexts.Count == 0)
                {
                    return BadRequest("OutputContexts is null or empty");
                }

                //string echoValue = requestData.queryResult.outputContexts[0].parameters.echoValue;

                //string echoValue = requestData.originalDetectIntentRequest.payload.Fields.echoValue.StringValue;
                string echoValue = requestData["originalDetectIntentRequest"]?["payload"]?["Fields"]?["echoValue"]?["StringValue"]?.ToString();

                var fulfillmentResponse = new
                {
                    fulfillmentMessages = new[]
                    {
                        new
                        {
                            text = new
                            {
                                text = new[]
                                {
                                    echoValue
                                }
                            }
                        }
                    }
                };

                string jsonResponse = JsonConvert.SerializeObject(fulfillmentResponse);

                return Ok(jsonResponse);
            }
            catch (JsonException jsonEx)
            {
                return BadRequest($"JSON error: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}








