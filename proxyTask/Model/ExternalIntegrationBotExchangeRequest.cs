using Newtonsoft.Json;

namespace proxyTask.Model
{
    using Newtonsoft.Json;

    public class ExternalIntegrationBotExchangeRequest
    {
        public enum UserInputType
        {
            NO_INPUT,
            TEXT,
            BASE64_ENCODED_G711_ULAW_WAV_FILE,
            USER_INPUT_ARCHIVED_AS_SPECIFIED,
            USER_ENDED_SESSION,
            AUTOMATED_TEXT,
            DTMF_AS_TEXT
        }

        [JsonProperty("virtualAgentId")]
        public string VirtualAgentId { get; set; }

        [JsonProperty("botConfig")]
        public string BotConfig { get; set; }

        [JsonProperty("userInput")]
        public string UserInput { get; set; }

        [JsonProperty("userInputType")]
        public UserInputType userInputType { get; set; }

        [JsonProperty("automatedIntent")]
        public string AutomatedIntent { get; set; }

        [JsonProperty("base64wavFile")]
        public string Base64WavFile { get; set; }

        [JsonProperty("customPayload")]
        public object CustomPayload { get; set; }

        [JsonProperty("mediaType")]
        public string MediaType { get; set; }

        [JsonProperty("botSessionState")]
        public BotSessionState BotSessionState { get; set; }
    }

}
