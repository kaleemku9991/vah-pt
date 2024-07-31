using Newtonsoft.Json;

namespace proxyTask.Model
{

    public class NextPromptBehaviour
    {
        /// <summary>
        /// The silence rules as a JSON string.
        /// </summary>
        [JsonProperty("silenceRules")]
        public string SilenceRules { get; set; }

        /// <summary>
        /// The audio collection rules as a JSON string.
        /// </summary>
        [JsonProperty("audioCollectionRules")]
        public string AudioCollectionRules { get; set; }
    }

}
