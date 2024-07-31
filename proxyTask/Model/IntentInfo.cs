using Newtonsoft.Json;
using System.Collections.Generic;

namespace proxyTask.Model
{

    public class IntentInfo
    {
        /// <summary>
        /// A string identification of the current intent recognized by the bot.
        /// </summary>
        [JsonProperty("intent")]
        public string Intent { get; set; }

        /// <summary>
        /// A string identification of the current context known by the bot (context of the
        /// intent).
        /// </summary>
        [JsonProperty("context")]
        public string Context { get; set; }

        /// <summary>
        /// Level of confidence in the Intent being correctly identified.
        /// </summary>
        [JsonProperty("intentConfidence")]
        public float IntentConfidence { get; set; }

        /// <summary>
        /// The last user input transcript.
        /// </summary>
        [JsonProperty("lastUserUtterance")]
        public string LastUserUtterance { get; set; }

        /// <summary>
        /// Slot name and values for the current intent recognition.
        /// </summary>
        [JsonProperty("slots")]
        public Dictionary<string, object> Slots { get; set; }

        public IntentInfo()
        {
            Slots = new Dictionary<string, object>();
        }
    }


}
