using Newtonsoft.Json;
using System.Collections.Generic;


namespace proxyTask.Model
{
    public class CustomExchangeResponse_V1
    {
        public enum BotExchangeBranch
        {
            DoNotBegin,
            PromptAndCollectNextResponse,
            ReturnControlToScript,
            EndContact,
            AudioInputUntranscribeable,
            Error,
            DtmfBreakout,
            UserInputTimeout,
            UserInputNotUnderstood
        }

        [JsonProperty("branchName")]
        public BotExchangeBranch BranchName { get; set; }

        /// <summary>
        /// Sequence of prompts to be rendered to customer on next round of the BotLoop.
        /// </summary>
        [JsonProperty("nextPromptSequence")]
        public PromptSequence NextPromptSequence { get; set; }

        /// <summary>
        /// Information from the bot on current user intent.
        /// </summary>
        [JsonProperty("intentInfo")]
        public IntentInfo IntentInfo { get; set; }

        /// <summary>
        /// Instructions for the next round of prompting the user.
        /// </summary>
        [JsonProperty("nextPromptBehaviors")]
        public PromptBehaviors NextPromptBehaviors { get; set; }

        [JsonProperty("customPayload")]
        //public Dictionary<string, object> CustomPayload { get; set; }

        public Dictionary<string, object> CustomPayload { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Diagnostic information for errors, and error handling behavior configuration.
        /// </summary>
        [JsonProperty("errorDetails")]
        public BotErrorDetails ErrorDetails { get; set; }

        [JsonProperty("botSessionState")]
        public BotSessionState BotSessionState { get; set; }
    }

}
