using Newtonsoft.Json.Converters;
using System.Text;

namespace proxyTask.Model
{

    public class Query
    {
        public QueryInput Query_Input { get; set; }
        public QueryParams Query_Params { get; set; }
    }

    public class QueryInput
    {
        public TextInput Text { get; set; }
    }

    public class TextInput
    {
        public string Text { get; set; }
        public string Language_Code { get; set; }
    }

    public class QueryParams
    {
        public object Payload { get; set; }
    }

}
