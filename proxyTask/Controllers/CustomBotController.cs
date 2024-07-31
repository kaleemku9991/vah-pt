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

        /// <summary>
        /// Initializes the CustomBotController with dependencies for handling Dialogflow requests and building app responses.
        /// </summary>
        /// <param name="httpClientFactory">The factory to create HTTP clients.</param>
        public CustomBotController(IHttpClientFactory httpClientFactory)
        {
            try
            {
                var httpClient = httpClientFactory.CreateClient();
                _dialogflowRequestHandle = new DialogFlowRequestHandle(httpClient);
                _appResponseBuilder = new VahResponseBuilder();
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if logging is set up.
                // Handle the exception or rethrow it if necessary.
                throw new InvalidOperationException("Failed to initialize CustomBotController.", ex);
            }
        }


        /// <summary>
        /// Forwards a request to Dialogflow and returns the application's response.
        /// Handles the received Dialogflow response or returns an error if the response is null.
        /// </summary>
        /// <param name="request">The incoming request payload.</param>
        /// <returns>An HTTP response with the response or an error message.</returns>
        [HttpPost("textBotExchangeCustom")]
        public async Task<IActionResult> Forward([FromBody] ExternalIntegrationBotExchangeRequest request)
        {
            try
            {
                var requestJson = JsonConvert.SerializeObject(request, Formatting.Indented);

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
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }


        /// <summary>
        /// Processes a webhook request, retrieves an echo value from the request, and responds with a fulfillment message.
        /// Handles JSON parsing errors and other exceptions.
        /// </summary>
        /// <param name="request">The incoming webhook request payload.</param>
        /// <returns>An HTTP response with the fulfillment message or an error message.</returns>
        [HttpPost("webhook")]
        public IActionResult GetCustomInput([FromBody] object request)
        {
            try
            {
                // Parse the incoming request to a dynamic object
                dynamic requestData = JObject.Parse(request.ToString());

                // Check if outputContexts exist
                if (requestData?.queryResult?.outputContexts == null || requestData.queryResult.outputContexts.Count == 0)
                {
                    return BadRequest("OutputContexts is null or empty.");
                }

                // Retrieve the echo value from the request
                string echoValue = requestData["originalDetectIntentRequest"]?["payload"]?["Fields"]?["echoValue"]?["StringValue"]?.ToString();

                // Construct the fulfillment response
                var fulfillmentResponse = new
                {
                    fulfillmentMessages = new[]
                    {
                new
                {
                    text = new
                    {
                        text = new[] { echoValue }
                    }
                }
            }
                };

                // Serialize the response to JSON and return it
                return Ok(JsonConvert.SerializeObject(fulfillmentResponse));
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








