using Newtonsoft.Json.Linq;
using proxyTask.Model;

public class VahResponseBuilder
{
    /// <summary>
    /// Creates a response object for the custom exchange based on the Dialogflow response and request data.
    /// Constructs prompt definitions, determines the branch name, and populates the custom payload.
    /// </summary>
    /// <param name="request">The request containing details for creating the response.</param>
    /// <param name="dialogflowResponse">The Dialogflow response used to populate the response object.</param>
    /// <returns>A CustomExchangeResponse_V1 object with the generated response details.</returns>
    public CustomExchangeResponse_V1 CreateAppResponse(ExternalIntegrationBotExchangeRequest request, dynamic dialogflowResponse)
    {
        var promptDefinitions = new List<PromptDefinition>();
        JObject mediaSpecificObject = null; // Handle dynamic JSON structure
        var branchName = CustomExchangeResponse_V1.BotExchangeBranch.PromptAndCollectNextResponse; // Default branch name
        Dictionary<string, object> scriptPayloads = null;
        var intentName = dialogflowResponse.queryResult?.intent?.displayName ?? "";

        // Process fulfillment messages
        if (dialogflowResponse?.queryResult?.fulfillmentMessages != null)
        {
            foreach (var message in dialogflowResponse.queryResult.fulfillmentMessages)
            {
                // Handle text messages
                if (message.text?.text != null)
                {
                    foreach (var text in message.text.text)
                    {
                        if (text == "SILENCE")
                        {
                            branchName = CustomExchangeResponse_V1.BotExchangeBranch.UserInputTimeout;
                        }

                        promptDefinitions.Add(new PromptDefinition
                        {
                            transcript = text,
                            base64EncodedG711ulawWithWavHeader = "",
                            audioFilePath = null,
                            textToSpeech = null,
                            mediaSpecificObject = mediaSpecificObject,
                        });
                    }
                }

                // Handle payload messages
                if (message.payload != null)
                {
                    if (message.payload.dfoMessage != null)
                    {
                        // Parse dfoMessage payload into JObject
                        mediaSpecificObject = JObject.FromObject(message.payload.dfoMessage);
                    }
                    if (message.payload.contentType == "ExchangeResultOverride")
                    {
                        // Handle ExchangeResultOverride type payload
                        branchName = message.payload.content.vahExchangeResultBranch;
                        intentName = message.payload.content.intent;
                    }
                    else
                    {
                        // Handle script payloads
                        scriptPayloads = message.payload.ToObject<Dictionary<string, object>>();
                    }
                }
            }
        }

        // Ensure at least one prompt is added
        if (promptDefinitions.Count == 0)
        {
            var defaultTranscript = dialogflowResponse.queryResult?.fulfillmentText?.ToString() ?? "";
            if (defaultTranscript == "SILENCE")
            {
                branchName = CustomExchangeResponse_V1.BotExchangeBranch.UserInputTimeout;
            }

            promptDefinitions.Add(new PromptDefinition
            {
                transcript = defaultTranscript,
                base64EncodedG711ulawWithWavHeader = "",
                audioFilePath = null,
                textToSpeech = null,
                mediaSpecificObject = mediaSpecificObject,
            });
        }

        // Handle specific intent case
        if (intentName == "StandardBotEscalation")
        {
            branchName = CustomExchangeResponse_V1.BotExchangeBranch.ReturnControlToScript;
        }

        if (intentName == "StandardBotEndConversation")
        {
            branchName = CustomExchangeResponse_V1.BotExchangeBranch.ReturnControlToScript;
        }


        // Construct the custom payload
        var customPayload = new Dictionary<string, object>
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
    };

        if (scriptPayloads != null)
        {
            customPayload["scriptPayloads"] = scriptPayloads;
        }

        // Return the constructed CustomExchangeResponse_V1 object
        return new CustomExchangeResponse_V1
        {
            branchName = branchName,
            nextPromptSequence = new PromptSequence
            {
                prompts = promptDefinitions
            },
            intentInfo = new IntentInfo
            {
                intent = intentName,
                context = dialogflowResponse.queryResult?.outputContexts?.ToString() ?? "",
                intentConfidence = dialogflowResponse.queryResult?.intentDetectionConfidence ?? 0,
                lastUserUtterance = request.userInput
            },
            customPayload = customPayload,
            errorDetails = new BotErrorDetails
            {
                errorBehavior = BotErrorDetails.BotLoopErrorBehavior.ReturnControlToScriptThroughErrorBranch,
                errorPromptSequence = null,
                systemErrorMessage = "External webhook response does not conform to spec. Exception caught in REST Post: The operation was canceled. Invalid HTTP response received (400)."
            },
            botSessionState = new BotSessionState
            {
                sessionId = request.botSessionState?.sessionId ?? ""
            }
        };
    }

}