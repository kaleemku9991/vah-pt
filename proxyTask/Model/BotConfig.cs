using Newtonsoft.Json;
using System.Linq;


namespace proxyTask.Model
{
    public class EndpointParameter
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class BotConfig
    {
        [JsonProperty("webhookUrl")]
        public string WebhookUrl { get; set; }

        [JsonProperty("authorizationHeader")]
        public object AuthorizationHeader { get; set; }

        [JsonProperty("customHeaders")]
        public object[] CustomHeaders { get; set; }

        [JsonProperty("endpointParameters")]
        public EndpointParameter[] EndpointParameters { get; set; }

        [JsonProperty("schemaVersion")]
        public string SchemaVersion { get; set; }

        [JsonProperty("timeout")]
        public int Timeout { get; set; }

        [JsonProperty("webhookClientCertificates")]
        public object[] WebhookClientCertificates { get; set; }

        /// <summary>
        /// Retrieves the value of an endpoint parameter by name.
        /// </summary>
        /// <param name="name">The name of the endpoint parameter to retrieve.</param>
        /// <returns>The value of the endpoint parameter if found; otherwise, null.</returns>
        public string GetEndpointParameter(string name)
        {
            var parameter = EndpointParameters.FirstOrDefault(p => p.Name == name);
            return parameter?.Value;
        }
    }

}
