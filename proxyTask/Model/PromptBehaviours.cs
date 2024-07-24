namespace proxyTask.Model
{
    public class PromptBehaviors
    {
        //
        // Summary:
        //     Rules about how long to wait for human and machine prompts, and what to do in
        //     case of delay
        public SilenceRules silenceRules { get; set; }

        //
        // Summary:
        //     Rules on how to capture user input through audio channel
        public AudioCollectionRules audioCollectionRules { get; set; }

        //
        // Summary:
        //     Construct a PromptBehavior object
        public PromptBehaviors()
        {
            silenceRules = new SilenceRules();
            audioCollectionRules = new AudioCollectionRules();
        }
    }

}
