namespace proxyTask.Model
{
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

        public string virtualAgentId { get; set; }

        public string botConfig { get; set; }

        public string userInput { get; set; }

        public UserInputType userInputType { get; set; }

        public string automatedIntent { get; set; }

        public string base64wavFile { get; set; }

        public object botSessionState { get; set; }

        public object custompPayload { get; set; }

        public string mediaType { get; set; }
    }
}
