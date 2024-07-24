namespace proxyTask.Model
{
    public class BotErrorDetails
    {
        public enum BotLoopErrorBehavior
        {
            ReturnControlToScriptThroughErrorBranch,
            EndContact
        }

        //
        // Summary:
        //     Behavior to perform if BotProvider is unreachable or returns errors from any
        //     BotExchange Possible values: •ReturnControlToScriptThroughErrorBranch - Return
        //     control to script through Error Branch •EndContact - End the bot session (hang
        //     up on phone call, end chat, etc)
        public BotLoopErrorBehavior errorBehavior { get; set; }

        //
        // Summary:
        //     Optional sequence of prompts to play in case of error during the BotLoop. If
        //     populated, the sequence will be rendered to user PRIOR to executing the specified
        //     'errorBehavior'
        public PromptSequence errorPromptSequence { get; set; }

        //
        // Summary:
        //     Debug level information that will be logged by the system to assist in troubleshooting
        public string systemErrorMessage { get; set; }
    }
}
