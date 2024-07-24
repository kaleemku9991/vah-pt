namespace proxyTask.Model
{
    public class AudioCollectionRules
    {
        public enum UserInputCollectType
        {
            DO_NOT_COLLECT_USER_RESPONSE,
            SEND_UTTERANCE_AUDIO,
            SEND_DTMF_ONLY_AS_TEXT
        }
    }
}
