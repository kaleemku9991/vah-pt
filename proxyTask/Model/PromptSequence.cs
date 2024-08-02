using Newtonsoft.Json;

namespace proxyTask.Model
{
    public class PromptSequence
    {
        /// <summary>
        /// Sequential lists of prompt definitions which will be rendered to the customer.
        /// For voice sessions, the sequence can be a mix of text-to-speech and pre-recorded
        /// audio.
        /// </summary>
        /// 

        [JsonProperty("prompts")]
        public List<PromptDefinition> Prompts { get; set; }

        public PromptSequence()
        {
            Prompts = new List<PromptDefinition>();
        }
    }

}
