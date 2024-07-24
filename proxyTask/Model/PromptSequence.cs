namespace proxyTask.Model
{
    public class PromptSequence
    {
        //
        // Summary:
        //     Sequential lists of prompt definitions which will be rendered to the customer.
        //     For Voice sessions, the sequence can be a mix of Text-To-Speech and pre-recorded
        //     audio
        public List<PromptDefinition> prompts { get; set; }

        public PromptSequence()
        {
            prompts = new List<PromptDefinition>();
        }
    }
}
