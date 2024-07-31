using Newtonsoft.Json;

namespace proxyTask.Model
{

    public class BotSessionState
    {
        /// <summary>
        /// Unique identifier for the session.
        /// </summary>
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
    }

}
