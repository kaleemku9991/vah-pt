using Newtonsoft.Json;

namespace proxyTask.Model
{
    public class PromptDefinition
    {
        /// <summary>
        /// Human readable, friendly text representation of the prompt. If the prompt is
        /// text-to-speech with markup for example, this representation is better for historical
        /// transcript. If the prompt is a pre-recorded wav file, this value will be used
        /// in transcript.
        /// </summary>
        [JsonProperty("transcript")]
        public string Transcript { get; set; }

        /// <summary>
        /// Audio payload supplied by the bot provider. The inContact MediaServer will save
        /// this file locally, but the file will only be referenceable during the lifetime
        /// of the phone call identified with ContactId (once the call ends, the file will
        /// be deleted). The MediaServer will automatically reformulate the "audioFilePath"
        /// to reference the local file, and remove this 'base64EncodedG711ulawWithWavHeader'
        /// payload before relaying the response to the ScriptEngine (too much data).
        /// </summary>
        [JsonProperty("base64EncodedG711ulawWithWavHeader")]
        public string Base64EncodedG711ulawWithWavHeader { get; set; }

        /// <summary>
        /// Optional path to a pre-recorded wav file. For Voice sessions, if a valid audioFilePath
        /// is supplied, the specified audio will be rendered instead of Text-To-Speech.
        /// Variables can be used in the path to support Persona variations (different personalities
        /// and languages). For example, a {persona} variable of 'Emma' might be a German female
        /// recording for '/prompts/{persona}/greeting.wav'.
        /// </summary>
        [JsonProperty("audioFilePath")]
        public string AudioFilePath { get; set; }

        /// <summary>
        /// Text to speech formatted representation of the prompt. SSML and other mark-up
        /// support varies by vendor. If 'textToSpeech' value is null or empty, the 'transcript'
        /// text will be used for TTS.
        /// </summary>
        [JsonProperty("textToSpeech")]
        public string TextToSpeech { get; set; }

        /// <summary>
        /// For chat channels for example, this may contain instructions to render an image
        /// or a link.
        /// </summary>
        [JsonProperty("mediaSpecificObject")]
        public object MediaSpecificObject { get; set; }
    }

}
