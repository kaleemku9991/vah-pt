using Newtonsoft.Json;
using proxyTask.Model;
using static proxyTask.Model.ExternalIntegrationBotExchangeRequest;
using Google.Protobuf.WellKnownTypes;
using proxyTask.Controllers;
using Newtonsoft.Json.Linq;

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

        private void SetErrorDetails(CustomExchangeResponse_V1 responseAction, string errorMessage)
        {
            responseAction.ErrorDetails = new BotErrorDetails
            {
                ErrorBehavior = BotErrorDetails.BotLoopErrorBehavior.ReturnControlToScriptThroughErrorBranch,
                ErrorPromptSequence = null,
                SystemErrorMessage = errorMessage,
            };
        }

        private void SetIntentInfo(CustomExchangeResponse_V1 responseAction, dynamic dialogflowResponse)
        {
            responseAction.IntentInfo = new IntentInfo
            {
                Intent = dialogflowResponse.queryResult?.intent?.displayName ?? ""
            };
        }

        private void SetPromptSequence(CustomExchangeResponse_V1 responseAction, dynamic dialogflowResponse)
        {
            var promptDefinitions = new List<PromptDefinition>();
            JObject mediaSpecificObject = null;

            if (dialogflowResponse?.queryResult?.fulfillmentMessages != null)
            {
                foreach (var message in dialogflowResponse.queryResult.fulfillmentMessages)
                {
                    if (message.text?.text != null)
                    {
                        foreach (var text in message.text.text)
                        {
                            if (text == "SILENCE")
                            {
                                responseAction.BranchName = CustomExchangeResponse_V1.BotExchangeBranch.UserInputTimeout;
                            }
                            promptDefinitions.Add(new PromptDefinition
                            {
                                Transcript = text,
                                MediaSpecificObject = mediaSpecificObject,
                            });
                        }
                    }

                    if (message.payload != null)
                    {
                        if (message.payload.dfoMessage != null)
                        {
                            mediaSpecificObject = JObject.FromObject(message.payload.dfoMessage);
                        }
                        if (message.payload.contentType == "ExchangeResultOverride")
                        {
                            responseAction.BranchName = message.payload.content.vahExchangeResultBranch;
                            responseAction.IntentInfo = new IntentInfo
                            {
                                Intent = message.payload.content.intent
                            };
                        }
                    }
                }
            }

            if (promptDefinitions.Count == 0)
            {
                var defaultTranscript = dialogflowResponse.queryResult?.fulfillmentText?.ToString() ?? "";
                if (defaultTranscript == "SILENCE")
                {
                    responseAction.BranchName = CustomExchangeResponse_V1.BotExchangeBranch.UserInputTimeout;
                }
                promptDefinitions.Add(new PromptDefinition
                {
                    Transcript = defaultTranscript,
                    MediaSpecificObject = mediaSpecificObject,
                });
            }

            responseAction.NextPromptSequence = new PromptSequence
            {
                Prompts = promptDefinitions
            };
        }

        private void SetBranchName(CustomExchangeResponse_V1 responseAction)
        {
            responseAction.BranchName = CustomExchangeResponse_V1.BotExchangeBranch.PromptAndCollectNextResponse;
            if (responseAction.IntentInfo.Intent == "StandardBotEscalation" ||
                responseAction.IntentInfo.Intent == "StandardBotEndConversation")
            {
                responseAction.BranchName = CustomExchangeResponse_V1.BotExchangeBranch.ReturnControlToScript;
            }
        }

        private void SetCustomPayload(CustomExchangeResponse_V1 responseAction, dynamic dialogflowResponse)
        {
            Dictionary<string, object> scriptPayloads = null;

            foreach (var message in dialogflowResponse.queryResult.fulfillmentMessages)
            {
                if (message.payload != null && message.payload.dfoMessage == null && message.payload.contentType != "ExchangeResultOverride")
                {
                    scriptPayloads = message.payload.ToObject<Dictionary<string, object>>();
                }
            }

            if (scriptPayloads != null)
            {
                responseAction.CustomPayload["scriptPayloads"] = scriptPayloads;
            }
        }

        public async Task<CustomExchangeResponse_V1> HandleDialogFlowRequest(ExternalIntegrationBotExchangeRequest request)
        {
            try
            {
                CustomExchangeResponse_V1 responseAction = new();
                var botConfig = JsonConvert.DeserializeObject<BotConfig>(request.BotConfig);
                if (botConfig == null)
                {
                    SetErrorDetails(responseAction, "Invalid bot configuration");
                    return responseAction;
                }

                if (request.BotSessionState == null)
                {
                    responseAction.BotSessionState = new BotSessionState
                    {
                        SessionId = GenerateSessionId()
                    };
                }
                else{
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

                var uri = $"https://dialogflow.googleapis.com/v2/projects/{userProjectId}/agent/sessions/{responseAction.BotSessionState.SessionId}:detectIntent";
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

                var jsonResponse = await _dialogflowService.SendDialogFlowRequest(jsonServiceAccount, dialogflowRequest, uri);
                if (jsonResponse == null)
                {
                    SetErrorDetails(responseAction, "Null response received from Dialogflow.");
                    return responseAction;
                }

                _logger.LogInformation($"Dialog Flow Response is: ${jsonResponse}");

                SetIntentInfo(responseAction, jsonResponse);
                SetPromptSequence(responseAction, jsonResponse);
                SetBranchName(responseAction);
                SetCustomPayload(responseAction, jsonResponse);

                return responseAction;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while processing the request: {ex.Message}", ex);
            }
        }
    }

}
