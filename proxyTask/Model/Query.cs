﻿using Newtonsoft.Json.Converters;
using System.Text;
using static proxyTask.Model.ExternalIntegrationBotExchangeRequest;

namespace proxyTask.Model
{

    public class Query
    {
        public QueryInput query_input { get; set; }
        public QueryParams query_params { get; set; }

        public UserInputType UserInputType { get; set; }
    }

    public class QueryInput
    {
        public TextInput Text { get; set; }
        public object Event { get; set; }
    }

    public class TextInput
    {
        public string Text { get; set; }
        public string LanguageCode { get; set; }
    }


    public class QueryParams
    {
        public object Payload { get; set; }
    }


}
