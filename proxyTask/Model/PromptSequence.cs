﻿using Newtonsoft.Json;

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
        public List<PromptDefinition> prompts { get; set; }

        public PromptSequence()
        {
            prompts = new List<PromptDefinition>();
        }
    }

}
