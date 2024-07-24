namespace proxyTask.Model
{
    public class SilenceRules
    {
        //
        // Summary:
        //     If true, the comfortPromptSequence will be rendered if 'botResponseDelayTolerance'
        //     elapses before the next BotExchangeResponse is returned
        public bool engageComfortSequence { get; set; }

        //
        // Summary:
        //     How long (in milliseconds) the user should endure silence from the bot before
        //     comfortSequence starts (if engageComfortSequence is true) Anecdotal recommendation
        //     is that more than 4 seconds (4000ms) of silence in a conversation becomes awkward.
        public int botResponseDelayTolerance { get; set; }

        //
        // Summary:
        //     Sequence of prompts to render if botResponseDelayTolerance is exceeded
        public PromptSequence comfortPromptSequence { get; set; }

        //
        // Summary:
        //     Overall interval to wait for user input, before submitting a NO_INPUT
        public int millisecondsToWaitForUserResponse { get; set; }
    }

}
