namespace proxyTask.Model
{
    public class IntentInfo
    {
        //
        // Summary:
        //     A string identification of the current intent recognized by the bot
        public string intent { get; set; }

        //
        // Summary:
        //     A string identification of the current context known by the bot (context of the
        //     intent)
        public string context { get; set; }

        //
        // Summary:
        //     Level of confidence in the Intent being correctly identified
        public float intentConfidence { get; set; }

        //
        // Summary:
        //     The last user input transcript
        public string lastUserUtterance { get; set; }

        //
        // Summary:
        //     Slot name and values for the current intent recognition
        public Dictionary<string, object> Slots { get; set; }

        public IntentInfo()
        {
            Slots = new Dictionary<string, object>();
        }
    }

}
