using proxyTask.Model;

    public class AppResponseBuilder
    {
        public CustomExchangeResponse_V1 CreateAppResponse(ExternalIntegrationBotExchangeRequest request, dynamic dialogflowResponse)
        {
            return new CustomExchangeResponse_V1
            {
                branchName = CustomExchangeResponse_V1.BotExchangeBranch.PromptAndCollectNextResponse,
                nextPromptSequence = new PromptSequence
                {
                    prompts = new List<PromptDefinition>
                {
                    new PromptDefinition
                    {
                        transcript = dialogflowResponse.queryResult?.fulfillmentText?.ToString() ?? "",
                        base64EncodedG711ulawWithWavHeader = "",
                        audioFilePath = null,
                        textToSpeech = null,
                        mediaSpecificObject = null
                    }
                }
                },
                intentInfo = new IntentInfo
                {
                    intent = dialogflowResponse.queryResult?.intent?.displayName ?? "",
                    context = dialogflowResponse.queryResult?.outputContexts?.ToString() ?? "",
                    intentConfidence = dialogflowResponse.queryResult?.intentDetectionConfidence ?? 0,
                    lastUserUtterance = request.userInput
                },
                customPayload = new Dictionary<string, object>
            {
                { "callbackTime", null },
                { "httpStatusCode", null },
                { "diagnostics", new Dictionary<string, object>
                    {
                        { "botExchangeDurationMS", "" },
                        { "vahExchangeResultBranch", "" },
                        { "vahInstanceId", "" },
                        { "detectedNoiseBursts", "" },
                        { "integrationVersion", "" },
                        { "notes", "" },
                        { "scriptVars", new Dictionary<string, string> { { "global:__messageSender", "customendpoint" } } }
                    }
                }
            },
                errorDetails = new BotErrorDetails
                {
                    errorBehavior = BotErrorDetails.BotLoopErrorBehavior.ReturnControlToScriptThroughErrorBranch,
                    errorPromptSequence = null,
                    systemErrorMessage = "External webhook response does not conform to spec. Exception caught in REST Post: The operation was canceled. Invalid HTTP response received (400)."
                }
            };
        }
    }
