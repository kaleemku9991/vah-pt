using Newtonsoft.Json.Linq;
using proxyTask.Model;

public class VahResponseBuilder
{

    public void SetErrorDetails(CustomExchangeResponse_V1 responseAction, string errorMessage)
    {
        responseAction.ErrorDetails = new BotErrorDetails
        {
            ErrorBehavior = BotErrorDetails.BotLoopErrorBehavior.ReturnControlToScriptThroughErrorBranch,
            ErrorPromptSequence = null,
            SystemErrorMessage = errorMessage,
        };
    }
    /// <summary>
    /// Creates a response object for the custom exchange based on the Dialogflow response and request data.
    /// Constructs prompt definitions, determines the branch name, and populates the custom payload.
    /// </summary>
    /// <param name="request">The request containing details for creating the response.</param>
    /// <param name="dialogflowResponse">The Dialogflow response used to populate the response object.</param>
    /// <returns>A CustomExchangeResponse_V1 object with the generated response details.</returns>
    public CustomExchangeResponse_V1 CreateResponseForVah(ExternalIntegrationBotExchangeRequest request, dynamic dialogflowResponse)
    {

        CustomExchangeResponse_V1 responseAction = new();
        var promptDefinitions = new List<PromptDefinition>();
        JObject mediaSpecificObject = null;
        responseAction.BranchName = CustomExchangeResponse_V1.BotExchangeBranch.PromptAndCollectNextResponse;
        Dictionary<string, object> scriptPayloads = null;
        responseAction.BotSessionState = new BotSessionState
        {
            SessionId = request.BotSessionState?.SessionId ?? ""
        };

        responseAction.IntentInfo = new IntentInfo
        {
            Intent = dialogflowResponse.queryResult?.intent?.displayName ?? ""
        };
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

        if (responseAction.IntentInfo.Intent == "StandardBotEscalation" || responseAction.IntentInfo.Intent == "StandardBotEndConversation")
        {
            responseAction.BranchName = CustomExchangeResponse_V1.BotExchangeBranch.ReturnControlToScript;
        }

        if (scriptPayloads != null)
        {
            responseAction.CustomPayload["scriptPayloads"] = scriptPayloads;
        }

        //if(responseAction.ErrorDetails.SystemErrorMessage== null)
        //{
        //    responseAction.ErrorDetails = new BotErrorDetails
        //    {
        //        ErrorBehavior = BotErrorDetails.BotLoopErrorBehavior.ReturnControlToScriptThroughErrorBranch,
        //        ErrorPromptSequence = null,
        //        SystemErrorMessage = null,
        //    };
        //}

        return responseAction;
    }

}