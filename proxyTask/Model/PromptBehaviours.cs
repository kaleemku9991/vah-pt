using Newtonsoft.Json;

namespace proxyTask.Model
{

    public class PromptBehaviors
    {
        /// <summary>
        /// Rules about how long to wait for human and machine prompts, and what to do in
        /// case of delay.
        /// </summary>
        [JsonProperty("silenceRules")]
        public SilenceRules SilenceRules { get; set; }

        /// <summary>
        /// Rules on how to capture user input through audio channel.
        /// </summary>
        [JsonProperty("audioCollectionRules")]
        public AudioCollectionRules AudioCollectionRules { get; set; }

        /// <summary>
        /// Construct a PromptBehavior object.
        /// </summary>
        public PromptBehaviors()
        {
            SilenceRules = new SilenceRules();
            AudioCollectionRules = new AudioCollectionRules();
        }
    }


}
