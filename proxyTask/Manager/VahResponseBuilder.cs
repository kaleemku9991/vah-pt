using Newtonsoft.Json.Linq;
using proxyTask.Model;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Nodes;

public class VahResponseBuilder
{
    public CustomExchangeResponse_V1 CreateAppResponse(ExternalIntegrationBotExchangeRequest request, dynamic dialogflowResponse)
    {
        var promptDefinitions = new List<PromptDefinition>();
        JObject mediaSpecificObject = null; // Use JObject to handle dynamic JSON structure
        var branchName = CustomExchangeResponse_V1.BotExchangeBranch.PromptAndCollectNextResponse; // Default branch name
        Dictionary<string, object> scriptPayloads = null;

        // Add each fulfillmentMessage to the promptDefinitions list
        if (dialogflowResponse?.queryResult?.fulfillmentMessages != null)
        {
            foreach (var message in dialogflowResponse.queryResult.fulfillmentMessages)
            {
                // Check if the message has text
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

                // Check if the message has a payload
                if (message.payload != null)
                {
                    if (message.payload.dfoMessage != null)
                    {
                        // Parse dfoMessage payload into JObject to better handle nested structures
                        mediaSpecificObject = JObject.FromObject(message.payload.dfoMessage);
                    }
                    else
                    {
                        // Handle scriptPayloads when dfoMessage is not present
                        scriptPayloads = message.payload.ToObject<Dictionary<string, object>>();
                    }
                }
            }
        }

        // Ensure at least one prompt is added (for backward compatibility with previous implementation)
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

        return new CustomExchangeResponse_V1
        {
            branchName = branchName,
            nextPromptSequence = new PromptSequence
            {
                prompts = promptDefinitions
            },
            intentInfo = new IntentInfo
            {
                intent = dialogflowResponse.queryResult?.intent?.displayName ?? "",
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
