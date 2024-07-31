using Newtonsoft.Json;

namespace proxyTask.Model
{

    public class SilenceRules
    {
        /// <summary>
        /// If true, the ComfortPromptSequence will be rendered if 'BotResponseDelayTolerance'
        /// elapses before the next BotExchangeResponse is returned.
        /// </summary>
        [JsonProperty("engageComfortSequence")]
        public bool EngageComfortSequence { get; set; }

        /// <summary>
        /// How long (in milliseconds) the user should endure silence from the bot before
        /// ComfortSequence starts (if EngageComfortSequence is true). Anecdotal recommendation
        /// is that more than 4 seconds (4000ms) of silence in a conversation becomes awkward.
        /// </summary>
        [JsonProperty("botResponseDelayTolerance")]
        public int BotResponseDelayTolerance { get; set; }

        /// <summary>
        /// Sequence of prompts to render if BotResponseDelayTolerance is exceeded.
        /// </summary>
        [JsonProperty("comfortPromptSequence")]
        public PromptSequence ComfortPromptSequence { get; set; }

        /// <summary>
        /// Overall interval to wait for user input, before submitting a NO_INPUT.
        /// </summary>
        [JsonProperty("millisecondsToWaitForUserResponse")]
        public int MillisecondsToWaitForUserResponse { get; set; }
    }

}
