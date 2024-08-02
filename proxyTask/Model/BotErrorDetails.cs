using Newtonsoft.Json;

namespace proxyTask.Model
{

    public class BotErrorDetails
    {
        public enum BotLoopErrorBehavior
        {
            ReturnControlToScriptThroughErrorBranch,
            EndContact
        }

        /// <summary>
        /// Behavior to perform if BotProvider is unreachable or returns errors from any
        /// BotExchange. Possible values: 
        /// • ReturnControlToScriptThroughErrorBranch - Return control to script through Error Branch
        /// • EndContact - End the bot session (hang up on phone call, end chat, etc).
        /// </summary>
        [JsonProperty("errorBehavior")]
        public BotLoopErrorBehavior ErrorBehavior { get; set; }

        /// <summary>
        /// Optional sequence of prompts to play in case of error during the BotLoop. If
        /// populated, the sequence will be rendered to user PRIOR to executing the specified
        /// 'errorBehavior'.
        /// </summary>
        [JsonProperty("errorPromptSequence")]
        public PromptSequence ErrorPromptSequence { get; set; }

        /// <summary>
        /// Debug level information that will be logged by the system to assist in troubleshooting.
        /// </summary>
        [JsonProperty("systemErrorMessage")]
        public string SystemErrorMessage { get; set; }
    }

}
