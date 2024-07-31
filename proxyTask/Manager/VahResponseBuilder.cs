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
    public CustomExchangeResponse_V1 createResponseForVah(ExternalIntegrationBotExchangeRequest request, dynamic dialogflowResponse)
    {
        var promptDefinitions = new List<PromptDefinition>();
        JObject mediaSpecificObject = null;
        var branchName = CustomExchangeResponse_V1.BotExchangeBranch.PromptAndCollectNextResponse;
        Dictionary<string, object> scriptPayloads = null;
        var intentName = dialogflowResponse.queryResult?.intent?.displayName ?? "";
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
                            branchName = CustomExchangeResponse_V1.BotExchangeBranch.UserInputTimeout;
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
                        branchName = message.payload.content.vahExchangeResultBranch;
                        intentName = message.payload.content.intent;
                    }
                    else
                    {
                        scriptPayloads = message.payload.ToObject<Dictionary<string, object>>();
                    }
                }
            }
        }

        if (promptDefinitions.Count == 0)
        {
            var defaultTranscript = dialogflowResponse.queryResult?.fulfillmentText?.ToString() ?? "";
            if (defaultTranscript == "SILENCE")
            {
                branchName = CustomExchangeResponse_V1.BotExchangeBranch.UserInputTimeout;
            }

            promptDefinitions.Add(new PromptDefinition
            {
                Transcript = defaultTranscript,
                MediaSpecificObject = mediaSpecificObject,
            });
        }

        if (intentName == "StandardBotEscalation" || intentName == "StandardBotEndConversation")
        {
            branchName = CustomExchangeResponse_V1.BotExchangeBranch.ReturnControlToScript;
        }

        var customPayload = new Dictionary<string, object>{};

        if (scriptPayloads != null)
        {
            customPayload["scriptPayloads"] = scriptPayloads;
        }

        return new CustomExchangeResponse_V1
        {
            BranchName = branchName,
            NextPromptSequence = new PromptSequence
            {
                Prompts = promptDefinitions
            },
            IntentInfo = new IntentInfo
            {
                Intent = intentName,
            },
            CustomPayload = customPayload,
            ErrorDetails = new BotErrorDetails
            {
                //errorBehavior = BotErrorDetails.BotLoopErrorBehavior.ReturnControlToScriptThroughErrorBranch,
                //errorPromptSequence = null,
                //systemErrorMessage = "External webhook response does not conform to spec. Exception caught in REST Post: The operation was canceled. Invalid HTTP response received (400)."
            },
            BotSessionState = new BotSessionState
            {
                SessionId = request.BotSessionState?.SessionId ?? ""
            }
        };
    }

}