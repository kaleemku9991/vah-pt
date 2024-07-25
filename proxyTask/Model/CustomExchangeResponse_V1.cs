namespace proxyTask.Model
{
    public class CustomExchangeResponse_V1
    {
        public enum BotExchangeBranch
        {
            DoNotBegin,
            PromptAndCollectNextResponse,
            ReturnControlToScript,
            EndContact,
            AudioInputUntranscribeable,
            Error,
            DTMFBreakout,
            UserInputTimeout,
            UserInputNotUnderstood
        }

        public BotExchangeBranch branchName { get; set; }
        //
        // Summary:
        //     Sequence of prompts to be rendered to customer on next round of the BotLoop
        public PromptSequence nextPromptSequence { get; set; }

        //
        // Summary:
        //     Information from the bot on current user intent
        public IntentInfo intentInfo { get; set; }

        //
        // Summary:
        //     Instructions for the next round of prompting the user
        public PromptBehaviors nextPromptBehaviors { get; set; }

        public Dictionary<string, object> customPayload { get; set; }

        //
        // Summary:
        //     Diagnostic information for errors, and error handling behavior configuration
        public BotErrorDetails errorDetails { get; set; }

        public BotSessionState botSessionState { get; set; }
    }

}
